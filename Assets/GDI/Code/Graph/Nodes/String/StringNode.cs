using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.String
{
	[Serializable]
	[GraphContextMenuItem("String", "String")]
	public class StringNode : AbstractStringNode {

		[SerializeField] private string _text = "";
		[NonSerialized] private Rect _textFieldRect;

		public StringNode(int id, Assets.GDI.Code.Graph.Graph parent) : base(id, parent)
		{
			_textFieldRect = new Rect(3, 0, 100, 20);
			Sockets.Add(new OutputSocket(this, typeof(IStringConnection)));
			Height = 45;
			Width = 50;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			Width = Mathf.Max(GUI.skin.textField.CalcSize(new GUIContent(_text)).x + 13, 50);
			_textFieldRect.width = Width - 8;
			string newText = GUI.TextField(_textFieldRect, _text);
			if (newText != _text)
			{
				_text = newText;
				TriggerChangeEvent();
			}
			_text = newText;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override string GetString(OutputSocket outSocket, Request request)
		{
			return _text;
		}
	}
}
