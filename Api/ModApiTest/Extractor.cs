using ModApi;

namespace ModApiTest
{
    public class Extractor : IExtractor
    {
        public string ModId { get; }
        public IResource MyResource { get; }

        public Extractor(ExtractorBuilder _builder)
        {
            ModId = _builder.ModId;
            MyResource = _builder.Resource;
        }
    }
}