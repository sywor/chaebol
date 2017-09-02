using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Circle")]
	public class CircleNode : AbstractNumberNode
	{

		private InputSocket _inputSocketRadius;
		private InputSocket _inputSocketGradientWidth;
		private InputSocket _inputSocketGradientExponent;

		private OutputSocket _outputSocketValue;
		private OutputSocket _outputSocketSize;

		private Rect _tmpRect;

		public CircleNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketRadius = new InputSocket(this, typeof(INumberConnection));
			_inputSocketRadius.SetDirectInputNumber(30, false);
			_inputSocketGradientWidth = new InputSocket(this, typeof(INumberConnection));
			_inputSocketGradientWidth.SetDirectInputNumber(20, false);
			_inputSocketGradientExponent = new InputSocket(this, typeof(INumberConnection));
			_inputSocketGradientExponent.SetDirectInputNumber(0.4f, false);

			Sockets.Add(_inputSocketRadius);
			Sockets.Add(_inputSocketGradientWidth);
			Sockets.Add(_inputSocketGradientExponent);

			_outputSocketValue = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketSize = new OutputSocket(this, typeof(INumberConnection));

			Sockets.Add(_outputSocketValue);
			Sockets.Add(_outputSocketSize);

			Width = 180;
			Height = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 110, 20);
			GUI.Label(_tmpRect, "radius");
			_tmpRect.Set(3, 20, 110, 20);
			GUI.Label(_tmpRect, "gradient width");
			_tmpRect.Set(3, 40, 110, 20);
			GUI.Label(_tmpRect, "gradient exponent");
			GUI.skin.label.alignment = TextAnchor.MiddleRight;

			_tmpRect.Set(Width - 63, 0, 60, 20);
			GUI.Label(_tmpRect, "value");
			_tmpRect.Set(Width - 63, 20, 60, 20);
			GUI.Label(_tmpRect, "diameter");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float radius = GetInputNumber(_inputSocketRadius, request);
			float gradientWidth = GetInputNumber(_inputSocketGradientWidth, request);

			if (outSocket == _outputSocketValue || outSocket == null)
			{

				float gradientExponent = GetInputNumber(_inputSocketGradientExponent, request);

				if (float.IsNaN(radius) || float.IsNaN(gradientWidth) || float.IsNaN(gradientExponent)) return float.NaN;

				float ix = request.X - radius - gradientWidth;
				float iz = request.Z - radius - gradientWidth;

				float dist = Mathf.Sqrt(ix * ix + iz * iz);

				if (dist <= radius) return 1f;

				if (dist <= radius + gradientWidth && dist > radius)
				{
					float g = (radius + gradientWidth - dist) / gradientWidth;

					return (Mathf.Pow(g, gradientExponent) - 0.5f) * 2f;
				}

			}

			if (outSocket == _outputSocketSize)
			{
				return radius * 2 + gradientWidth * 2;
			}

			return float.NaN;
		}
	}
}
