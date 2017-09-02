using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "And")]
	public class AndNode : AbstractNumberNode
	{
		private InputSocket _inputSocket01;
		private InputSocket _inputSocket02;

		public AndNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket01 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket01);
			_inputSocket02 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket02);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 40;
			Height = 60;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var number01 = GetInputNumber(_inputSocket01, request);
			var number02 = GetInputNumber(_inputSocket02, request);
			if (float.IsNaN(number01) || float.IsNaN(number02)) return float.NaN;
			if (number01 > -1 && number02 > -1) return (number01 + number02) / 2;
			return -1;
		}
	}
}