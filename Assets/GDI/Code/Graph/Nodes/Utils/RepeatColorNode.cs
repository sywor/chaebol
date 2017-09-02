using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Utils
{
	[Serializable]
	[GraphContextMenuItem("Utils", "Repeat Color")]
	public class RepeatColorNode : AbstractColorNode
	{

		private InputSocket _inputSocket;

		public RepeatColorNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(_inputSocket);
			Sockets.Add(new OutputSocket(this, typeof(IColorConnection)));
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

		public override UnityEngine.Color GetColor(OutputSocket socket, Request request)
		{
			return GetInputColor(_inputSocket, request);
		}
	}
}
