using System;
using System.Collections.Generic;
using System.Linq;

public class DependencySolver
{
	public struct ModAndDependencies
	{
		public readonly ModInfo Mod;
		public readonly HashSet<string> UnresolvedDependencies;

		public ModAndDependencies(ModInfo _mod, IEnumerable<string> _unresolvedDependencies)
		{
			Mod = _mod;
			UnresolvedDependencies = new HashSet<string>(_unresolvedDependencies);
		}
	}

	public struct Result
	{
		public readonly List<ModInfo> SortedDependencies;
		public readonly List<ModInfo> SkippedDuplicates;
		public readonly List<ModInfo> WithMissingDependencies;
		public readonly List<ModAndDependencies> WithCycles;

		public Result(List<ModInfo> _sortedDependencies,
			List<ModInfo> _skippedDuplicates,
			List<ModInfo> _withMissingDependencies,
			List<ModAndDependencies> _withCycles)
		{
			SortedDependencies = _sortedDependencies;
			SkippedDuplicates = _skippedDuplicates;
			WithMissingDependencies = _withMissingDependencies;
			WithCycles = _withCycles;
		}
	}

	public static Result Solve(IEnumerable<ModInfo> _modInfos)
	{
		var registeredModIds = new HashSet<string>();
		var modsWithValidIds = new List<ModInfo>();
		var skippedDuplicates = new List<ModInfo>();

		foreach (var modInfo in _modInfos)
		{
			var modId = modInfo.Id;

			if (registeredModIds.Contains(modId))
			{
				skippedDuplicates.Add(modInfo);
				continue;
			}

			modsWithValidIds.Add(modInfo);
			registeredModIds.Add(modId);
		}

		var groupByResolve = modsWithValidIds.ToLookup(_m => registeredModIds.IsSupersetOf(_m.Dependencies));
		var unresolvedMods = groupByResolve[false].ToList();
		var modsThatCanBeResolved = groupByResolve[true].ToList();

		IEnumerable<ModAndDependencies> modsWithCycles;
		var modsByDependency = SortModsByDependency(modsThatCanBeResolved, out modsWithCycles).ToList();

		return new Result(modsByDependency,
			skippedDuplicates,
			unresolvedMods,
			modsWithCycles.ToList());
	}

	private static IEnumerable<ModInfo> SortModsByDependency(IEnumerable<ModInfo> _mods,
		out IEnumerable<ModAndDependencies> _modsWithCycles)
	{
		var modsWithDependencies = _mods.Select(_m => new ModAndDependencies(_m, _m.Dependencies)).ToList();

		var sortedMods = new List<ModInfo>();

		while (modsWithDependencies.Any())
		{
			var groupByResolved = modsWithDependencies.ToLookup(_m => !_m.UnresolvedDependencies.Any());
			var withoutUnresolvedDependencies = groupByResolved[true].Select(_m => _m.Mod).ToList();

			if (withoutUnresolvedDependencies.Any())
			{
				withoutUnresolvedDependencies.Sort((_lhs, _rhs) => String.Compare(_lhs.Id, _rhs.Id, StringComparison.Ordinal));
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

		_modsWithCycles = modsWithDependencies;
		return sortedMods;
	}
}