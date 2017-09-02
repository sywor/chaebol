using System;
using Assets.GDI.Code.Chunk.Landscape;
using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Nodes.Chunk;
using UnityEngine;

namespace Assets.GDI.Code.Chunk.Jobs
{
	public class LandscapeJob : AbstractChunkJob
	{

		private float[,] _heightValues;
		private Color[,] _colorValues;

		private LandscapeNode _node;
		private int _sizeX;
		private int _sizeZ;

		public const int OverlapBorderWidth = 1;

		private int _heightValuesSizeX;
		private int _heightValuesSizeZ;

		private int _requestOffsetX;
		private int _requestOffsetZ;

		private float _heightMultipicator = 15f;

		private ILandscapeCreatedListener _listener;
		private AbstractGridMesh _grid;

		public LandscapeJob(Chunk chunk, LandscapeNode node, ILandscapeCreatedListener listener) : base(chunk, node.Id)
		{
			_chunk = chunk;
			_listener = listener;
			_node = node;
			_sizeX = chunk.SizeX;
			_sizeZ = chunk.SizeZ;
			_heightValuesSizeX = _sizeX + 1 + OverlapBorderWidth * 2;
			_heightValuesSizeZ = _sizeZ + 1 + OverlapBorderWidth * 2;
			_requestOffsetX = chunk.X * _sizeX;
			_requestOffsetZ = chunk.Z * _sizeZ;
		}

		public override void Start()
		{
			_grid = _chunk.CreateChild("Landscape " + _node.Id).AddComponent<SmoothShadedGridMesh>();
			base.Start();
		}

		protected override void ThreadFunction()
		{
			Request requestHeightFactor = new Request();
			requestHeightFactor.Seed = _chunk.Seed;
			_heightMultipicator = _node.GetHeightFactor(requestHeightFactor);
			_grid.PrepareGrid(_sizeX, _sizeZ);
			_heightValues = new float[_heightValuesSizeX, _heightValuesSizeZ];
			_colorValues = new Color[_heightValuesSizeX, _heightValuesSizeZ];
			try
			{
				Request requestNumber = new Request();
				Request requestColor = new Request();
				for (var z = 0; z < _heightValuesSizeZ; z++)
				{
					for (var x = 0; x < _heightValuesSizeX; x++)
					{
						int requestX = x + _requestOffsetX - OverlapBorderWidth;
						int requestZ = z + _requestOffsetZ - OverlapBorderWidth;
						requestNumber.X = requestX;
						requestNumber.Z = requestZ;
						requestNumber.Seed = _chunk.Seed;
						float height = _node.GetNumber(null, requestNumber);
						if (float.IsNaN(height)) height = -1;
						_heightValues[x, z] = height;
						requestColor.X = x + _requestOffsetX;
						requestColor.Y = height;
						requestColor.Z = z + _requestOffsetZ;
						requestColor.Seed = _chunk.Seed;
						Color color = _node.GetColor(null, requestColor);
						_colorValues[x, z] = color;
					}
				}

			}
			catch (Exception e)
			{
				Log.Error("Exception " + e.Message + " " + e.StackTrace);
			}
		}

		protected override void OnFinished()
		{
			var request = new Request();
			request.Seed = _chunk.Seed;
			Material m = _node.GetMaterial(null, request);
			_grid.GetComponent<MeshRenderer>().sharedMaterial = m;

			if (_grid == null) return;

			for (var z = 0; z <= _sizeZ; z++)
			{
				for (var x = 0; x <= _sizeX; x++)
				{
					int requestX = x + OverlapBorderWidth;
					int requestZ = z + OverlapBorderWidth;
					float height = _heightValues[requestX, requestZ];
					_grid.SetHeight(x, z, height * _heightMultipicator);
					Color color = _colorValues[requestX, requestZ];
					_grid.SetColor(x, z, color);
				}
			}
			_grid.Create();
			SmoothEdges(_grid);
			_grid.ApplyNormals();

			_listener.OnLandscapeCreated(_grid, _node.Id);
		}

