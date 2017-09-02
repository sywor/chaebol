using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Pow")]
	public class PowNode : AbstractNumberNode
	{

		private InputSocket _valueSocket01;
		private InputSocket _valueSocket02;

		public PowNode(int id, Graph parent) : base(id, parent)
		{
			_valueSocket01 = new InputSocket(this, typeof(INumberConnection));
			_valueSocket02 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_valueSocket01);
			Sockets.Add(_valueSocket02);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 50;
			Height = 60;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float v1 = GetInputNumber(_valueSocket01, request);
			float v2 = GetInputNumber(_valueSocket02, request);
			if (float.IsNaN(v1) || float.IsNaN(v2)) return float.NaN;
			return Mathf.Pow(v1, v2);
		}
	}
}