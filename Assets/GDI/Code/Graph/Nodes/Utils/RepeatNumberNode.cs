using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Utils
{
	[Serializable]
	[GraphContextMenuItem("Utils", "Repeat Number")]
	public class RepeatNumberNode : AbstractNumberNode
	{

		private InputSocket _inputSocket;

		public RepeatNumberNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Name = "";
			Height = 40;
		}

		protected override void OnGUI()
		{
			Width = GUI.skin.label.CalcSize(new GUIContent(Name)).x + 10;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			return GetInputNumber(_inputSocket, request);
		}
	}
}
