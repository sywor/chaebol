using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Abs")]
	public class AbsNode : AbstractNumberNode
	{
		private InputSocket _inputSocket;

		public AbsNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 40;
			Height = 40;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var number = GetInputNumber(_inputSocket, request);
			if (float.IsNaN(number)) return float.NaN;
			return Mathf.Abs(number);
		}
	}
}