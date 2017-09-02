using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Color
{
	[Serializable]
	[GraphContextMenuItem("Color", "Gradient")]
	public class GradientNode : AbstractColorNode
	{

		[SerializeField] private List<float> _times = new List<float>();
		[SerializeField] private InputSocket _inputSocketRangeScale;

		[NonSerialized] private Rect _tmpRect;
		[NonSerialized] private Rect _addColorButton;

		[NonSerialized] private  List<InputSocket> _inputSockets;
		[NonSerialized] private Gradient _gradient;
		[NonSerialized] private Texture2D _previewTexture;
		[NonSerialized] private bool _needsUpdate = true;

		private UnityEngine.Color _nanColor = UnityEngine.Color.magenta;

		public GradientNode(int id, Graph parent) : base(id, parent)
		{
			_inputSockets = new List<InputSocket>();
			_tmpRect = new Rect();
			_addColorButton = new Rect();
			_gradient = new Gradient();
			_inputSocketRangeScale = new InputSocket(this, typeof(INumberConnection));
			_inputSocketRangeScale.SetDirectInputNumber(1, false);
			Sockets.Add(_inputSocketRangeScale);
			Sockets.Add(new OutputSocket(this, typeof(IColorConnection)));
			Width = 200;
		}

		public override void OnDeserialization(SerializableNode sNode)
		{
			var aditionalSocketsCount = _times.Count;
			while (Sockets.Count <= aditionalSocketsCount) AddInputSocket(false);
		}

		private void AddInputSocket(bool addTimes)
		{
			InputSocket s = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(s);
			_inputSockets.Add(s);
			if (addTimes) _times.Add(0);
			_needsUpdate = true;
		}

		private void RemoveInputSocket(int i)
		{
			Sockets.Remove(_inputSockets[i]);
			_inputSockets.Remove(_inputSockets[i]);
			_times.RemoveAt(i);
			_needsUpdate = true;
		}

		protected override void OnGUI()
		{
			Height = _times.Count * 20 + 65;

			GUI.Label(new Rect(3, 0, 90, 20), "range scale");

			_addColorButton.Set(28, Height - 42, 65, 18);
			if (_inputSockets.Count < 8 && GUI.Button(_addColorButton, "add color") )
			{
				AddInputSocket(true);
				TriggerChangeEvent();
			}

			var socketToRemove = -1;

			bool wasMouseUp = Event.current.type == EventType.MouseUp;

			for (var i = 0; i < _inputSockets.Count; i++)
			{
				_tmpRect.Set(25, 20 * i + 20, Width - 60, 20);

				var value = _times[i];
				_times[i] = GUI.HorizontalSlider(_tmpRect, value, 0f, 1f);
				if (value != _times[i])
				{
					UpdateColorPreview();
				}
				_tmpRect.Set(3, 20 * i + 18, 18, 18);
				if (!_inputSockets[i].IsConnected())
				{
					if (GUI.Button(_tmpRect, "x")) socketToRemove = i;
				}
				else
				{
					EditorGUI.DrawRect(_tmpRect, GetInputColor(_inputSockets[i], new Request()));
				}
			}

			if (wasMouseUp && Event.current.type == EventType.Used)
			{
				_needsUpdate = true;
				TriggerChangeEvent();
			}

			if (_previewTexture != null)
			{
				GUI.DrawTexture(new Rect(Width - _previewTexture.width - 6, 0, _previewTexture.width, _previewTexture.height), _previewTexture);
			}

			if (socketToRemove != -1)
			{
				RemoveInputSocket(socketToRemove);
				TriggerChangeEvent();
			}

			if (_needsUpdate) UpdateColorPreview();
		}

		public void UpdateColorPreview()
		{
			_needsUpdate = false;
			UpdateGradient(0, 0, 0, 0);
			if (_previewTexture != null) Texture2D.DestroyImmediate(_previewTexture);
			_previewTexture = new Texture2D(20, (int) Height - 24);

			for (int y = 0; y < _previewTexture.height; y++)
			{
				UnityEngine.Color c = _gradient.Evaluate(y / (float) _previewTexture.height);
				for (int x = 0; x < _previewTexture.width; x++)
				{
					_previewTexture.SetPixel(x, y, c);
				}
			}
			_previewTexture.Apply();
		}

		private void UpdateGradient(float x, float y, float z, float seed)
		{
			GradientColorKey[] colorKeys = new GradientColorKey[_inputSockets.Count];
			GradientAlphaKey[] alphaKeys = new GradientAlphaKey[_inputSockets.Count];
			Request request = new Request();
			request.X = x;
			request.Y = y;
			request.Z = z;
			request.Seed = seed;
			for (int i = 0; i < _inputSockets.Count; i++)
			{
				UnityEngine.Color c = UnityEngine.Color.black;
				if (_inputSockets[i].IsConnected())
				{
					AbstractColorNode colorNode = (AbstractColorNode) _inputSockets[i].GetConnectedSocket().Parent;
					c = colorNode.GetColor(_inputSockets[i].GetConnectedSocket(), request);
				}
				colorKeys[i] = new GradientColorKey();
				colorKeys[i].color = c;
				colorKeys[i].time = _times[i];
				alphaKeys[i] = new GradientAlphaKey();
				alphaKeys[i].alpha = c.a;
				alphaKeys[i].time = _times[i];
			}
			_gradient.SetKeys(colorKeys, alphaKeys);
		}

		public override void Update()
		{
			if (!Collapsed)
			UpdateColorPreview();
		}

		public override UnityEngine.Color GetColor(OutputSocket outSocket, Request request)
		{
			float rangeScale = AbstractNumberNode.GetInputNumber(_inputSocketRangeScale, request);
			if (float.IsNaN(request.Y)) return _nanColor;
			UnityEngine.Color color = _gradient.Evaluate((request.Y * rangeScale + 1) / 2f);
			return color;
		}


	}
}
