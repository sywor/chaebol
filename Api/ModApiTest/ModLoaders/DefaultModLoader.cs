﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModApi;
using ModApi.Attributes;

namespace ModApiTest.ModLoaders
{
	[ModLoaderId("DefaultModLoader")]
	public class DefaultModLoader : IModLoader
	{
		private readonly Dictionary<IModInfo, Type> registeredMods = new Dictionary<IModInfo, Type>();
		
		public void OnLoad(IModLoaderContext _context)
		{
			IEnumerable<Assembly> assemblies = new[] {typeof(DefaultModLoader).Assembly};

			foreach (var assembly in assemblies)
			{
				var modTypes = assembly.ExportedTypes.Where(_t => _t.GetInterfaces().Contains(typeof(IMod)));

				foreach (var modType in modTypes)
				{
					var modIdAttribute = modType.GetCustomAttribute<ModIdAttribute>();
					var modId = modIdAttribute == null ? Guid.NewGuid().ToString() : modIdAttribute.Id;

					var dependsOnAttribute = modType.GetCustomAttribute<DependsOnAttribute>();
					var dependencies = dependsOnAttribute == null ? Enumerable.Empty<string>() : dependsOnAttribute.Dependencies;
					
					var modInfo = _context.GetModInfoBuilder()
						.WithId(modId)
						.WithDependencies(dependencies)
						.Register();

					if (modInfo == null)
					{
						Console.WriteLine("Failed to load " + modId + " from " + assembly);
						continue;
					}
					
					registeredMods.Add(modInfo, modType);
				}
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
}