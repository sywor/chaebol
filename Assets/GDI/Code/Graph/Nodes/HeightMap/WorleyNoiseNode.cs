using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Worley Noise")]
	public class WorleyNoiseNode : AbstractNoiseNode, IVectorConnection
	{
		private InputSocket _inputSocketScale;
		private InputSocket _inputSocketSeed;
		private InputSocket _inputSocketJitter;

		[SerializeField] private int _selectedDistanceType;
		[NonSerialized] public string[] DistanceCalc = {"Euclidean", "Manhattan", "Voronoi"};

		[SerializeField] private int _selectedDistanceIndex;
		[NonSerialized] public string[] DistanceIndex = {"First", "Second", "S-F", "Min(S-F)"};


		private static int[] _px = {0,0,1,1,1,0,-1,-1,-1};
		private static int[] _pz = {0,1,1,0,-1,-1,-1,0,1};


		private Rect _tmpRect;

		public WorleyNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketScale = new InputSocket(this, typeof(INumberConnection));
			_inputSocketScale.SetDirectInputNumber(10, false);
			_inputSocketSeed = new InputSocket(this, typeof(INumberConnection));
			_inputSocketJitter = new InputSocket(this, typeof(INumberConnection));
			_inputSocketJitter.SetDirectInputNumber(0.75f, false);

			Sockets.Add(_inputSocketScale);
			Sockets.Add(_inputSocketSeed);
			Sockets.Add(_inputSocketJitter);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Sockets.Add(new OutputSocket(this, typeof(IVectorConnection)));
			Width = 280;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "scale");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "seed");
			_tmpRect.Set(3, 40, 90, 20);
			GUI.Label(_tmpRect, "jitter factor");

			_tmpRect.Set(100, 0, 90, 60);
			int newNoiseType = GUI.SelectionGrid(_tmpRect, _selectedDistanceType, DistanceCalc, 1, "toggle");
			if (newNoiseType != _selectedDistanceType)
			{
				_selectedDistanceType = newNoiseType;
				TriggerChangeEvent();
			}

			_tmpRect.Set(200, 0, 90, 80);
			int newDistanceIndex = GUI.SelectionGrid(_tmpRect, _selectedDistanceIndex, DistanceIndex, 1, "toggle");
			if (newDistanceIndex != _selectedDistanceIndex)
			{
				_selectedDistanceIndex = newDistanceIndex;
				TriggerChangeEvent();
			}


			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}


		public float GetPointX(int pointIndexX, int pointIndexZ, float scale, float jitter)
		{
			float noiseX = FlickNoise.SimplexValue2D(pointIndexX, pointIndexZ);
			float pointXOffset = noiseX * scale * jitter;
			return pointIndexX * scale + pointXOffset;
		}

		public float GetPointZ(int pointIndexX, int pointIndexZ, float scale, float jitter)
		{
			float noiseZ = FlickNoise.SimplexValue2D(pointIndexX + 1, pointIndexZ + 1);
			float pointZOffset = noiseZ * scale * jitter;
			return pointIndexZ * scale + pointZOffset;
		}

		private float GetDistanceToPoint(int pointIndexX, int pointIndexZ, float originX, float originZ, float scale, float jitter)
		{
			float pointPositionX = GetPointX(pointIndexX, pointIndexZ, scale, jitter);
			float pointPositionZ = GetPointZ(pointIndexX, pointIndexZ, scale, jitter);

			float distance;
			if (_selectedDistanceType == 1) distance = ManhattanDistance(originX - pointPositionX, 0, originZ - pointPositionZ, scale);
			else distance = EuclideanDistance(originX - pointPositionX, 0, originZ - pointPositionZ, scale);
			return distance;
		}


		public static float EuclideanDistance(float differenceX, float differenceY, float differenceZ, float scale)
		{
			return Mathf.Sqrt(differenceX * differenceX + differenceZ * differenceZ + differenceY * differenceY) / scale;
		}

		public static float ManhattanDistance(float differenceX, float differenceY, float differenceZ, float scale)
		{
			return (Mathf.Abs(differenceX) + Mathf.Abs(differenceZ)) / scale;
		}


		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float scale = 			GetInputNumber(_inputSocketScale, request);
			float jitter = 			GetInputNumber(_inputSocketJitter, request);
			float internalSeed = 	GetInputNumber(_inputSocketSeed, request);

			jitter = Mathf.Max(0, Mathf.Min(jitter, 1));

			if (float.IsNaN(scale) || float.IsNaN(internalSeed) ||  scale == 0) return float.NaN;

			int pointX = Mathf.RoundToInt(request.X / scale);
			int pointZ = Mathf.RoundToInt(request.Z / scale);

			float d1 = 	GetDistanceToPoint(pointX + _px[0], pointZ + _pz[0], request.X, request.Z, scale, jitter);
			float d2 = 	GetDistanceToPoint(pointX + _px[1], pointZ + _pz[1], request.X, request.Z, scale, jitter);
			float d3 = 	GetDistanceToPoint(pointX + _px[2], pointZ + _pz[2], request.X, request.Z, scale, jitter);
			float d4 = 	GetDistanceToPoint(pointX + _px[3], pointZ + _pz[3], request.X, request.Z, scale, jitter);
			float d5 = 	GetDistanceToPoint(pointX + _px[4], pointZ + _pz[4], request.X, request.Z, scale, jitter);
			float d6 = 	GetDistanceToPoint(pointX + _px[5], pointZ + _pz[5], request.X, request.Z, scale, jitter);
			float d7 = 	GetDistanceToPoint(pointX + _px[6], pointZ + _pz[6], request.X, request.Z, scale, jitter);
			float d8 = 	GetDistanceToPoint(pointX + _px[7], pointZ + _pz[7], request.X, request.Z, scale, jitter);
			float d9 = 	GetDistanceToPoint(pointX + _px[8], pointZ + _pz[8], request.X, request.Z, scale, jitter);

			int firstLowestIndex = GetLowestDistanceIndex(d1, d2, d3, d4, d5, d6, d7, d8, d9);

			if (_selectedDistanceType == 2)
			{
				// voronoi
				float firstLowestPointY = FlickNoise.SimplexValue2D(pointX + _px[firstLowestIndex], pointZ + _pz[firstLowestIndex]);
				return firstLowestPointY;
			}

			float firstLowestDistance = GetValue(firstLowestIndex, d1, d2, d3, d4, d5, d6, d7, d8, d9);

			if (_selectedDistanceIndex == 0)
			{
				// first
				return Norm(firstLowestDistance);
			}

			int secondLowestIndex = GetSecondLowestDistanceIndex(firstLowestIndex, d1, d2, d3, d4, d5, d6, d7, d8, d9);
			float secondLowestDistance = GetValue(secondLowestIndex, d1, d2, d3, d4, d5, d6, d7, d8, d9);

			if (_selectedDistanceIndex == 1)
			{
				// second
				return Norm(secondLowestDistance);
			}

			float sf = secondLowestDistance - firstLowestDistance;

			if (_selectedDistanceIndex == 2)
			{
				return Norm(sf);
			}

			if (_selectedDistanceIndex == 3)
			{
				// Min(second - first);
				return Norm(Mathf.Min(sf, Mathf.Min(secondLowestDistance, firstLowestDistance)));
			}

			return Norm(firstLowestDistance);
		}

		private float Norm(float v)
		{
			return Mathf.Clamp(v, -1f, 1f) * 2f - 1f;
		}

		private int GetLowestDistanceIndex(float d1, float d2, float d3, float d4, float d5, float d6, float d7, float d8, float d9)
		{
			if (IsLowestValue(d1, d2, d3, d4, d5, d6, d7, d8, d9)) return 0;
			if (IsLowestValue(d2, d1, d3, d4, d5, d6, d7, d8, d9)) return 1;
			if (IsLowestValue(d3, d1, d2, d4, d5, d6, d7, d8, d9)) return 2;
			if (IsLowestValue(d4, d1, d2, d3, d5, d6, d7, d8, d9)) return 3;
			if (IsLowestValue(d5, d1, d2, d3, d4, d6, d7, d8, d9)) return 4;
			if (IsLowestValue(d6, d1, d2, d3, d4, d5, d7, d8, d9)) return 5;
			if (IsLowestValue(d7, d1, d2, d3, d4, d5, d6, d8, d9)) return 6;
			if (IsLowestValue(d8, d1, d2, d3, d4, d5, d6, d7, d9)) return 7;
			if (IsLowestValue(d9, d1, d2, d3, d4, d5, d6, d7, d8)) return 8;
			return 0;
		}

		private int GetSecondLowestDistanceIndex(int firstLowestIndex, float d1, float d2, float d3, float d4, float d5,
			float d6, float d7, float d8, float d9)
		{
			if (firstLowestIndex == 0) return GetLowestDistanceIndex(float.MaxValue, d2, d3, d4, d5, d6, d7, d8, d9);
			if (firstLowestIndex == 1) return GetLowestDistanceIndex(d1, float.MaxValue, d3, d4, d5, d6, d7, d8, d9);
			if (firstLowestIndex == 2) return GetLowestDistanceIndex(d1, d2, float.MaxValue, d4, d5, d6, d7, d8, d9);
			if (firstLowestIndex == 3) return GetLowestDistanceIndex(d1, d2, d3, float.MaxValue, d5, d6, d7, d8, d9);
			if (firstLowestIndex == 4) return GetLowestDistanceIndex(d1, d2, d3, d4, float.MaxValue, d6, d7, d8, d9);
			if (firstLowestIndex == 5) return GetLowestDistanceIndex(d1, d2, d3, d4, d5, float.MaxValue, d7, d8, d9);
			if (firstLowestIndex == 6) return GetLowestDistanceIndex(d1, d2, d3, d4, d5, d6, float.MaxValue, d8, d9);
			if (firstLowestIndex == 7) return GetLowestDistanceIndex(d1, d2, d3, d4, d5, d6, d7, float.MaxValue, d9);
			if (firstLowestIndex == 8) return GetLowestDistanceIndex(d1, d2, d3, d4, d5, d6, d7, d8, float.MaxValue);
			return 1;
		}

		private bool IsLowestValue(float checkFor, float v1, float v2, float v3, float v4, float v5, float v6, float v7, float v8)
		{
			return checkFor <= v1 && checkFor <= v2 && checkFor <= v3 && checkFor <= v4 && checkFor <= v5 && checkFor <= v6 &&
			       checkFor <= v7 && checkFor <= v8;
		}

		private float GetValue(int index, float d1, float d2, float d3, float d4, float d5, float d6, float d7,
								float d8, float d9)
		{
			if (index == 0) return d1;
			if (index == 1) return d2;
			if (index == 2) return d3;
			if (index == 3) return d4;
			if (index == 4) return d5;
			if (index == 5) return d6;
			if (index == 6) return d7;
			if (index == 7) return d8;
			if (index == 8) return d9;
			return float.NaN;
		}

		public List<UnityEngine.Vector3> GetVector3List(OutputSocket outSocket, Request request)
		{
			float scale = GetInputNumber(_inputSocketScale, request);
			float jitter = GetInputNumber(_inputSocketJitter, request);

			if (float.IsNaN(scale) || float.IsNaN(jitter) || scale == 0) return null;

			int startPointIndexX = Mathf.RoundToInt(request.X / scale);
			int endPointIndexX = Mathf.RoundToInt((request.X + request.SizeX) / scale);
			int startPointIndexZ = Mathf.RoundToInt(request.Z / scale);
			int endPointIndexZ = Mathf.RoundToInt((request.Z + request.SizeZ) / scale);

			List<UnityEngine.Vector3> list = null;

			for (var cX = startPointIndexX; cX <= endPointIndexX; cX++)
			{
				for (var cZ = startPointIndexZ; cZ <= endPointIndexZ; cZ++)
				{

					float pX = GetPointX(cX, cZ, scale, jitter);
					float pZ = GetPointZ(cX, cZ, scale, jitter);

					if (pX >= request.X && pX < request.X + request.SizeX && pZ >= request.Z && pZ < request.Z + request.SizeZ)
					{
						if (list == null) list = new List<UnityEngine.Vector3>();

						UnityEngine.Vector3 v = new UnityEngine.Vector3(pX, 0, pZ);
						list.Add(v);
					}
				}
			}
			return list;
		}
	}
}
