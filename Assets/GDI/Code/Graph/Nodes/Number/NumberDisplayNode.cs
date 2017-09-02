using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{

	[Serializable]
	[GraphContextMenuItem("Number", "Display")]
	public class NumberDisplayNode : AbstractNumberNode
	{
		[NonSerialized] public float Value;
		[NonSerialized] private Rect _textFieldArea;
		[NonSerialized] private InputSocket _inSocket;

		public NumberDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_textFieldArea = new Rect(10, 0, 80, Config.SocketSize);
			_inSocket = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inSocket);
			Height = 20 + Config.SocketOffsetTop;
		}

		protected override void OnGUI()
		{
			GUI.Label(_textFieldArea, Value + "");
		}

		public override void Update()
		{
			Value = GetInputNumber(_inSocket, new Request());
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			return GetInputNumber(_inSocket, request);
		}
	}
}
