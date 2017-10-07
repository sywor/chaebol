namespace ModApi
{
    public interface IResourceBuilder : IBuilder<IResource>
    {
        IResourceBuilder WithResourceName(string _name);
    }
}