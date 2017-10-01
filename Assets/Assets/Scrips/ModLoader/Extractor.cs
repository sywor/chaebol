using ModApi;

public class Extractor : IExtractor
{
	public string ModId { get; private set; }
	public IResource MyResource { get; private set; }

	public Extractor(ExtractorBuilder _builder)
	{
		ModId = _builder.ModId;
		MyResource = _builder.Resource;
	}
}