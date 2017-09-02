using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.ColorMap
{
	[Serializable]
	[GraphContextMenuItem("ColorMap", "To Pixel")]
	public class ColorMapFilterNode : AbstractColorNode, INumberConnection
	{
		private UnityEngine.Color[,] _colorMap;

		private InputSocket _inputSocketColorMap;
		private OutputSocket _outputSocketWidth;
		private OutputSocket _outputSocketHeight;

		private Rect _tmpRect;

		public ColorMapFilterNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColorMap = new InputSocket(this, typeof(IColorMapConnection));
			var outputSocketColor = new OutputSocket(this, typeof(IColorConnection));
			_outputSocketWidth = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketHeight = new OutputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketColorMap);
			Sockets.Add(outputSocketColor);
			Sockets.Add(_outputSocketWidth);
			Sockets.Add(_outputSocketHeight);
			Width = 120;
			Height = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 100, 20);
			GUI.Label(_tmpRect, "color map");

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 80, 0, 80, 20);
			GUI.Label(_tmpRect, "color");

			_tmpRect.Set(Width - 80, 20, 80, 20);
			GUI.Label(_tmpRect, "width");

			_tmpRect.Set(Width - 80, 40, 80, 20);
			GUI.Label(_tmpRect, "height");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{
			_colorMap = AbstractColorMapNode.GetInputColorMap(_inputSocketColorMap, new Request());
		}

		public override UnityEngine.Color GetColor(OutputSocket socket, Request request)
		{
			if (_colorMap != null)
			{
				int width = _colorMap.GetLength(0);
				int height = _colorMap.GetLength(1);

				int iX = (int) request.X;
				int iZ = (int) request.Z;

				if (iX < width && iZ < height)
				{
					return _colorMap[iX, iZ];
				}
			}
			return UnityEngine.Color.magenta;
		}

		public float GetNumber(OutputSocket outSocket, Request request)
		{
			if (_colorMap != null)
			{
				if (outSocket == _outputSocketWidth) return _colorMap.GetLength(0);
				if (outSocket == _outputSocketHeight) return _colorMap.GetLength(1);
			}
			return float.NaN;
		}
	}
}
