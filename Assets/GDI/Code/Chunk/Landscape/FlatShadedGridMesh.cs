using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Assets.Code.Chunk
{
	/*[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class FlatShadedGridMesh : AbstractGridMesh
	{

		private Vector3[] _vertices;
		private int[] _indices;
		private Vector2[] _uvs;
		private Vector3[] _normals;
		private Color[] _colors;
		private Mesh _mesh;

		private int _cols;
		private int _rows;

		private int _rowVertexCount;

		private Vector3 _gizmoV01 = new Vector3();
		private Vector3 _gizmoV02 = new Vector3();

		private int[] _tmpPositionArray = new int[6];

		public bool GizmoHighlight = false;

		public override void PrepareGrid(int cols, int rows)
		{
			_cols = cols;
			_rows = rows;
			_rowVertexCount = _cols*6;

			_mesh = new Mesh { name = "Grid" };
			GetComponent<MeshFilter>().mesh = _mesh;

			_vertices = new Vector3[(_rowVertexCount * _rows)];
			_indices = new int[_vertices.Length];
			_uvs = new Vector2[_vertices.Length];
			_normals = new Vector3[_vertices.Length];
			_colors = new Color[_vertices.Length];

			for (var y = 0; y < _rows; y++)
			{
				for (var x = 0; x < _cols; x++)
				{
					CreateRectangle(x, y);
				}
			}

			_mesh.vertices = _vertices;
			_mesh.triangles = _indices;
			_mesh.uv = _uvs;
			_mesh.normals = _normals;
			_mesh.colors = _colors;

		}


		private void CreateRectangle(int rectangleX, int rectangleZ)
		{
			var vertexOffset = GetVertexOffset(rectangleX, rectangleZ);

			var x = -(_cols/2f) + rectangleX;
			var z = -(_rows/2f) + rectangleZ;

			// triangle 1
			_vertices[vertexOffset] = 		new Vector3(x, 		0, 	z);
			_vertices[vertexOffset + 1] = 	new Vector3(x, 		0, 	z + 1);
			_vertices[vertexOffset + 2] = 	new Vector3(x +1, 	0, 	z + 1);

			_indices[vertexOffset] = vertexOffset;
			_indices[vertexOffset + 1] = vertexOffset + 1;
			_indices[vertexOffset + 2] = vertexOffset + 2;

			_normals[vertexOffset] = 	 new Vector3(0, 0, 1);
			_normals[vertexOffset + 1] = new Vector3(0, 0, 1);
			_normals[vertexOffset + 2] = new Vector3(0, 0, 1);

			_colors[vertexOffset] = 	 new Color();
			_colors[vertexOffset + 1] =  new Color();
			_colors[vertexOffset + 2] =  new Color();


			// triangle 2
			_vertices[vertexOffset + 3] = new Vector3(x, 		0, 	z);
			_vertices[vertexOffset + 4] = new Vector3(x + 1, 	0,  z + 1);
			_vertices[vertexOffset + 5] = new Vector3(x + 1, 	0,  z);

			_indices[vertexOffset + 3] = vertexOffset + 3;
			_indices[vertexOffset + 4] = vertexOffset + 4;
			_indices[vertexOffset + 5] = vertexOffset + 5;

			_normals[vertexOffset + 3] = new Vector3(0, 0, 1);
			_normals[vertexOffset + 4] = new Vector3(0, 0, 1);
			_normals[vertexOffset + 5] = new Vector3(0, 0, 1);

			_colors[vertexOffset + 3] =  new Color();
			_colors[vertexOffset + 4] =  new Color();
			_colors[vertexOffset + 5] =  new Color();
		}


		private int GetVertexOffset(int rectangleX, int rectangleY)
		{
			return rectangleY * _rowVertexCount + rectangleX * 6;
		}


		//
		//Triangle indices:
		//|---------|---------|
        //|       / |       / |
        //|     /   | (0) /   |
        //|   /     |   /     |
   		//| /   (5) | /   (1) |
		//|---------P---------|
        //|       / |       / |
        //| (4) /   | (2) /   |
        //|   /     |   /     |
   		//| /  (3)  | /       |
		//|---------|---------|

		//Gets the vertex index of the triangle
		//sahring the points coordinates.


		private int GetVertexIndex(int pointX, int pointZ, int triangleIndex)
		{
			if (pointX < 0 || pointZ < 0 || pointX > _cols || pointZ > _rows) return -1;
			if (triangleIndex == 0)
			{
				if (pointX == _cols || pointZ == _rows) return -1;
				return GetVertexOffset(pointX, pointZ);
			}
			if (triangleIndex == 1)
			{
				if (pointX == _cols || pointZ == _rows) return -1;
				return GetVertexOffset(pointX, pointZ) + 3;
			}
			if (triangleIndex == 2)
			{
				if (pointX == _cols || pointZ == 0) return -1;
				return GetVertexOffset(pointX, pointZ - 1) + 1;
			}
			if (triangleIndex == 3)
			{
				if (pointX == 0 || pointZ == 0) return -1;
				return GetVertexOffset(pointX - 1, pointZ - 1) + 4;
			}
			if (triangleIndex == 4)
			{
				if (pointX == 0 || pointZ == 0) return -1;
				return GetVertexOffset(pointX - 1, pointZ - 1) + 2;
			}
			if (triangleIndex == 5)
			{
				if (pointX == 0 || pointZ == _rows) return -1;
				return GetVertexOffset(pointX - 1, pointZ) + 5;
			}
			return -1;
		}

		private int[] GetVertexIndices(int pointX, int pointZ, ref int[] indices)
		{
			indices[0] = GetVertexIndex(pointX, pointZ, 0);
			indices[1] = GetVertexIndex(pointX, pointZ, 1);

			var tmp = GetVertexIndex(pointX, pointZ, 2);
			if (tmp != -1) indices[2] = tmp;
			else indices[2] = -1;

			tmp = GetVertexIndex(pointX, pointZ, 3);
			if (tmp != -1) indices[3] = tmp;
			else indices[3] = -1;

			tmp = GetVertexIndex(pointX, pointZ, 4);
			if (tmp != -1) indices[4] = tmp;
			else indices[4] = -1;

			tmp = GetVertexIndex(pointX, pointZ, 5);
			if (tmp != -1) indices[5] = tmp;
			else indices[5] = -1;
			return indices;
		}

		private Vector3 GetPosition(int pointX, int pointZ)
		{
			GetVertexIndices(pointX, pointZ, ref _tmpPositionArray);
			for (var i = 0; i < _tmpPositionArray.Length; i++)
			{
				if (_tmpPositionArray[i] != -1)
				{
					return _vertices[_tmpPositionArray[i]];
				}
			}
			return Vector3.zero;
		}

		public override void SetHeight(int pointX, int pointZ, float height)
		{
			var vertex00 = GetVertexIndex(pointX, pointZ, 0);
			var vertex01 = GetVertexIndex(pointX, pointZ, 1);
			var vertex02 = GetVertexIndex(pointX, pointZ, 2);
			var vertex03 = GetVertexIndex(pointX, pointZ, 3);
			var vertex04 = GetVertexIndex(pointX, pointZ, 4);
			var vertex05 = GetVertexIndex(pointX, pointZ, 5);
			if (IsVertexIndexInBounds(vertex00)) _vertices[vertex00].y = height;
			if (IsVertexIndexInBounds(vertex01)) _vertices[vertex01].y = height;
			if (IsVertexIndexInBounds(vertex02)) _vertices[vertex02].y = height;
			if (IsVertexIndexInBounds(vertex03)) _vertices[vertex03].y = height;
			if (IsVertexIndexInBounds(vertex04)) _vertices[vertex04].y = height;
			if (IsVertexIndexInBounds(vertex05)) _vertices[vertex05].y = height;
		}

		public override void SetColor(int pointX, int pointZ, Color color)
		{
			var vertex00 = GetVertexIndex(pointX, pointZ, 0);
			var vertex01 = GetVertexIndex(pointX, pointZ, 1);
			var vertex02 = GetVertexIndex(pointX, pointZ, 2);
			var vertex03 = GetVertexIndex(pointX, pointZ, 3);
			var vertex04 = GetVertexIndex(pointX, pointZ, 4);
			var vertex05 = GetVertexIndex(pointX, pointZ, 5);
			if (IsVertexIndexInBounds(vertex00)) _colors[vertex00] = color;
			if (IsVertexIndexInBounds(vertex01)) _colors[vertex01] = color;
			if (IsVertexIndexInBounds(vertex02)) _colors[vertex02] = color;
			if (IsVertexIndexInBounds(vertex03)) _colors[vertex03] = color;
			if (IsVertexIndexInBounds(vertex04)) _colors[vertex04] = color;
			if (IsVertexIndexInBounds(vertex05)) _colors[vertex05] = color;
		}

		public override void SetNormal(int pointX, int pointZ, Vector3 normal)
		{
			throw new System.NotImplementedException();
		}


		public override void Update()
		{
			_mesh.vertices = _vertices;
			_mesh.colors = _colors;
			_mesh.RecalculateNormals();
			_mesh.RecalculateBounds();
		}

		public override void UpdateNormals()
		{
			throw new System.NotImplementedException();
		}

		private bool IsVertexIndexInBounds(int vertexIndex)
		{
			return (vertexIndex >= 0) && vertexIndex < _vertices.Length;
		}


		void OnDrawGizmos() {



			for (var x = 0; x < _cols; x++)
			{
				Gizmos.color = Color.black;

				DrawGizmoEdge(x, 0, x + 1, 0, 0, 0, 0);
				DrawGizmoEdge(x, _rows, x + 1, _rows, 0, 0, 0);

				if (GizmoHighlight)
				{
					Gizmos.color = Color.blue;
					DrawGizmoEdge(x, 0, x + 1, 0, 0, 0.2f, 0.1f);
					DrawGizmoEdge(x, _rows, x + 1, _rows, 0, 0.2f, -0.1f);
				}
			}



			for (var z = 0; z < _cols; z++)
			{
				Gizmos.color = Color.black;

				DrawGizmoEdge(0, z, 0, z + 1, 0, 0, 0);
				DrawGizmoEdge(_cols, z, _cols, z + 1, 0, 0, 0);

				if (GizmoHighlight)
				{
					Gizmos.color = Color.blue;
					DrawGizmoEdge(0, z, 0, z + 1, 0.1f, 0.2f, 0);
					DrawGizmoEdge(_cols, z, _cols, z + 1, -0.1f, 0.2f, 0);
				}
			}
		}



		void DrawGizmoEdge(int x1, int z1, int x2, int z2, float offsetX, float offsetY, float offsetZ)
		{
			var xpos01 = x1 + transform.position.x - _cols / 2f;
			var xpos02 = x2 + transform.position.x - _cols / 2f;
			var zpos01 = z1 + transform.position.z - _rows / 2f;
			var zpos02 = z2 + transform.position.z - _rows / 2f;

			var ypos01 = GetPosition(x1, z1).y + transform.position.y;
			var ypos02 = GetPosition(x2, z2).y + transform.position.y;
			_gizmoV01.Set(xpos01 + offsetX, ypos01 + offsetY, zpos01 + offsetZ);
			_gizmoV02.Set(xpos02 + offsetX, ypos02 + offsetY, zpos02 + offsetZ);
			Gizmos.DrawLine(_gizmoV01, _gizmoV02);
		}

	}*/
}
