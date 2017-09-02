using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Components")]
	public class ColorComponentsNode : AbstractNumberNode
	{

		private InputSocket _inputSocketColor;

		private OutputSocket _outputSocketR;
		private OutputSocket _outputSocketG;
		private OutputSocket _outputSocketB;
		private OutputSocket _outputSocketA;

		private Rect _tmpRect;

		public ColorComponentsNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			_outputSocketR = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketG = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketB = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketA = new OutputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketColor);
			Sockets.Add(_outputSocketR);
			Sockets.Add(_outputSocketG);
			Sockets.Add(_outputSocketB);
			Sockets.Add(_outputSocketA);
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 60, 20);
			GUI.Label(_tmpRect, "color");

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 20, 0, 20, 20);
			GUI.Label(_tmpRect, "r");
			_tmpRect.Set(Width - 20, 20, 20, 20);
			GUI.Label(_tmpRect, "g");
			_tmpRect.Set(Width - 20, 40, 20, 20);
			GUI.Label(_tmpRect, "b");
			_tmpRect.Set(Width - 20, 60, 20, 20);
			GUI.Label(_tmpRect, "a");

		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			UnityEngine.Color c = AbstractColorNode.GetInputColor(_inputSocketColor, request);
			if (outSocket == _outputSocketR) return c.r;
			if (outSocket == _outputSocketG) return c.g;
			if (outSocket == _outputSocketB) return c.b;
			if (outSocket == _outputSocketA) return c.a;
			return float.NaN;
		}
	}
}
