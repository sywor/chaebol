using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Mix")]
	public class MixColorNode : AbstractColorNode
	{
		private InputSocket _inputSocketColor01;
		private InputSocket _inputSocketColor02;

		private InputSocket _inputSocketValue;

		private OutputSocket _outputSocket;

		public MixColorNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColor01 = new InputSocket(this, typeof(IColorConnection));
			_inputSocketColor02 = new InputSocket(this, typeof(IColorConnection));
			_inputSocketValue = new InputSocket(this, typeof(INumberConnection));

			_outputSocket = new OutputSocket(this, typeof(IColorConnection));

			Sockets.Add(_inputSocketColor01);
			Sockets.Add(_inputSocketColor02);
			Sockets.Add(_inputSocketValue);

			Sockets.Add(_outputSocket);
			Width = 50;
			Height = 80;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override UnityEngine.Color GetColor(OutputSocket socket, Request request)
		{
			UnityEngine.Color color01 = GetInputColor(_inputSocketColor01, request);
			UnityEngine.Color color02 = GetInputColor(_inputSocketColor02, request);
			float value = AbstractNumberNode.GetInputNumber(_inputSocketValue, request);
			if (float.IsNaN(value)) return UnityEngine.Color.magenta;
			value = (value + 1) / 2;
			UnityEngine.Color color = color01 * value + color02 * (1 - value);
			return color;

		}
	}
}
