using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.ColorMap
{
	[Serializable]
	[GraphContextMenuItem("ColorMap", "Create From Height Map")]
	public class CreateColorMapFromHeightMapNode : AbstractColorMapNode
	{

		private InputSocket _inputSocketHeightMap;
		private InputSocket _inputSocketWidth;
		private InputSocket _inputSocketHeight;
		private InputSocket _inputSocketGradient;

		private UnityEngine.Color[,] _colors;

		private Rect _tmpRect;

		public CreateColorMapFromHeightMapNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketHeightMap = new InputSocket(this, typeof(INumberConnection));
			_inputSocketWidth = new InputSocket(this, typeof(INumberConnection));
			_inputSocketWidth.SetDirectInputNumber(50, false);
			_inputSocketHeight = new InputSocket(this, typeof(INumberConnection));
			_inputSocketHeight.SetDirectInputNumber(50, false);
			_inputSocketGradient = new InputSocket(this, typeof(IColorConnection));

			Sockets.Add(_inputSocketHeightMap);
			Sockets.Add(_inputSocketWidth);
			Sockets.Add(_inputSocketHeight);
			Sockets.Add(_inputSocketGradient);

			Sockets.Add(new OutputSocket(this, typeof(IColorMapConnection)));
			Width = 170;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "value");

			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "width");

			_tmpRect.Set(3, 40, 50, 20);
			GUI.Label(_tmpRect, "height");

			_tmpRect.Set(3, 60, 50, 20);
			GUI.Label(_tmpRect, "color");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

			float width = AbstractNumberNode.GetInputNumber(_inputSocketWidth, new Request());
			float height = AbstractNumberNode.GetInputNumber(_inputSocketHeight, new Request());
			_colors = null;
			if (float.IsNaN(width) || float.IsNaN(height) || width == 0 || height == 0) return;

			_colors = new UnityEngine.Color[(int) width, (int) height];
			Request request = new Request();
			for (int x = 0; x < width; x++)
			{
				for (int z = 0; z < height; z++)
				{
					request.X = x;
					request.Z = z;
					float value = AbstractNumberNode.GetInputNumber(_inputSocketHeightMap, request);
					if (float.IsNaN(value))
					{
						_colors[x, z] = UnityEngine.Color.magenta;
					}
					else
					{

						if (_inputSocketGradient.IsConnected())
						{
							Request r = new Request();
							r.Y = value;
							UnityEngine.Color c = AbstractColorNode.GetInputColor(_inputSocketGradient, r);
							_colors[x, z] = c;
						}
						else
						{
							_colors[x, z] = UnityEngine.Color.white * (value + 1) / 2f;
							_colors[x, z].a = 1;
						}
					}
				}
			}
		}

		public override UnityEngine.Color[,] GetColorMap(OutputSocket socket, Request request)
		{
			return _colors;
		}
	}
}
