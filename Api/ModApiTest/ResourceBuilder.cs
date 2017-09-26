using ModApi;

namespace ModApiTest
{
    public class ResourceBuilder : IResourceBuilder
    {
        public string ModId { get; }
        public string Name { get; private set; }

        private readonly ModContext myModContext;

        public ResourceBuilder(ModContext _modContext)
        {
            ModId = _modContext.ModId;
            myModContext = _modContext;
        }

        public IResourceBuilder WithResourceName(string _name)
        {
            Name = _name;
            return this;
        }

        public IResource BuildAndRegister()
        {
            Resource resource = new Resource(this);
            myModContext.RegisterResource(resource);
            return resource;
        }
    }
}