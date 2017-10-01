using ModApi;

public class Resource : IResource
{
	public string ModId { get; private set; }
	public string Name { get; private set; }

	public Resource(ResourceBuilder _builder)
	{
		ModId = _builder.ModId;
		Name = _builder.Name;
	}

	public override string ToString()
	{
		return Name;
	}
}