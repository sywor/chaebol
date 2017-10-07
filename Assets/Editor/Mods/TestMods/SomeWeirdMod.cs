using ModApi;
using ModApi.Attributes;

[ModId("SomeWeirdMod")]
[DependsOn("TheModEveryoneUses")]
public class SomeWeirdMod : IMod
{
	public void OnLoad(IModContext _modContext)
	{
		IResource sewageResource = _modContext.GetBuilder<IResourceBuilder, IResource>()
			.WithResourceName("Sewage")
			.BuildAndRegister();

		_modContext.GetBuilder<IExtractorBuilder, IExtractor>()
			.WithResource(sewageResource)
			.BuildAndRegister();
	}
}