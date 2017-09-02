using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Border")]
	public class BorderNode : AbstractNumberNode
	{
		[SerializeField]
		private bool _useDistance = true;

		private InputSocket _inputSocketNoise;
		private InputSocket _inputSocketWidth;
		private InputSocket _inputSocketBottom;
		private InputSocket _inputSocketTop;

		private Rect _tmpRect;

		public BorderNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketNoise = new InputSocket(this, typeof(INumberConnection));
			_inputSocketWidth = new InputSocket(this, typeof(INumberConnection));
			_inputSocketBottom = new InputSocket(this, typeof(INumberConnection));
			_inputSocketTop = new InputSocket(this, typeof(INumberConnection));

			_inputSocketWidth.SetDirectInputNumber(3, false);
			_inputSocketTop.SetDirectInputNumber(0.02f, false);
			_inputSocketBottom.SetDirectInputNumber(-0.02f, false);

			Sockets.Add(_inputSocketNoise);
			Sockets.Add(_inputSocketWidth);
			Sockets.Add(_inputSocketTop);
			Sockets.Add(_inputSocketBottom);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 55, 20);
			GUI.Label(_tmpRect, "noise");

			_tmpRect.Set(3, 20, 55, 20);
			GUI.Label(_tmpRect, "width");

			_tmpRect.Set(3, 40, 55, 20);
			GUI.Label(_tmpRect, "top");

			_tmpRect.Set(3, 60, 55, 20);
			GUI.Label(_tmpRect, "bottom");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			_tmpRect.Set(55, 0, 55, 20);
			var currentDist = GUI.Toggle(_tmpRect, _useDistance, "dist.");
			if (currentDist != _useDistance)
			{
				_useDistance = currentDist;
				TriggerChangeEvent();
			}
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			Request r = new Request();
			r.X = request.X * 2;
			r.Y = request.Y;
			r.Z = request.Z * 2;
			r.Seed = request.Seed;
			float radius = GetInputNumber(_inputSocketWidth, request);
			float bottom = GetInputNumber(_inputSocketBottom, request);
			float top = GetInputNumber(_inputSocketTop, request);

			if (float.IsNaN(radius) || float.IsNaN(bottom) || float.IsNaN(top)) return float.NaN;

			radius = radius / 2f;

			if (bottom > top)
			{
				float t = top;
				float b = bottom;
				top = b;
				bottom = t;
			}

			float v = float.NaN;

			Request r2 = new Request();
			for (var xi = -radius; xi <= radius; xi++)
			{
				for (var zi = -radius; zi <= radius; zi++)
				{
					float distance = Mathf.Sqrt(xi * xi + zi * zi);
					if (distance <= radius)
					{
						r2.X = request.X + xi;
						r2.Y = request.Y;
						r2.Z = request.Z + zi;
						r2.Seed = request.Seed;
						float noiseValue = GetInputNumber(_inputSocketNoise, request);
						if (!float.IsNaN(noiseValue) && noiseValue >= bottom && noiseValue <= top)
						{
							if (!_useDistance) return 1;
							else
							{
								float d = (distance / radius);
								if (float.IsNaN(v))
								{
									v = d;
								}
								if (d < v)
								{
									v = d;
								}
							}
						}
					}
				}
			}

			if (float.IsNaN(v) || !_useDistance) return -1;

			return ((v * 2f) - 1f) * -1f;
		}
	}
}