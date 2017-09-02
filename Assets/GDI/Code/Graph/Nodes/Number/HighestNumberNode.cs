using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Highest")]
	public class HighestNumberNode : AbstractNumberNode
	{

		private InputSocket _inputSocketValue01;
		private InputSocket _inputSocketValue02;

		public HighestNumberNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketValue01 = new InputSocket(this, typeof(INumberConnection));
			_inputSocketValue02 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketValue01);
			Sockets.Add(_inputSocketValue02);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 60;
			Width = 60;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float val01 = GetInputNumber(_inputSocketValue01, request);
			float val02 = GetInputNumber(_inputSocketValue02, request);
			if (float.IsNaN(val01) || float.IsNaN(val02)) return float.NaN;

			if (val01 >= val02) return val01;
			return val02;
		}
	}
}
