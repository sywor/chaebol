using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModApi;
using ModApi.Attributes;

namespace ModApiTest
{
	internal class Program
	{
		public static void Main(string[] _args)
		{
			var assemblyExportedTypes = typeof(Program).Assembly.ExportedTypes;
			var modTypes = assemblyExportedTypes.Where(_t => _t.GetInterfaces().Contains(typeof(IMod))).ToList();

			var modContexts = new List<ModContext>();
			var loadedModIds = new HashSet<string>();

			foreach (var modType in modTypes)
			{
				var mod = (IMod) Activator.CreateInstance(modType);

				var modIdAttribute = modType.GetCustomAttribute<ModIdAttribute>();
				var modId = modIdAttribute == null ? Guid.NewGuid().ToString() : modIdAttribute.Id;

				if (loadedModIds.Contains(modId))
				{
					Console.WriteLine($"Duplicate ModId detected: {modId}\n" +
					                  $"Ignoring {modType}");
					continue;
				}

				var dependsOnAttribute = modType.GetCustomAttribute<DependsOnAttribute>();
				var dependencies = dependsOnAttribute == null ? Enumerable.Empty<string>() : dependsOnAttribute.Dependencies;

				var context = new ModContext(mod, modId, dependencies);
				modContexts.Add(context);
				loadedModIds.Add(modId);
			}

			var groupByResolve = modContexts.GroupBy(_m => loadedModIds.IsSupersetOf(_m.Dependencies)).ToList();
			var unresolvedMods = (groupByResolve.SingleOrDefault(_g => !_g.Key) ?? Enumerable.Empty<ModContext>()).ToList();
			if (unresolvedMods.Any())
			{
				Console.WriteLine("Mods with missing dependencies:");
				foreach (var unresolved in unresolvedMods)
				{
					Console.WriteLine(unresolved);
					Console.WriteLine("Missing: " + string.Join(", ", unresolved.Dependencies.Except(loadedModIds)));
				}
			}

			var modsThatCanBeResolved = groupByResolve.SingleOrDefault(_g => _g.Key) ?? Enumerable.Empty<ModContext>();

			var modsByDependency = SortModsByDependency(modsThatCanBeResolved).ToList();

			Console.WriteLine("Mods found (" + modsByDependency.Count + "):");
			modsByDependency.ForEach(_m => Console.WriteLine("  " + _m));
			Console.WriteLine();

			foreach (var modContext in modsByDependency)
			{
				Console.WriteLine("Loading mod: " + modContext.ModId);
				modContext.Load();

				Console.WriteLine("Registered resources:");
				modContext.Resources.ForEach(_res => Console.WriteLine("  " + _res));
				Console.WriteLine("Registered extractors:");
				modContext.Extractors.ForEach(_ex => Console.WriteLine("  " + _ex));
				Console.WriteLine();
			}
		}

		private struct ModAndDependencies
		{
			public readonly ModContext Mod;
			public readonly HashSet<string> UnresolvedDependencies;

			public ModAndDependencies(ModContext _mod, IEnumerable<string> _unresolvedDependencies)
			{
				Mod = _mod;
				UnresolvedDependencies = new HashSet<string>(_unresolvedDependencies);
			}
		}

		private static IEnumerable<ModContext> SortModsByDependency(IEnumerable<ModContext> _mods)
		{
			var modsWithDependencies = _mods.Select(_m => new ModAndDependencies(_m, _m.Dependencies)).ToList();

			var sortedMods = new List<ModContext>();

			while (modsWithDependencies.Any())
			{
				var groupByResolved = modsWithDependencies.ToLookup(_m => !_m.UnresolvedDependencies.Any());
				var withoutUnresolvedDependencies = groupByResolved[true].Select(_m => _m.Mod).ToList();

				if (withoutUnresolvedDependencies.Any())
				{
					withoutUnresolvedDependencies.Sort((_lhs, _rhs) => string.Compare(_lhs.ModId, _rhs.ModId, StringComparison.Ordinal));
					sortedMods.AddRange(withoutUnresolvedDependencies);

					var resolvedModIds = withoutUnresolvedDependencies.Select(_m => _m.ModId);
					modsWithDependencies = groupByResolved[false]
						.Select(_m => new ModAndDependencies(_m.Mod, _m.UnresolvedDependencies.Except(resolvedModIds)))
						.ToList();
				}
				else
				{
					break;
				}
			}

			if (modsWithDependencies.Any())
			{
				Console.WriteLine("Mods with dependency cycles detected:");
				modsWithDependencies.ForEach(_m =>
				{
					Console.WriteLine(_m.Mod);
					Console.WriteLine("Unresolved: " + string.Join(", ", _m.UnresolvedDependencies));
				});
				Console.WriteLine();
			}

			return sortedMods;
		}
	}
}