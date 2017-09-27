namespace ModApi
{
	public interface IModLoader
	{
		void OnLoad(IModLoaderContext _context);
		IMod Load(IModInfo _modInfo);
	}
}