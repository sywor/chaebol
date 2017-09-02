using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Heightmap Scatter")]
	public class HeightMapScatterNode : AbstractNumberNode
	{

		private InputSocket _inputSocketValue;
		private InputSocket _inputSocketWidth;
		private InputSocket _inputSocketHeight;
		private InputSocket _inputSocketVec;
		private InputSocket _inputSocketMask;

		[SerializeField] private bool _fadeMask = true;

		private Rect _tmpRect;

		public HeightMapScatterNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketValue = new InputSocket(this, typeof(INumberConnection));
			_inputSocketWidth = new InputSocket(this, typeof(INumberConnection));
			_inputSocketHeight = new InputSocket(this, typeof(INumberConnection));
			_inputSocketVec = new InputSocket(this, typeof(IVectorConnection));
			_inputSocketMask = new InputSocket(this, typeof(INumberConnection));
			_inputSocketMask.SetDirectInputNumber(1, false);

			Sockets.Add(_inputSocketValue);
			Sockets.Add(_inputSocketWidth);
			Sockets.Add(_inputSocketHeight);
			Sockets.Add(_inputSocketVec);
			Sockets.Add(_inputSocketMask);

			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));

			Width = 120;
			Height = 120;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 60, 20);
			GUI.Label(_tmpRect, "value");
			_tmpRect.Set(3, 20, 60, 20);
			GUI.Label(_tmpRect, "width");
			_tmpRect.Set(3, 40, 60, 20);
			GUI.Label(_tmpRect, "height");
			_tmpRect.Set(3, 60, 60, 20);
			GUI.Label(_tmpRect, "vec");
			_tmpRect.Set(3, 80, 60, 20);
			GUI.Label(_tmpRect, "mask");

			_tmpRect.Set(80, 80, 80, 20);
			var currentFadeMask = GUI.Toggle(_tmpRect, _fadeMask, "fade");

			if (currentFadeMask != _fadeMask)
			{
				_fadeMask = currentFadeMask;
				TriggerChangeEvent();
			}

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 63, 0, 60, 20);
			GUI.Label(_tmpRect, "value");

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{

			float mask = GetInputNumber(_inputSocketMask, request);
			if (float.IsNaN(mask)) return float.NaN;
			if (mask < 0) return float.NaN;

			int width = Mathf.FloorToInt(GetInputNumber(_inputSocketWidth, request));
			int height = Mathf.FloorToInt(GetInputNumber(_inputSocketHeight, request));

			if (float.IsNaN(width) || float.IsNaN(height) || width == 0 || height == 0) return float.NaN;

			float halfWidth = Mathf.Ceil(width / 2f);
			float halfHeight = Mathf.Ceil(height / 2f);

			Request r = new Request();
			r.X = request.X - width * 1.5f - 1;
			r.Y = request.Y;
			r.Z = request.Z - height * 1.5f - 1;
			r.SizeX = width * 3 + 1;
			r.SizeZ = height * 3 + 1;
			r.Seed = request.Seed;
			List<UnityEngine.Vector3> points = AbstractVector3Node.GetInputVector3List(_inputSocketVec, r);

			if (points == null) return float.NaN;

			float value = float.NaN;
			bool initialized = false;

			Request r2 = new Request();
			for (int index = 0; index < points.Count; index++)
			{
				var point = points[index];
				if (!_fadeMask && !AllPositionsCoveredByMask(
					    (int) (point.x - halfWidth),
					    (int) (point.z - halfHeight), width, height, request.Y, request.Seed)) continue;

				int mapRelativeX = (int) (halfWidth - (point.x - request.X));
				int mapRelativeZ = (int) (halfHeight - (point.z - request.Z));
				if (mapRelativeX < width && mapRelativeX > 0 && mapRelativeZ < height && mapRelativeZ > 0)
				{
					r2.X = mapRelativeX;
					r2.Z = mapRelativeZ;
					r2.Seed = request.Seed;
					float v = GetInputNumber(_inputSocketValue, r2);
					if (!initialized && !float.IsNaN(v))
					{
						value = 0;
						initialized = true;
					}

					if (!float.IsNaN(v)) value = v;
				}
			}

			if (_fadeMask) value = value * mask;
			return value;

		}

		private bool AllPositionsCoveredByMask(int x, int z, int width, int height, float y, float seed)
		{
			Request request = new Request();
			for (var maskX = x; maskX < x + width; maskX++)
			{
				for (var maskZ = z; maskZ < z + height; maskZ++)
				{
					request.X = maskX;
					request.Y = y;
					request.Z = maskZ;
					request.Seed = seed;
					float mask = GetInputNumber(_inputSocketMask, request);
					if (float.IsNaN(mask) || mask < 0) return false;
				}
			}
			return true;
		}
	}
}
