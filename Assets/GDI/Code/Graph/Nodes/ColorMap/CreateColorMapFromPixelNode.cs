using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.ColorMap
{
	[Serializable]
	[GraphContextMenuItem("ColorMap", "Create From Pixel")]
	public class CreateColorMapFromPixelNode : AbstractColorMapNode
	{

		private UnityEngine.Color[,] _colorMap;

		private OutputSocket _outputSocketColorMap;

		private InputSocket _inputSocketColor;
		private InputSocket _inputSocketWidth;
		private InputSocket _inputSocketHeight;

		private Rect _tmpRect;

		public CreateColorMapFromPixelNode(int id, Assets.GDI.Code.Graph.Graph parent) : base(id, parent)
		{
			_outputSocketColorMap = new OutputSocket(this, typeof(IColorMapConnection));
			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			_inputSocketWidth = new InputSocket(this, typeof(INumberConnection));
			_inputSocketHeight = new InputSocket(this, typeof(INumberConnection));
			_inputSocketWidth.SetDirectInputNumber(10, false);
			_inputSocketHeight.SetDirectInputNumber(10, false);

			Sockets.Add(_outputSocketColorMap);
			Sockets.Add(_inputSocketColor);
			Sockets.Add(_inputSocketWidth);
			Sockets.Add(_inputSocketHeight);
			Width = 120;
			Height = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 80, 20);
			GUI.Label(_tmpRect, "color");
			_tmpRect.Set(3, 20, 80, 20);
			GUI.Label(_tmpRect, "width");
			_tmpRect.Set(3, 40, 80, 20);
			GUI.Label(_tmpRect, "height");

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 80, 0 , 80, 20);
			GUI.Label(_tmpRect, "color map");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		}

		public override void Update()
		{

			float widthF = AbstractNumberNode.GetInputNumber(_inputSocketWidth, new Request());
			float heightF = AbstractNumberNode.GetInputNumber(_inputSocketHeight, new Request());

			_colorMap = null;

			if (!float.IsNaN(widthF) && !float.IsNaN(heightF))
			{
				int widthI = (int) widthF;
				int heightI = (int) heightF;

				Request request = new Request();

				if (widthI > 0 && heightI > 0)
				{
					_colorMap = new UnityEngine.Color[widthI, heightI];

					for (int x = 0; x < widthI; x++)
					{
						for (int z = 0; z < heightI; z++)
						{
							request.X = x;
							request.Z = z;
							UnityEngine.Color c = AbstractColorNode.GetInputColor(_inputSocketColor, request);
							_colorMap[x, z] = c;
						}
					}
				}
			}
		}

		public override UnityEngine.Color[,] GetColorMap(OutputSocket socket, Request request)
		{
			return _colorMap;
		}
	}
}
