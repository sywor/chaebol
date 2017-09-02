using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Color")]
	public class ColorNode : AbstractColorNode
	{
		[SerializeField] private UnityEngine.Color _color;

		[NonSerialized] private InputSocket _inputSocketR;
		[NonSerialized] private InputSocket _inputSocketG;
		[NonSerialized] private InputSocket _inputSocketB;
		[NonSerialized] private InputSocket _inputSocketA;

		[NonSerialized] private Rect _labelR;
		[NonSerialized] private Rect _labelG;
		[NonSerialized] private Rect _labelB;
		[NonSerialized] private Rect _labelA;

		[NonSerialized] private Rect _sliderR;
		[NonSerialized] private Rect _sliderG;
		[NonSerialized] private Rect _sliderB;
		[NonSerialized] private Rect _sliderA;

		public ColorNode(int id, Graph parent) : base(id, parent)
		{
			_labelR = new Rect(3, 0, 20, 20);
			_labelG = new Rect(3, 20, 20, 20);
			_labelB = new Rect(3, 40, 20, 20);
			_labelA = new Rect(3, 60, 20, 20);

			_inputSocketR = new InputSocket(this, typeof(INumberConnection));
			_inputSocketG = new InputSocket(this, typeof(INumberConnection));
			_inputSocketB = new InputSocket(this, typeof(INumberConnection));
			_inputSocketA = new InputSocket(this, typeof(INumberConnection));

			_inputSocketA.SetDirectInputNumber(1, false);

			Sockets.Add(_inputSocketR);
			Sockets.Add(_inputSocketG);
			Sockets.Add(_inputSocketB);
			Sockets.Add(_inputSocketA);

			Sockets.Add(new OutputSocket(this, typeof(IColorConnection)));
			Width = 150;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			var r = _color.r;
			var g = _color.g;
			var b = _color.b;
			var a = _color.a;

			_sliderR = new Rect(18, 0, Width - 45, 20);
			_sliderG = new Rect(18, 20, Width - 45, 20);
			_sliderB = new Rect(18, 40, Width - 45, 20);
			_sliderA = new Rect(18, 60, Width - 45, 20);

			bool wasMouseUp = Event.current.type == EventType.MouseUp;

			GUI.Label(_labelR, "r");
			GUI.Label(_labelG, "g");
			GUI.Label(_labelB, "b");
			GUI.Label(_labelA, "a");

			r = GUI.HorizontalSlider(_sliderR, (float) Math.Round(r, 3), 0f, 1f);
			g = GUI.HorizontalSlider(_sliderG, (float) Math.Round(g, 3), 0f, 1f);
			b = GUI.HorizontalSlider(_sliderB, (float) Math.Round(b, 3), 0f, 1f);
			a = GUI.HorizontalSlider(_sliderA, (float) Math.Round(a, 3), 0f, 1f);

			var rChanged = UpdateDirectInput(_inputSocketR, _color.r, r);
			var gChanged = UpdateDirectInput(_inputSocketG, _color.g, g);
			var bChanged = UpdateDirectInput(_inputSocketB, _color.b, b);
			var aChanged = UpdateDirectInput(_inputSocketA, _color.a, a);

			if (rChanged || gChanged || bChanged || aChanged)
			{
				SetColor(r, g, b, a);
			}

			if (wasMouseUp && Event.current.type == EventType.Used) TriggerChangeEvent();

			NodeUtils.GUIDrawRect(new Rect(Width - 22, 0, 20, Height - 25), _color);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		private bool UpdateDirectInput(InputSocket socket, float oldValue, float newValue)
		{
			if (oldValue != newValue && socket.IsInDirectInputMode())
			{
				socket.SetDirectInputNumber(newValue, false);
				return true;
			}
			return false;
		}


		private void UpdateDirectInputs()
		{
			if (_inputSocketR.IsInDirectInputMode()) _color.r = _inputSocketR.GetDirectInputNumber();
			if (_inputSocketG.IsInDirectInputMode()) _color.g = _inputSocketG.GetDirectInputNumber();
			if (_inputSocketB.IsInDirectInputMode()) _color.b = _inputSocketB.GetDirectInputNumber();
			if (_inputSocketA.IsInDirectInputMode()) _color.a = _inputSocketA.GetDirectInputNumber();
		}

		private void SetColor(float r, float g, float b, float a)
		{
			_color.r = r;
			_color.g = g;
			_color.b = b;
			_color.a = a;
		}

		public override void Update()
		{
			UpdateDirectInputs();
		}

		private void UpdateColor(Request request) {
			_color.r = AbstractNumberNode.GetInputNumber(_inputSocketR, request);
			_color.g = AbstractNumberNode.GetInputNumber(_inputSocketG, request);
			_color.b = AbstractNumberNode.GetInputNumber(_inputSocketB, request);
			_color.a = AbstractNumberNode.GetInputNumber(_inputSocketA, request);
		}


		public override UnityEngine.Color GetColor(OutputSocket outSocket, Request request)
		{
			UpdateColor(request);
			return _color;
		}
	}
}
