using System;
using System.Collections.Generic;
using System.Linq;
using ModApi;

public class ModContext : IModContext
{
	public readonly string ModId;
	public readonly List<string> Dependencies;
	public readonly List<Extractor> Extractors = new List<Extractor>();
	public readonly List<Resource> Resources = new List<Resource>();

	private readonly IMod myMod;

	private static readonly Dictionary<Type, Func<ModContext, IBuilder<IModObject>>> RegisteredBuilders =
		new Dictionary<Type, Func<ModContext, IBuilder<IModObject>>>
		{
			{typeof(IResourceBuilder), _context => new ResourceBuilder(_context)},
			{typeof(IExtractorBuilder), _context => new ExtractorBuilder(_context)}
		};

	public ModContext(IMod _mod, string _modId, IEnumerable<string> _dependencies)
	{
		myMod = _mod;
		ModId = _modId;
		Dependencies = _dependencies.ToList();
	}

	public override string ToString()
	{
		return ModId + "(" + myMod + ") Depends on: [" + string.Join(", ", Dependencies.ToArray()) + "]";
	}

	public TBuilder GetBuilder<TBuilder, TPlaceable>() where TBuilder : IBuilder<TPlaceable> where TPlaceable : IModObject
	{
		var registeredBuilder = RegisteredBuilders[typeof(TBuilder)];
		if (registeredBuilder == null)
		{
			throw new ArgumentException("Unkown Builder Type");
		}

		return (TBuilder) registeredBuilder.Invoke(this);
	}

	public IEnumerable<IResource> GetResources()
	{
		return (IEnumerable<IResource>) Resources;
	}

	public void Load()
	{
		myMod.OnLoad(this);
	}

	public void RegisterResource(Resource _resource)
	{
		Resources.Add(_resource);
	}

	public void RegisterExtractor(Extractor _extractor)
	{
		Extractors.Add(_extractor);
	}
}