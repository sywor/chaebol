using ModApi;

namespace ModApiTest
{
    public class ExtractorBuilder : IExtractorBuilder
    {
        public string ModId { get; }
        public IResource Resource { get; private set; }

        private readonly ModContext myModContext;

        public ExtractorBuilder(ModContext _modContext)
        {
            ModId = _modContext.ModId;
            myModContext = _modContext;
        }

        public IExtractorBuilder WithResource(IResource _resource)
        {
            Resource = _resource;
            return this;
        }

        public IExtractor BuildAndRegister()
        {
            Extractor extractor = new Extractor(this);
            myModContext.RegisterExtractor(extractor);
            return extractor;
        }
    }
}