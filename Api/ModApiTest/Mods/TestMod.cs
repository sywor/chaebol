using ModApi;
using ModApi.Attributes;

namespace ModApiTest.Mods
{
    [ModId("TestMod")]
    [DependsOn("SomeWeirdMod", "TheModEveryoneUses")]
    public class TestMod : IMod
    {
        public void OnLoad(IModContext _modContext)
        {
            IResource vespeneGasResource = _modContext.GetBuilder<IResourceBuilder, IResource>()
                .WithResourceName("Vespene Gas")
                .BuildAndRegister();

            _modContext.GetBuilder<IExtractorBuilder, IExtractor>()
                .WithResource(vespeneGasResource)
                .BuildAndRegister();
        }

        public override string ToString()
        {
            return "My Awesome Mod!";
        }
    }
}