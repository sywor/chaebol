using System;
using System.Collections.Generic;
using System.Linq;
using ModApi;

namespace ModApiTest
{
	public class ModInfo : IModInfo
	{
		public string Id { get; }
		public IEnumerable<string> Dependencies { get; }
		
		public ModInfo(ModInfoBuilder _modInfoBuilder)
		{
			Id = _modInfoBuilder.Id ?? _modInfoBuilder.ModLoaderContext.Id + ':' + Guid.NewGuid();
			Dependencies = _modInfoBuilder.Dependencies ?? Enumerable.Empty<string>();
		}

		public override string ToString()
		{
			return $"{Id}, DependsOn: [{string.Join(", ", Dependencies)}]";
		}
	}
}