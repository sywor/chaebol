using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools.Thread;
using Assets.GDI.Code.Tools.Thread.TextureJobs;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Unity Perlin Noise")]
	/*[NodeHelpText("The <b>UnityPerlinNoise</b> node creates 2D noise values\n"
				+ "based on the <i>Mathf.PerlinNoise</i> function.\n"
				+ "You might not want to request noise for x < 0 or z < 0\n"
				+ "because the noise values are mirrored there.\n \n"

	 			+ "<b><color=#0000ffff>Output:</color></b>\n"
	 			+ "A float value between [-1, 1].\n \n"

				+ "<b><color=#005500ff>Input:</color></b>\n"
	 			+ "<b>scale:</b> The scale of the noise. (scale > 1 || scale < -1)\n"
				+ "<b>seed:</b> An offset for x and z.\n \n"

	 			+ "<b><color=#800080ff>Internal Request:</color></b>\n"
	 			+ "Uses <i>x</i> and <i>z</i> as function parameters.\n"
	 			+ "Uses <i>seed</i> to modify the seed input.")]*/
	public class UnityPerlinNoiseNode : AbstractNoiseNode
	{

		[NonSerialized] private Rect _labelScale;
		[NonSerialized] private Rect _labelSeed;

		[NonSerialized] private InputSocket _inputSocketScale;
		[NonSerialized] private InputSocket _inputSocketSeed;

		public UnityPerlinNoiseNode(int id, Graph parent) : base(id, parent)
		{
			_labelScale = new Rect(6, 0, 30, Config.SocketSize);
			_labelSeed = new Rect(6, 20, 30, Config.SocketSize);
			_inputSocketScale = new InputSocket(this, typeof(INumberConnection));
			_inputSocketSeed = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			_inputSocketScale.SetDirectInputNumber(5, false);

			Sockets.Add(_inputSocketScale);
			Sockets.Add(_inputSocketSeed);

			//Height = CurrentTextureSize + 70;
			_textures.Add(new GUIThreadedTexture()); // heightmap
			Height = 60;
			Width = 110;
		}

		protected override void OnGUI()
		{
			if (!_textures[0].DoneInitialUpdate) Update();

			_textures[0].X = 40;

			GUI.Label(_labelScale, "scale");
			GUI.Label(_labelSeed, "seed");
			DrawTextures();
		}


		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var scale = GetInputNumber(_inputSocketScale, request);
			var socketSeed = GetInputNumber(_inputSocketSeed, request);

			if (float.IsNaN(scale) || float.IsNaN(socketSeed)) return float.NaN;

			if (scale == 0) scale = 1;

			var modifiedSeed = NodeUtils.ModifySeed(socketSeed, request.Seed);

			float noise = Mathf.PerlinNoise(request.X / scale + modifiedSeed, request.Z / scale + modifiedSeed) * 2f - 1f;
			return Mathf.Max(Mathf.Min(noise, 1), -1);
		}

		public override void Update()
		{
			HeightMapUpdateJob job = new HeightMapUpdateJob();
			job.Request(0, 0, 55, 35, this, null);
			if (!Collapsed) _textures[0].StartTextureUpdateJob(55, 35, job);
		}
	}
}
