
namespace Assets.GDI.Code.Graph
{
	/// <summary>
	/// This class contains the data of a request for the graph.
	/// </summary>
	public struct Request
	{
		public float X;
		public float Y;
		public float Z;
		public float Seed;
		public float SizeX;
		public float SizeY;
		public float SizeZ;

		public Request(float x, float y, float z, float sizeX, float sizeY, float sizeZ, float seed)
		{
			X = x;
			Y = y;
			Z = z;
			SizeX = sizeX;
			SizeY = sizeY;
			SizeZ = sizeZ;
			Seed = seed;
		}
	}
}
