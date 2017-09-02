using System;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Utils
{
	[Serializable]
	[GraphContextMenuItem("Utils", "Label")]
	public class LabelNode : Node
	{

		[SerializeField] private string _text = "TEXT";
		[SerializeField] private GUIStyle _labelStyle;
		[SerializeField] private int _fontSize = 20;

		private Rect _tmpRect;

		public LabelNode(int id, Graph parent) : base(id, parent)
		{
			CustomStyle = true;
			CustomBackgroundColor = UnityEngine.Color.white;
		}

		protected override void OnGUI()
		{
			_labelStyle = new GUIStyle();
			_labelStyle.fontSize = _fontSize;

			Vector2 size = _labelStyle.CalcSize(new GUIContent(_text));
			Width = size.x + 20;
			Height = size.y + 40;
			_tmpRect.Set(10, 0, size.x, size.y);

			if (Selected)
			{
				_text = GUI.TextField(_tmpRect, _text, _labelStyle);
				_tmpRect.Set(10, size.y, 20, 15);
				if (GUI.Button(_tmpRect, "+"))
				{
					_labelStyle.fontSize += 5;
					_fontSize = _labelStyle.fontSize;
				}
				_tmpRect.Set(30, size.y, 20, 15);
				if (GUI.Button(_tmpRect, "-"))
				{
					if (_labelStyle.fontSize > 10) _labelStyle.fontSize -= 5;
					_fontSize = _labelStyle.fontSize;
				}
			}
			else
			{
				GUI.Label(_tmpRect, _text, _labelStyle);
			}
		}

		public override void Update()
		{

		}
	}
}
