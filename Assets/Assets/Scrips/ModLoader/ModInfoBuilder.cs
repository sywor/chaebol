using System.Collections.Generic;
using System.Linq;
using ModApi;

public class ModInfoBuilder : IModInfoBuilder
{
	public ModLoaderContext ModLoaderContext { get; private set; }
	public string Id { get; private set; }
	public List<string> Dependencies { get; private set; }

	public ModInfoBuilder(ModLoaderContext _modLoaderContext)
	{
		ModLoaderContext = _modLoaderContext;
	}

	public IModInfoBuilder WithId(string _id)
	{
		Id = _id;
		return this;
	}

	public IModInfoBuilder WithDependencies(IEnumerable<string> _dependencies)
	{
		Dependencies = _dependencies.ToList();
		return this;
	}

	public IModInfo Register()
	{
		var modInfo = new ModInfo(this);
		ModLoaderContext.Register(modInfo);
		return modInfo;
	}
}