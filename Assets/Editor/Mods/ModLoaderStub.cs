using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModApi;
using ModApi.Attributes;

[ModLoaderId("ModLoaderStub")]
public class ModLoaderStub : IModLoader
{
	private readonly IEnumerable<Type> mods;
	private readonly Dictionary<IModInfo, Type> registeredMods = new Dictionary<IModInfo, Type>();

	public ModLoaderStub(params Type[] _mods)
	{
		mods = _mods;
	}

	public void OnLoad(IModLoaderContext _context)
	{
		foreach (var modType in mods)
		{
			var modIdAttribute = Attribute.GetCustomAttribute(modType, typeof(ModIdAttribute)) as ModIdAttribute;
			var modId = modIdAttribute == null ? Guid.NewGuid().ToString() : modIdAttribute.Id;

			var dependsOnAttribute = Attribute.GetCustomAttribute(modType, typeof(DependsOnAttribute)) as DependsOnAttribute;
			var dependencies = dependsOnAttribute == null ? Enumerable.Empty<string>() : dependsOnAttribute.Dependencies;

			var modInfo = _context.GetModInfoBuilder()
				.WithId(modId)
				.WithDependencies(dependencies)
				.Register();

			if (modInfo == null)
			{
				Console.WriteLine("Failed to load " + modId);
				continue;
			}

			registeredMods.Add(modInfo, modType);
		}
	}

	public IMod Load(IModInfo _modInfo)
	{
		var modType = registeredMods[_modInfo];

		if (modType == null)
		{
			throw new ArgumentException(_modInfo + " was not registered by this mod loader");
		}

		return (IMod) Activator.CreateInstance(modType);
	}
}