using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Octave")]
	public class OctaveNode : AbstractNumberNode {


		[NonSerialized] private InputSocket _inputNoiseSocket;
		[NonSerialized] private InputSocket _inputIterationSocket;
		[NonSerialized] private InputSocket _inputLacunaritySocket;
		[NonSerialized] private InputSocket _inputPersistenceSocket;

		[NonSerialized] private Rect _labelNoise;
		[NonSerialized] private Rect _labelIteration;
		[NonSerialized] private Rect _labelLacunarity;
		[NonSerialized] private Rect _labelPersistence;

		public OctaveNode(int id, Graph parent) : base(id, parent)
		{
			_labelNoise = new Rect(3, 0, 100, 20);
			_labelIteration = new Rect(3, 20, 100, 20);
			_labelLacunarity = new Rect(3, 40, 100, 20);
			_labelPersistence = new Rect(3, 60, 100, 20);

			_inputNoiseSocket = new InputSocket(this, typeof(INumberConnection));
			_inputIterationSocket = new InputSocket(this, typeof(INumberConnection));
			_inputLacunaritySocket = new InputSocket(this, typeof(INumberConnection));
			_inputPersistenceSocket = new InputSocket(this, typeof(INumberConnection));

			_inputIterationSocket.SetDirectInputNumber(4, false);
			_inputLacunaritySocket.SetDirectInputNumber(3, false);
			_inputPersistenceSocket.SetDirectInputNumber(0.2f, false);

			Sockets.Add(_inputNoiseSocket);
			Sockets.Add(_inputIterationSocket);
			Sockets.Add(_inputLacunaritySocket);
			Sockets.Add(_inputPersistenceSocket);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 100;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(_labelNoise, "noise");
			GUI.Label(_labelIteration, "iteration");
			GUI.Label(_labelLacunarity, "lacunarity");
			GUI.Label(_labelPersistence, "persistence");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocker, Request request)
		{
			var iterations = GetInputNumber(_inputIterationSocket, request);
			var lacunarity = GetInputNumber(_inputLacunaritySocket, request);
			var persistance = GetInputNumber(_inputPersistenceSocket, request);

			if (float.IsNaN(iterations) || float.IsNaN(lacunarity) || float.IsNaN(persistance)) return float.NaN;

			float noiseHeight = 0;
			var frequency = 1f;
			var amplitude = 1f;

			Request r = new Request();
			for (var i = 0; i < (int) iterations; i++)
			{
				r.X = request.X * frequency;
				r.Y = request.Y * frequency;
				r.Z = request.Z * frequency;
				r.Seed = request.Seed / (i + 1);
				var noise = GetInputNumber(_inputNoiseSocket, r) * 2 - 1;

				noise = (noise + 1f) / 2f;

				noiseHeight += noise * amplitude;
				amplitude *= persistance;
				frequency *= lacunarity;
			}
			return noiseHeight;
		}

	}
}
