using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Condition")]
	public class ConditionNode : AbstractNumberNode
	{

		[NonSerialized] private Rect _labelInput01;
		[NonSerialized] private Rect _labelInput02;
		[NonSerialized] private Rect _labelValue01;
		[NonSerialized] private Rect _labelValue02;

		[NonSerialized] private InputSocket _inputSocket01;
		[NonSerialized] private InputSocket _inputSocket02;
		[NonSerialized] private InputSocket _inputSocketValueI;
		[NonSerialized] private InputSocket _inputSocketValueY;

		public ConditionNode(int id, Graph parent) : base(id, parent)
		{
			_labelInput01 = new Rect(3, 0, 100, 20);
			_labelInput02 = new Rect(3, 20, 100, 20);
			_labelValue01 = new Rect(3, 40, 100, 20);
			_labelValue02 = new Rect(3, 60, 100, 20);

			_inputSocket01 = new InputSocket(this, typeof(INumberConnection));
			_inputSocket02 = new InputSocket(this, typeof(INumberConnection));
			_inputSocketValueI = new InputSocket(this, typeof(INumberConnection));
			_inputSocketValueY = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket01);
			Sockets.Add(_inputSocket02);
			Sockets.Add(_inputSocketValueI);
			Sockets.Add(_inputSocketValueY);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Height = 100;
			Width = 80;
		}

		protected override void OnGUI()
		{
			GUI.Label(_labelInput01, "if (i <  j)");
			GUI.Label(_labelInput02, "if (i >= j)");
			GUI.Label(_labelValue01, "i");
			GUI.Label(_labelValue02, "j");
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var valueI = GetInputNumber(_inputSocketValueI, request);
			var valueY = GetInputNumber(_inputSocketValueY, request);
			if (float.IsNaN(valueI) || float.IsNaN(valueY)) return float.NaN;
			if (valueI < valueY) return GetInputNumber(_inputSocket01, request);
			return GetInputNumber(_inputSocket02, request);
		}
	}
}
