using ModApi;
using ModApi.Attributes;

[ModId("TheModEveryoneUses")]
public class TheModEveryoneUses : IMod
{
	public void OnLoad(IModContext _modContext)
	{
		IResource diamondGasResource = _modContext.GetBuilder<IResourceBuilder, IResource>()
			.WithResourceName("Diamond Gas")
			.BuildAndRegister();

		_modContext.GetBuilder<IExtractorBuilder, IExtractor>()
			.WithResource(diamondGasResource)
			.BuildAndRegister();
	}
}