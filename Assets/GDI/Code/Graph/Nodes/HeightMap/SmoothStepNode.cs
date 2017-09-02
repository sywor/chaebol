using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Smooth Step")]
	public class SmoothStepNode : AbstractNumberNode
	{

		private InputSocket _inputSocketFrom;
		private InputSocket _inputSocketTo;
		private InputSocket _inputSocketT;

		private Rect _tmpRect;

		public SmoothStepNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketT = 	new InputSocket(this, typeof(INumberConnection));
			_inputSocketFrom = 	new InputSocket(this, typeof(INumberConnection));
			_inputSocketTo =	new InputSocket(this, typeof(INumberConnection));

			Sockets.Add(_inputSocketT);
			Sockets.Add(_inputSocketFrom);
			Sockets.Add(_inputSocketTo);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));

			Width = 80;
			Height = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "t");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "from");
			_tmpRect.Set(3, 40, 50, 20);
			GUI.Label(_tmpRect, "to");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float from = GetInputNumber(_inputSocketFrom, request);
			float to = GetInputNumber(_inputSocketTo, request);
			float t = GetInputNumber(_inputSocketT, request);
			return Mathf.SmoothStep(from, to, t);
		}
	}
}
