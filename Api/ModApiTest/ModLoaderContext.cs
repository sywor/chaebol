using System.Collections.Generic;
using ModApi;

namespace ModApiTest
{
	public class ModLoaderContext : IModLoaderContext
	{
		public readonly string Id;
		public List<ModInfo> RegisteredModInfos { get; } = new List<ModInfo>();
		
		private readonly IModLoader modLoader;

		public ModLoaderContext(IModLoader _modLoader, string _id)
		{
			modLoader = _modLoader;
			Id = _id;
		}

		public IModInfoBuilder GetModInfoBuilder()
		{
			return new ModInfoBuilder(this);
		}

		public void Register(ModInfo _modInfo)
		{
			RegisteredModInfos.Add(_modInfo);
		}
	}
}