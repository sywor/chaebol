using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "ReplaceNaN")]
	public class ReplaceNanNode : AbstractNumberNode
	{

		private InputSocket _inputSocketNumber;
		private InputSocket _inputSocketReplacement;

		private Rect _tmpRect;

		public ReplaceNanNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketNumber = new InputSocket(this, typeof(INumberConnection));
			_inputSocketReplacement = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketNumber);
			Sockets.Add(_inputSocketReplacement);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 80;
			Height = 60;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "in");

			_tmpRect.Set(3, 20, 70, 20);
			GUI.Label(_tmpRect, "replacement.");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}


		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float input = GetInputNumber(_inputSocketNumber, request);
			if (float.IsNaN(input))
			{
				input = GetInputNumber(_inputSocketReplacement, request);
			}
			return input;
		}
	}
}
