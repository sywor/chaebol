using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Limit")]
	public class LimitNode : AbstractNumberNode {

		[SerializeField] private bool _minActive = true;
		[SerializeField] private bool _maxActive = true;

		[NonSerialized] private InputSocket _inputSocket01;
		[NonSerialized] private InputSocket _inputSocketMin;
		[NonSerialized] private InputSocket _inputSocketMax;

		[NonSerialized] private Rect _tmpRect;

		public LimitNode(int id, Assets.GDI.Code.Graph.Graph parent) : base(id, parent)
		{
			_inputSocket01 = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocket01);
			_inputSocketMin = new InputSocket(this, typeof(INumberConnection));
			_inputSocketMin.SetDirectInputNumber(1, false);
			Sockets.Add(_inputSocketMin);
			_inputSocketMax = new InputSocket(this, typeof(INumberConnection));
			_inputSocketMax.SetDirectInputNumber(-1, false);
			Sockets.Add(_inputSocketMax);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));

			_tmpRect = new Rect();
			Height = 80;
			Width = 50;
		}


		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 20, 50, 20);
			var currentMin = GUI.Toggle(_tmpRect, _minActive, "min");
			_tmpRect.Set(3, 40, 50, 20);
			var currentMax = GUI.Toggle(_tmpRect, _maxActive, "max");

			if (currentMin != _minActive || currentMax != _maxActive) TriggerChangeEvent();

			_maxActive = currentMax;
			_minActive = currentMin;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var value01 = GetInputNumber(_inputSocket01, request);

			if (float.IsNaN(value01)) return float.NaN;

			if (_minActive)
			{
				var min = GetInputNumber(_inputSocketMin, request);
				if (float.IsNaN(min)) return float.NaN;
				value01 = Mathf.Min(value01, min);
			}

			if (_maxActive)
			{
				var max = GetInputNumber(_inputSocketMax, request);
				if (float.IsNaN(max)) return float.NaN;
				value01 = Mathf.Max(value01, max);
			}

			return value01;
		}

	}
}
