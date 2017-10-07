namespace ModApi
{
    public interface IExtractorBuilder : IBuilder<IExtractor>
    {
        IExtractorBuilder WithResource(IResource _resource);
    }
}