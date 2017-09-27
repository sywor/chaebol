using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModApi.Attributes;
using ModApiTest.ModLoaders;

namespace ModApiTest
{
	internal class Program
	{
		public static void Main(string[] _args)
		{
			var defaultModLoader = new DefaultModLoader();
			var id = typeof(DefaultModLoader).GetCustomAttribute<ModLoaderIdAttribute>().Id;

			var modLoaderContext = new ModLoaderContext(defaultModLoader, id);
			defaultModLoader.OnLoad(modLoaderContext);

			var registeredModIds = new HashSet<string>();
			var modInfos = new List<ModInfo>();
			
			foreach (var modInfo in modLoaderContext.RegisteredModInfos)
			{
				var modId = modInfo.Id;

				if (registeredModIds.Contains(modId))
				{
					Console.WriteLine($"Duplicate ModId detected: {modId}\n" +
					                  $"Ignoring {modLoaderContext.Id}:{modId}");
					continue;
				}

				modInfos.Add(modInfo);
				registeredModIds.Add(modId);
			}

			var groupByResolve = modInfos.ToLookup(_m => registeredModIds.IsSupersetOf(_m.Dependencies));
			var unresolvedMods = groupByResolve[false].ToList();
			if (unresolvedMods.Any())
			{
				Console.WriteLine("Mods with missing dependencies:");
				foreach (var unresolved in unresolvedMods)
				{
					Console.WriteLine(unresolved);
					Console.WriteLine("Missing: " + string.Join(", ", unresolved.Dependencies.Except(registeredModIds)));
				}
			}

			var modsThatCanBeResolved = groupByResolve[true];
			var modsByDependency = SortModsByDependency(modsThatCanBeResolved).ToList();

			Console.WriteLine("Mods found (" + modsByDependency.Count + "):");
			modsByDependency.ForEach(_m => Console.WriteLine("  " + _m));
			Console.WriteLine();

			foreach (var modInfo in modsByDependency)
			{
				Console.WriteLine("Loading mod: " + modLoaderContext.Id + ":" + modInfo.Id);
				
				var instantiatedMod = defaultModLoader.Load(modInfo);
				var modContext = new ModContext(instantiatedMod, modInfo.Id, modInfo.Dependencies);

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
			public readonly ModInfo Mod;
			public readonly HashSet<string> UnresolvedDependencies;

			public ModAndDependencies(ModInfo _mod, IEnumerable<string> _unresolvedDependencies)
			{
				Mod = _mod;
				UnresolvedDependencies = new HashSet<string>(_unresolvedDependencies);
			}
		}

		private static List<ModInfo> SortModsByDependency(IEnumerable<ModInfo> _mods)
		{
			var modsWithDependencies = _mods.Select(_m => new ModAndDependencies(_m, _m.Dependencies)).ToList();

			var sortedMods = new List<ModInfo>();

			while (modsWithDependencies.Any())
			{
				var groupByResolved = modsWithDependencies.ToLookup(_m => !_m.UnresolvedDependencies.Any());
				var withoutUnresolvedDependencies = groupByResolved[true].Select(_m => _m.Mod).ToList();

				if (withoutUnresolvedDependencies.Any())
				{
					withoutUnresolvedDependencies.Sort((_lhs, _rhs) => string.Compare(_lhs.Id, _rhs.Id, StringComparison.Ordinal));
					sortedMods.AddRange(withoutUnresolvedDependencies);

					var resolvedModIds = withoutUnresolvedDependencies.Select(_m => _m.Id);
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