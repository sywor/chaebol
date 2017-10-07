using System;
using System.Linq;
using System.Reflection;
using ModApi;
using ModApi.Attributes;
using NUnit.Framework;

public class ModApiTest
{
	[Test]
	public void TestBitsOfModLoading()
	{
		var modLoaderStub = new ModLoaderStub(typeof(SomeWeirdMod), typeof(TestMod), typeof(TheModEveryoneUses));

		var modLoaderContext = new ModLoaderContext(modLoaderStub, "ModLoaderStub");
		modLoaderStub.OnLoad(modLoaderContext);
		var registeredModInfos = modLoaderContext.RegisteredModInfos;

		var result = DependencySolver.Solve(registeredModInfos);

		Assert.That(result.SkippedDuplicates, Is.Empty);
		Assert.That(result.WithMissingDependencies, Is.Empty);
		Assert.That(result.WithCycles, Is.Empty);

		Assert.That(result.SortedDependencies.Select(_m => _m.Id),
			Is.EquivalentTo(new[] {"TheModEveryoneUses", "SomeWeirdMod", "TestMod"}));

		var loadedMods = result.SortedDependencies.Select(modLoaderStub.Load);
		Assert.That(loadedMods.Select(_m => _m.GetType()),
			Is.EquivalentTo(new[] {typeof(TheModEveryoneUses), typeof(SomeWeirdMod), typeof(TestMod)}));
	}
}