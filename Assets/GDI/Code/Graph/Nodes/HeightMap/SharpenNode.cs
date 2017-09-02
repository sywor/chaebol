using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Sharpen")]
	/*[NodeHelpText("The <b>Sharpen</b> node can be used to sharpen noise values.\n"
	              + "It uses <i>Math.Tanh(input * intensity)</i> for it.\n \n"
	              + "<b><color=#0000ffff>Output:</color></b>\n"
	              + "\n"
	              + "\n \n "
	              + "<b><color=#005500ff>Input:</color></b>\n"
	              + "<b>noise:</b> A float value to sharpen.\n"
	              + "<b>intensity:</b> The intesity to sharpen.\n \n"
	              + "<b><color=#800080ff>Internal Request:</color></b>\n"
	              + "Bypasses the request parameters to the inputs.")]*/
	public class SharpenNode : AbstractNumberNode
	{

		private InputSocket _inputSocketNoise;
		private InputSocket _inputSocketIntensity;
		private InputSocket _inputSocketThreshold;
		private Rect _tmpRect;

		public SharpenNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketNoise = new InputSocket(this, typeof(INumberConnection));
			_inputSocketIntensity = new InputSocket(this, typeof(INumberConnection));
			_inputSocketThreshold = new InputSocket(this, typeof(INumberConnection));
			_inputSocketIntensity.SetDirectInputNumber(1, false);
			_inputSocketThreshold.SetDirectInputNumber(0.01f, false);
			Sockets.Add(_inputSocketNoise);
			Sockets.Add(_inputSocketIntensity);
			Sockets.Add(_inputSocketThreshold);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			_tmpRect = new Rect();
			Height = 80;
			Width = 80;


		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 40, 20);
			GUI.Label(_tmpRect, "noise");
			_tmpRect.Set(3, 20, 60, 20);
			GUI.Label(_tmpRect, "instensity");
			_tmpRect.Set(3, 40, 60, 20);
			GUI.Label(_tmpRect, "threshold");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;


		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float noise = GetInputNumber(_inputSocketNoise, request);
			float intensity = GetInputNumber(_inputSocketIntensity, request);
			float threshold = GetInputNumber(_inputSocketThreshold, request);
			if (float.IsNaN(noise) || float.IsNaN(intensity) || float.IsNaN(threshold)) return float.NaN;
			float value = (float) Math.Tanh(noise * intensity);
			if (value > 1 - threshold) value = 1;
			if (value < -1 + threshold) value = -1;
			return value;
		}
	}
}
