using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools.Thread;
using Assets.GDI.Code.Tools.Thread.TextureJobs;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Display
{
	[Serializable]
	[GraphContextMenuItem("", "Display Profile")]
	public class DisplayProfileNode : AbstractNoiseNode
	{

		private InputSocket _inputSocketValue;
		private InputSocket _inputSocketColor;

		[SerializeField] private bool _showLine;

		[NonSerialized] private const int SizeStep = 50;
		[NonSerialized] private const int TextureLeft = 25;
		[NonSerialized] private const int TextureTop = 13;
		[NonSerialized] private const int TextureRight = 5;
		[NonSerialized] private const int TextureBottom = 20;

		private Rect _tmpRect;

		public DisplayProfileNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketValue = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketValue);
			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(_inputSocketColor);
			_textures.Add(new GUIThreadedTexture(new ProfileTextureUpdateJob()));
			Width = 440;
			Height = 150;
		}

		protected override void OnGUI()
		{
			if (!_textures[0].DoneInitialUpdate) StartTextureJob();
			NodeUtils.DrawVerticalLine(new Vector2(TextureLeft - 2, CalcTextureHeight() + TextureTop),
				new Vector2(TextureLeft - 2, TextureTop), Config.ArrowY, "y", "-1", "1");

			NodeUtils.DrawHorizontalLine(new Vector2(TextureLeft, CalcTextureHeight() + TextureTop),
				new Vector2(TextureLeft + CalcTextureWidth(), CalcTextureHeight() + TextureTop), Config.ArrowX, "x", "0", CalcTextureWidth() +"" );

			DrawTextures();

			_tmpRect.Set(100, Height - 40, 50, 20);
			_showLine = GUI.Toggle(_tmpRect, _showLine, "line");
			if (_showLine)
			{
				Handles.DrawLine(new Vector2(TextureLeft, TextureTop + CalcTextureHeight() / 2f),
					new Vector2(TextureLeft + CalcTextureWidth(), TextureTop + CalcTextureHeight() / 2f));
			}
		}

		private int CalcTextureHeight()
		{
			return (int) Height - 25 - TextureTop - TextureBottom;
		}

		private int CalcTextureWidth()
		{
			return (int) Width - 10 - TextureLeft - TextureRight;
		}

		private void StartTextureJob()
		{
			ProfileTextureUpdateJob job = new ProfileTextureUpdateJob();
			job.Request(0, 0, CalcTextureWidth(), CalcTextureHeight(),
				this, _inputSocketValue.GetConnectedSocket(),
				GetColorSampler(), _inputSocketColor.GetConnectedSocket());
			_textures[0].StartTextureUpdateJob(TextureLeft, TextureTop, CalcTextureWidth(), CalcTextureHeight(), job);
		}

		public override void Update()
		{
			if (_inputSocketValue.CanGetResult()) StartTextureJob();
			else _textures[0].Hide();
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			return GetInputNumber(_inputSocketValue, request);
		}

		private IColorConnection GetColorSampler()
		{
			if (_inputSocketColor.CanGetResult()) return (AbstractColorNode) _inputSocketColor.GetConnectedSocket().Parent;
			return null;
		}
	}
}
