using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Noise")]
	public class NoiseNode : AbstractNumberNode
	{

		[SerializeField] private int _selectedDimension = 1;
		[SerializeField] private int _selectedNoiseType = 0;
		[NonSerialized] public static string[] Dimensions = new string[] {"1D", "2D", "3D"};
		[NonSerialized] public static string[] CutDimensions = new string[] {"1D", "2D"};
		[NonSerialized] public string[] NoiseTypes = new string[] {"Value", "Perlin", "Simplex Value", "Simplex"};

		private InputSocket _inputSocketScale;
		private InputSocket _inputSocketSeed;
		//private InputSocket _inputSocketRotation;

		private Rect _tmpRect;

		//private UnityEngine.Vector3 tmpRotation;

		public NoiseNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketScale = new InputSocket(this, typeof(INumberConnection));
			_inputSocketScale.SetDirectInputNumber(1, false);
			_inputSocketSeed = new InputSocket(this, typeof(INumberConnection));
			//_inputSocketRotation = new InputSocket(this, typeof(IVectorSampler));

			Sockets.Add(_inputSocketScale);
			Sockets.Add(_inputSocketSeed);
			//Sockets.Add(_inputSocketRotation);
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 200;
			Height = 120;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "scale");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "seed");

			_tmpRect.Set(60, 3, 100, 90);
			int newNoiseType = GUI.SelectionGrid(_tmpRect, _selectedNoiseType, NoiseTypes, 1, "toggle");
			if (newNoiseType != _selectedNoiseType)
			{
				_selectedNoiseType = newNoiseType;
				if (_selectedNoiseType == 3 && _selectedDimension == 2) _selectedDimension = 1;
				TriggerChangeEvent();
			}

			int newDimension;
			if (_selectedNoiseType == 3)
			{
				_tmpRect.Set(160, 3, 50, 45);
				newDimension = GUI.SelectionGrid(_tmpRect, _selectedDimension, CutDimensions, 1, "toggle");
			}
			else
			{
				_tmpRect.Set(160, 3, 50, 70);
				newDimension = GUI.SelectionGrid(_tmpRect, _selectedDimension, Dimensions, 1, "toggle");
			}

			if (newDimension != _selectedDimension)
			{
				_selectedDimension = newDimension;
				TriggerChangeEvent();
			}
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{


		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			float scale = GetInputNumber(_inputSocketScale, request);
			float inputSeed = GetInputNumber(_inputSocketSeed, request);

			if (float.IsNaN(scale) || float.IsNaN(inputSeed)) return float.NaN;

			float s = NodeUtils.ModifySeed(request.Seed, inputSeed);

			float x = request.X + s;
			float y = request.Y + s;
			float z = request.Z + s;

			if (_selectedNoiseType == 0)
			{
				if (_selectedDimension == 0) return FlickNoise.Value1D(x / scale, true);
				if (_selectedDimension == 1) return FlickNoise.Value2D(x / scale, z / scale, true);
				if (_selectedDimension == 2) return FlickNoise.Value3D(x / scale, z / scale, y / scale, true);
			}

			if (_selectedNoiseType == 1)
			{
				if (_selectedDimension == 0) return FlickNoise.Perlin1D(x / scale, true);
				if (_selectedDimension == 1) return FlickNoise.Perlin2D(x / scale, z / scale, true);
				if (_selectedDimension == 2) return FlickNoise.Perlin3D(x / scale, z / scale, y / scale, true);
			}

			if (_selectedNoiseType == 2)
			{
				if (_selectedDimension == 0) return FlickNoise.SimplexValue1D(x / scale);
				if (_selectedDimension == 1) return FlickNoise.SimplexValue2D(x / scale, z / scale);
				if (_selectedDimension == 2) return FlickNoise.SimplexValue3D(x / scale, z / scale, y / scale);
			}

			if (_selectedNoiseType == 3)
			{
				if (_selectedDimension == 0) return FlickNoise.Simplex1D(x / scale);
				if (_selectedDimension == 1) return FlickNoise.Simplex2D(x / scale, z / scale);
			}
			return float.NaN;
		}

	}
}