		/// <summary>Smoothes the edges of a grid. Uses neigbour values to create the normals for the edges</summary>
		private void SmoothEdges(AbstractGridMesh grid)
		{
			// bottom edge
			for (var x = 0; x <= _sizeX; x++)
			{
				grid.SetNormal(x, 0, CalcEdgeNormal(x + OverlapBorderWidth, OverlapBorderWidth));
			}

			// top edge
			for (var x = 0; x <= _sizeX; x++)
			{
				grid.SetNormal(x, _sizeZ, CalcEdgeNormal(x + OverlapBorderWidth, _sizeZ + OverlapBorderWidth));
			}

			// left edge
			for (var z = 0; z <= _sizeZ; z++)
			{
				grid.SetNormal(0, z, CalcEdgeNormal(OverlapBorderWidth, z + OverlapBorderWidth));
			}

			// right edge
			for (var z = 0; z <= _sizeZ; z++)
			{
				grid.SetNormal(_sizeX, z, CalcEdgeNormal(_sizeX + OverlapBorderWidth, z + OverlapBorderWidth));
			}
		}

		/*

			Triangle indices:            Height vectors:
			|---------|---------|       (6)-------(7)-------(8)
			|       / |       / |        |       / |       / |
			|     /   | (0) /   |        |     /   |    /    |
			|   /     |   /     |        |   /     |   /     |
			| /   (5) | /   (1) |        | /       | /       |
			|---------P---------|       (3)----- P(4)-------(5)
			|       / |       / |        |       / |       / |
			| (4) /   | (2) /   |        |     /   |     /   |
			|   /     |   /     |        |   /     |   /     |
			| /  (3)  | /       |        | /       | /       |
			|---------|---------|       (0)-------(1)--------(2)

			p = (requestX, requestZ)
		*/
		private Vector3 CalcEdgeNormal(int requestX, int requestZ)
		{
			Vector3 heightVector0 = CreateVector(requestX - 1, requestZ - 1);
			Vector3 heightVector1 = CreateVector(requestX, requestZ - 1);
			//Vector3 heightVector2 = CreateVector(requestX + 1, requestZ - 1);
			Vector3 heightVector3 = CreateVector(requestX - 1, requestZ);
			Vector3 heightVector4 = CreateVector(requestX, requestZ);
			Vector3 heightVector5 = CreateVector(requestX + 1, requestZ);
			//Vector3 heightVector6 = CreateVector(requestX - 1, requestZ + 1);
			Vector3 heightVector7 = CreateVector(requestX, requestZ + 1);
			Vector3 heightVector8 = CreateVector(requestX + 1, requestZ + 1);

			Vector3 normal0 = CalcNormal(heightVector4, heightVector7, heightVector8);
			Vector3 normal1 = CalcNormal(heightVector4, heightVector8, heightVector5);
			Vector3 normal2 = CalcNormal(heightVector4, heightVector5, heightVector1);
			Vector3 normal3 = CalcNormal(heightVector4, heightVector1, heightVector0);
			Vector3 normal4 = CalcNormal(heightVector4, heightVector0, heightVector3);
			Vector3 normal5 = CalcNormal(heightVector4, heightVector3, heightVector7);
			return (normal0 + normal1 + normal2 + normal3 + normal4 + normal5) / 6f;
		}

		private Vector3 CreateVector(int requestX, int requestZ)
		{
			return new Vector3(requestX - OverlapBorderWidth, _heightValues[requestX, requestZ] * _heightMultipicator, requestZ - OverlapBorderWidth);
		}

		public static Vector3 CalcNormal(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 side1 = b - a;
			Vector3 side2 = c - a;
			Vector3 perp = Vector3.Cross(side1, side2);
			float perpLength = perp.magnitude;
			return perp / perpLength;
		}
	}
}
