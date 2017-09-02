using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Number
{
	[Serializable]
	[GraphContextMenuItem("Number", "Mix")]
	public class MixNode : AbstractNumberNode
	{
		[NonSerialized] private Rect labelInput01;
		[NonSerialized] private Rect labelInput02;
		[NonSerialized] private Rect labelFactor;

		[NonSerialized] private InputSocket _inputSocket01;
		[NonSerialized] private InputSocket _inputSocket02;
		[NonSerialized] private InputSocket _factorSocket;

		public MixNode(int id, Assets.GDI.Code.Graph.Graph parent) : base(id, parent)
		{
			labelInput01 = new Rect(3, 0, 100, 20);
			labelInput02 = new Rect(3, 20, 100, 20);
			labelFactor = new Rect(3, 40, 100, 20);

			_inputSocket01 = new InputSocket(this, typeof(INumberConnection));
			_inputSocket02 = new InputSocket(this, typeof(INumberConnection));
			_factorSocket = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Sockets.Add(_inputSocket01);
			Sockets.Add(_inputSocket02);
			Sockets.Add(_factorSocket);
			Height = 80;
			Width = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(labelInput01, "in 1");
			GUI.Label(labelInput02, "in 2");
			GUI.Label(labelFactor, "factor (-1 to 1)");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public static float Clamp(float value, float min, float max)
		{
			return value < min ? min : value > max ? max : value;
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var factorValue = (GetInputNumber(_factorSocket, request) + 1) / 2;
			if (float.IsNaN(factorValue)) return float.NaN;

			// to avoid calc of obsolete values here..
			if (factorValue <= -1) return GetInputNumber(_inputSocket01, request);
			if (factorValue >= 1) return GetInputNumber(_inputSocket02, request);

			float v1 = GetInputNumber(_inputSocket01, request);
			float v2 = GetInputNumber(_inputSocket02, request);

			if (float.IsNaN(v1) || float.IsNaN(v2)) return float.NaN;

			v1 = Clamp(v1, -1, 1);
			v2 = Clamp(v2, -1, 1);


			return v1 * (1 - factorValue) + v2 * factorValue;
		}
	}
}
