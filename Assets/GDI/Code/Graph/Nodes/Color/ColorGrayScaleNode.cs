using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Gray Sacle")]
	public class ColorGrayScaleNode : AbstractColorNode
	{

		private InputSocket _inputSocketColor;

		public ColorGrayScaleNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(new OutputSocket(this, typeof(IColorConnection)));
			Sockets.Add(_inputSocketColor);
			Width = 70;
			Height = 40;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override UnityEngine.Color GetColor(OutputSocket socket, Request request)
		{
			UnityEngine.Color c = GetInputColor(_inputSocketColor, request);
			return new UnityEngine.Color(c.grayscale, c.grayscale, c.grayscale, 1);
		}
	}
}
