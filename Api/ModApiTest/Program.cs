using System;
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
			var registeredModInfos = modLoaderContext.RegisteredModInfos;

			var result = DependencySolver.Solve(registeredModInfos);

			foreach (var duplicate in result.SkippedDuplicates)
			{
				Console.WriteLine($"Duplicate ModId detected: {duplicate.Id}\n" +
				                  $"Ignoring {modLoaderContext.Id}:{duplicate.Id}");
			}

			if (result.WithMissingDependencies.Any())
			{
				var registeredModIds = registeredModInfos.Select(_m => _m.Id).ToList();

				Console.WriteLine("Mods with missing dependencies:");
				foreach (var unresolved in result.WithMissingDependencies)
				{
					Console.WriteLine(unresolved);
					Console.WriteLine("Missing: " + string.Join(", ", unresolved.Dependencies.Except(registeredModIds)));
				}
			}

			if (result.WithCycles.Any())
			{
				Console.WriteLine("Mods with dependency cycles detected:");
				result.WithCycles.ForEach(_m =>
				{
					Console.WriteLine(_m.Mod);
					Console.WriteLine("Unresolved: " + string.Join(", ", _m.UnresolvedDependencies));
				});
				Console.WriteLine();
			}

			Console.WriteLine("Mods found (" + result.SortedDependencies.Count + "):");
			result.SortedDependencies.ForEach(_m => Console.WriteLine("  " + _m));
			Console.WriteLine();

			foreach (var modInfo in result.SortedDependencies)
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
	}
}