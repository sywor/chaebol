using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Number")]
	public class NumberNode : AbstractNumberNode
	{

		[SerializeField] private float _number;
		private string _numberString = "0";

		public NumberNode(int id, Graph parent) : base(id, parent)
		{
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 43;
			Width = 58;
		}

		protected override void OnGUI()
		{

			if (NodeUtils.FloatTextField(new Rect(3, 0, 50, 20), ref _numberString))
			{
				_number = NodeUtils.Parse(_numberString);
				TriggerChangeEvent();
			}
		}

		public override void Update()
		{
			_numberString = _number + "";
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			return _number;
		}
	}
}
