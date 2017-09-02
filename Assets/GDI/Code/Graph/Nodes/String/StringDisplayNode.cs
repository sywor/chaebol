using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.String
{
	[Serializable]
	[GraphContextMenuItem("String", "Display")]
	public class StringDisplayNode : AbstractStringNode
	{

		private InputSocket _inputSocket;
		private Rect _tmpRect;
		private string _text;

		public StringDisplayNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocket = new InputSocket(this, typeof(IStringConnection));
			Sockets.Add(_inputSocket);
			Height = 40;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			Width = Mathf.Max(GUI.skin.textField.CalcSize(new GUIContent(_text)).x + 5, 50);
			_tmpRect.Set(3, 0, Width - 6, 20);
			GUI.Label(_tmpRect, _text);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{
			_text = GetInputString(_inputSocket, new Request());
		}

		public override string GetString(OutputSocket outSocket, Request request)
		{
			return null;
		}
	}
}
