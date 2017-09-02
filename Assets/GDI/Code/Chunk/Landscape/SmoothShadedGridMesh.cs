using UnityEngine;

namespace Assets.GDI.Code.Chunk.Landscape
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SmoothShadedGridMesh : AbstractGridMesh {

		private Vector3[] _vertices;
		private int[] _indices;
		private Vector2[] _uvs;
		private Vector3[] _normals;
		private Color[] _colors;
		private Mesh _mesh;

		private int _cols;
		private int _rows;

		public override void PrepareGrid(int cols, int rows)
		{
			_cols = cols;
			_rows = rows;

			int vertexCount = (_cols + 1) * (_rows + 1);
			_vertices = new Vector3[vertexCount];
			int triangleCount = _cols * _rows * 2;
			_indices = new int[triangleCount * 3];
			_uvs = new Vector2[vertexCount];
			_normals = new Vector3[_vertices.Length];
			_colors = new Color[_vertices.Length];

			for (var z = 0; z <= _rows; z++)
			{
				for (var x = 0; x <= _cols; x++)
				{
					int vertexIndex = GetVertexIndex(x, z);
					_vertices[vertexIndex] = new Vector3(x, 0, z);

					if (z < _rows && x < cols)
					{
						_normals[vertexIndex] = new Vector3(0, 0, 1);
						_normals[vertexIndex + 1] = new Vector3(0, 0, 1);
						_normals[vertexIndex + 2] = new Vector3(0, 0, 1);

						_colors[vertexIndex] = new Color();
						_colors[vertexIndex + 1] = new Color();
						_colors[vertexIndex + 2] = new Color();
					}
				}
			}

			CreateIndices();
		}

		private int GetVertexIndex(int pointX, int pointZ)
		{
			return pointZ * _rows + pointZ + pointX;
		}

		private int GetTriangleOffset(int pointX, int pointZ)
		{
			return (pointZ * _rows + pointX) * 6;
		}

		private void CreateIndices()
		{
			for (var z = 0; z < _rows; z++)
			{
				for (var x = 0; x < _cols; x++)
				{
					int rectVertexIndex01 = GetVertexIndex(x, z);
					int rectVertexIndex02 = GetVertexIndex(x, z + 1);
					int rectVertexIndex03 = GetVertexIndex(x + 1, z + 1);
					int rectVertexIndex04 = GetVertexIndex(x + 1, z);

					int triangleOffset = GetTriangleOffset(x, z);

					_indices[triangleOffset] = rectVertexIndex01;
					_indices[triangleOffset + 1] = rectVertexIndex02;
					_indices[triangleOffset + 2] = rectVertexIndex03;

					// triangle 1
					_indices[triangleOffset + 3] = rectVertexIndex01;
					_indices[triangleOffset + 4] = rectVertexIndex03;
					_indices[triangleOffset + 5] = rectVertexIndex04;
				}
			}
		}

		public override void SetHeight(int pointX, int pointZ, float height)
		{
			_vertices[GetVertexIndex(pointX, pointZ)].y = height;
		}

		public override void SetColor(int pointX, int pointZ, Color color)
		{
			_colors[GetVertexIndex(pointX, pointZ)] = color;
		}

		public override void SetNormal(int pointX, int pointZ, Vector3 normal)
		{
			_normals[GetVertexIndex(pointX, pointZ)] = normal;
		}

		public override void Create()
		{
			_mesh = new Mesh {name = "Landscape"};
			GetComponent<MeshFilter>().mesh = _mesh;
			_mesh.vertices = _vertices;
			_mesh.triangles = _indices;
			_mesh.normals = _normals;
			_mesh.uv = _uvs;
			_mesh.colors = _colors;
			_mesh.RecalculateNormals();
			_normals = _mesh.normals;
			_mesh.RecalculateBounds();
		}

		public override void ApplyNormals()
		{
			_mesh.normals = _normals;
		}
	}
}
