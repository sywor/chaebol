using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Invert")]
	public class InvertColorNode : AbstractColorNode
	{

		private InputSocket _inputSocketColor;

		public InvertColorNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(_inputSocketColor);
			Sockets.Add(new OutputSocket(this, typeof(IColorConnection)));
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
			UnityEngine.Color color = GetInputColor(_inputSocketColor, request);
			return new UnityEngine.Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b, 1f);
		}
	}
}
