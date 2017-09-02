using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools.Thread;
using Assets.GDI.Code.Tools.Thread.TextureJobs;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.HeightMap
{
	[Serializable]
	[GraphContextMenuItem("HeightMap", "Sine")]
	public class SineMapNode : AbstractNoiseNode {


		[NonSerialized] private Rect _textLabelScaleX;
		[NonSerialized] private Rect _textLabelScaleZ;

		[NonSerialized] private InputSocket _socketInputX;
		[NonSerialized] private InputSocket _socketInputZ;

		public SineMapNode(int id, Graph parent) : base(id, parent)
		{
			_textLabelScaleX = new Rect(6, 0, 65, Config.SocketSize);
			_textLabelScaleZ = new Rect(6, 20, 65, Config.SocketSize);
			_socketInputX = new InputSocket(this, typeof(INumberConnection));
			_socketInputX.SetDirectInputNumber(5, false);
			Sockets.Add(_socketInputX);
			_socketInputZ = new InputSocket(this, typeof(INumberConnection));
			_socketInputZ.SetDirectInputNumber(5, false);
			Sockets.Add(_socketInputZ);

			Height = 60;
			_textures.Add(new GUIThreadedTexture()); // heightmap
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
		}

		protected override void OnGUI()
		{
			if (!_textures[0].DoneInitialUpdate) Update();

			_textures[0].X = 48;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(_textLabelScaleX, "scale x");
			GUI.Label(_textLabelScaleZ, "scale z");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			DrawTextures();
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			var scaleX = GetInputNumber(_socketInputX, request);
			var scaleZ = GetInputNumber(_socketInputZ, request);

			if (float.IsNaN(scaleX) || float.IsNaN(scaleZ)) return float.NaN;

			if (scaleX == 0) scaleX = 1;
			if (scaleZ == 0) scaleZ = 1;

			return (Mathf.Sin(request.X / scaleX + request.Seed) + Mathf.Sin(request.Z / scaleZ + request.Seed)) / 2f;
		}

		public override void Update()
		{
			HeightMapUpdateJob job = new HeightMapUpdateJob();
			job.Request(0, 0, 45, 35, this, null);
			if (!Collapsed) _textures[0].StartTextureUpdateJob(45, 35, job);
		}
	}
}
