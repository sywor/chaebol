using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Texture
{
	[Serializable]
	[GraphContextMenuItem("Texture", "Properties")]
	public class TexturePropertiesNode : AbstractNumberNode, IStringConnection
	{

		private Texture2D _texture;
		private InputSocket _inputSocketTexture;
		private Rect _tmpRect;

		private OutputSocket _outputSocketWidth;
		private OutputSocket _outputSocketHeight;
		private OutputSocket _outputSocketName;
		private OutputSocket _outputSocketAniso;
		private OutputSocket _outputSocketMipMapBias;
		private OutputSocket _outputSocketMipMapCount;

		public TexturePropertiesNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketTexture = new InputSocket(this, typeof(ITextureConnection));
			Sockets.Add(_inputSocketTexture);

			_outputSocketWidth = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketHeight = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketName = new OutputSocket(this, typeof(IStringConnection));
			_outputSocketAniso = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketMipMapBias = new OutputSocket(this, typeof(INumberConnection));
			_outputSocketMipMapCount = new OutputSocket(this, typeof(INumberConnection));
			Sockets.Add(_outputSocketWidth);
			Sockets.Add(_outputSocketHeight);
			Sockets.Add(_outputSocketName);
			Sockets.Add(_outputSocketAniso);
			Sockets.Add(_outputSocketMipMapBias);
			Sockets.Add(_outputSocketMipMapCount);
			Width = 90;
			Height = 150;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 83, 0, 80, 20);
			GUI.Label(_tmpRect, "width");
			_tmpRect.Set(Width - 83, 20, 80, 20);
			GUI.Label(_tmpRect, "height");
			_tmpRect.Set(Width - 83, 40, 80, 20);
			GUI.Label(_tmpRect, "name");
			_tmpRect.Set(Width - 83, 60, 80, 20);
			GUI.Label(_tmpRect, "anisoLevel");
			_tmpRect.Set(Width - 83, 80, 80, 20);
			GUI.Label(_tmpRect, "mipMapBias");
			_tmpRect.Set(Width - 83, 100, 80, 20);
			GUI.Label(_tmpRect, "mipMapCount");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{
			_texture = AbstractTextureNode.GetInputTexture(_inputSocketTexture, new Request());
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			_texture = AbstractTextureNode.GetInputTexture(_inputSocketTexture, request);
			if (_texture != null)
			{
				if (outSocket == _outputSocketWidth) return _texture.width;
				if (outSocket == _outputSocketHeight) return _texture.height;
				if (outSocket == _outputSocketAniso) return _texture.anisoLevel;
				if (outSocket == _outputSocketMipMapBias) return _texture.mipMapBias;
				if (outSocket == _outputSocketMipMapCount) return _texture.mipmapCount;
			}
			return float.NaN;
		}

		public string GetString(OutputSocket outSocket, Request request)
		{
			if (_texture != null)
			{
				if (outSocket == _outputSocketName) return _texture.name;
			}
			return null;
		}
	}
}
