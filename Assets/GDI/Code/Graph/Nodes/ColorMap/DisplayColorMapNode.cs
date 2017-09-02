using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools.Thread;
using Assets.GDI.Code.Tools.Thread.TextureJobs;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.ColorMap
{
	[Serializable]
	[GraphContextMenuItem("ColorMap", "Display")]
	public class DisplayColorMapNode : AbstractColorMapNode
	{

		private InputSocket _inputSocket;
		private GUIThreadedTexture _texture;
		private Rect _tmpRect;
		private UnityEngine.Color[,] _colorMap;

		private const int MinWidth = 100;
		private const int MinHeight = 40;

		public DisplayColorMapNode(int id, Graph parent) : base(id, parent)
		{
			_texture = new GUIThreadedTexture();
			_inputSocket = new InputSocket(this, typeof(IColorMapConnection));
			Sockets.Add(_inputSocket);
			Width = MinWidth;
			Height = MinHeight;

		}

		protected override void OnGUI()
		{
			if (!_texture.DoneInitialUpdate) UpdateJob();

			Width = Mathf.Max(_texture.Width, MinWidth) + 10;
			Height = Mathf.Max(_texture.Height, MinHeight) + 25;
			_texture.OnGUI();


			if (_texture.IsUpdating)
			{
				_tmpRect.Set(3, 3, 80, 20);
				GUI.Label(_tmpRect, "updating data..");
			}
		}

		private void UpdateJob()
		{
			if (_inputSocket.IsConnected())
			{
				ColorMapTextureJob job = new ColorMapTextureJob();
				job.Request(this, _inputSocket.GetConnectedSocket());
				_texture.StartTextureUpdateJob(MinWidth, MinHeight, job);
			}
		}

		public override void Update()
		{
			if (Collapsed) return;
			_colorMap = GetInputColorMap(_inputSocket, new Request());
			if (_colorMap != null) UpdateJob();
			else
			{
				Width = MinWidth;
				Height = MinHeight;
				_texture.Hide();
			}
		}


		public override UnityEngine.Color[,] GetColorMap(OutputSocket socket, Request request)
		{
			return _colorMap;
		}
	}
}
