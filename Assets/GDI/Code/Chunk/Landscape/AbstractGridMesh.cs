using UnityEngine;

namespace Assets.GDI.Code.Chunk.Landscape
{
	public abstract class AbstractGridMesh : MonoBehaviour
	{

		public abstract void PrepareGrid(int cols, int rows);
		public abstract void Create();
		public abstract void ApplyNormals();
		public abstract void SetHeight(int pointX, int pointZ, float height);
		public abstract void SetColor(int pointX, int pointZ, Color color);
		public abstract void SetNormal(int pointX, int pointZ, Vector3 normal);

	}
}

