using ModApi;

namespace ModApiTest
{
    public class Resource : IResource
    {
        public string ModId { get; }
        public string Name { get; }

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
}