using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Nodes.HeightMap;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Mask
{

	[Serializable]
	[GraphContextMenuItem("Mask", "Four Biomes")]
	public class FourBiomesMaskNode : AbstractNumberNode
	{
		private InputSocket _inputSocketNoise;
		private InputSocket _inputSocketSeed;

		private OutputSocket _outputSocketCombined;
		private OutputSocket _outputSocketBiome01;
		private OutputSocket _outputSocketBiome02;
		private OutputSocket _outputSocketBiome03;
		private OutputSocket _outputSocketBiome04;

		private Rect _tmpRect;

		public FourBiomesMaskNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketNoise = new InputSocket(this, typeof(INumberConnection));
			_inputSocketSeed = new InputSocket(this, typeof(INumberConnection));

			_outputSocketCombined = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketBiome01 = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketBiome02 = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketBiome03 = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketBiome04 = new OutputSocket(this, typeof(INumberConnection));

			Sockets.Add(_inputSocketNoise);
			Sockets.Add(_inputSocketSeed);

			Sockets.Add(_outputSocketCombined);
			Sockets.Add(_outputSocketBiome01);
			Sockets.Add(_outputSocketBiome02);
			Sockets.Add(_outputSocketBiome03);
			Sockets.Add(_outputSocketBiome04);

			Height = 120;
			Width = 145;
		}

		protected override void OnGUI()
		{
			_tmpRect.Set(0, 0, 50, 20);
			GUI.Label(_tmpRect, "noise");
			_tmpRect.Set(0, 20, 80, 20);
			GUI.Label(_tmpRect, "seed");

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 63, 0, 60, 20);
			GUI.Label(_tmpRect, "combined");
			_tmpRect.Set(Width - 63, 20, 60, 20);
			GUI.Label(_tmpRect, "biome 1");
			_tmpRect.Set(Width - 63, 40, 60, 20);
			GUI.Label(_tmpRect, "biome 2");
			_tmpRect.Set(Width - 63, 60, 60, 20);
			GUI.Label(_tmpRect, "biome 3");
			_tmpRect.Set(Width - 63, 80, 60, 20);
			GUI.Label(_tmpRect, "biome 4");
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float noise01 = GetInputNumber(_inputSocketNoise, request);
			if (float.IsNaN(noise01)) return float.NaN;

			float seed = GetInputNumber(_inputSocketSeed, request);
			Request r = new Request(request.X, request.Y, request.Z, request.SizeX, request.SizeY,
				request.SizeZ, NodeUtils.ModifySeed(request.Seed, seed));

			//Request r = new Request(request.X, request.Y, request.Z, request.SizeX, request.SizeY,
			//	request.SizeZ, 68431);

			float noise02 = GetInputNumber(_inputSocketNoise, r);
			if (float.IsNaN(noise02)) return float.NaN;


			// sharpen
			noise01 = (float) Math.Tanh(noise01 * 6);
			noise02 = (float) Math.Tanh(noise02 * 6);
			// step
			noise01 = GradientStepNode.Calc(noise01, 2, 0);
			noise02 = GradientStepNode.Calc(noise02, 2, 0);

			float combined = (noise01 + noise02 * 2f) / 3f;

			if (outSocket == _outputSocketCombined) return combined;

			if (outSocket == _outputSocketBiome01) return Calc(combined, -1.0000001f, -0.5f);
			if (outSocket == _outputSocketBiome02) return Calc(combined, -0.5f, 0);
			if (outSocket == _outputSocketBiome03) return Calc(combined, 0, 0.5f);
			if (outSocket == _outputSocketBiome04) return Calc(combined, 0.5f, 1.0000001f);

			return 1;
		}

		private static float Calc(float noise, float min, float max)
		{
			if (noise >= min && noise < max) return 1;
			return -1;
		}

	}
}