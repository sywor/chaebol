using System;
using System.Collections.Generic;
using System.Linq;
using ModApi;

public class ModInfo : IModInfo
{
	public string Id { get; private set; }
	public IEnumerable<string> Dependencies { get; private set; }

	public ModInfo(ModInfoBuilder _modInfoBuilder)
	{
		Id = _modInfoBuilder.Id ?? _modInfoBuilder.ModLoaderContext.Id + ':' + Guid.NewGuid();
		Dependencies = _modInfoBuilder.Dependencies ?? Enumerable.Empty<string>();
	}

	public override string ToString()
	{
		return Id + ", DependsOn: [" + string.Join(", ", Dependencies.ToArray()) + "]";
	}
}