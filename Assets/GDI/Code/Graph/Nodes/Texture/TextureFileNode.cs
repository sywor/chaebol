using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Texture
{
	[Serializable]
	[GraphContextMenuItem("Texture", "Texture File")]
	public class TextureFileNode : AbstractTextureNode, IColorMapConnection
	{
		[SerializeField] private string _assetPath;

		private Texture2D _texture;
		private UnityEngine.Color[,] _colorMap;

		private Rect _tmpRect;
		private bool _initialLoadingDone;
		private bool _loadingError;


		public TextureFileNode(int id, Graph parent) : base(id, parent)
		{
			Sockets.Add(new OutputSocket(this, typeof(ITextureConnection)));
			Sockets.Add(new OutputSocket(this, typeof(IColorMapConnection)));
			Width = 200;
			Height = 221;
		}

		protected override void OnGUI()
		{
			if (!_initialLoadingDone)
			{
				LoadTexture();
				LoadColorMap();
				_initialLoadingDone = true;
				TriggerChangeEvent();
			}

			DrawTextureField();

			if (_texture != null)
			{
				_tmpRect.Set(3, 200, 100, 40);
			}

		}

		private void DrawTextureField()
		{
			if (_texture == null)
			{
				_tmpRect.Set(2, 2, 194, 22);
				Height = 60;
				EditorGUI.DrawRect(_tmpRect, UnityEngine.Color.magenta);
			}
			else
			{
				_tmpRect.Set(3, 3, 194, 194);
				Height = 220;
			}

			Texture2D newTexture = (Texture2D) EditorGUI.ObjectField(_tmpRect, "", _texture, typeof(Texture2D), false);

			if (_texture != newTexture)
			{
				_texture = newTexture;
				_assetPath = AssetDatabase.GetAssetPath(_texture);
				LoadColorMap();
				TriggerChangeEvent();
			}

			if (_loadingError)
			{
				_tmpRect.Set(2, 2, 190, 22);
				EditorGUI.DrawRect(_tmpRect, UnityEngine.Color.magenta);
				_tmpRect.Set(4, 4, 186, 18);
				EditorGUI.DrawRect(_tmpRect, UnityEngine.Color.black);
				GUI.Label(_tmpRect, "Texture Not Set To Readable");
			}
		}

		private void LoadTexture()
		{
			if (_texture != null) Texture2D.DestroyImmediate(_texture);
			if (_texture == null && !string.IsNullOrEmpty(_assetPath))
			{
				_texture = (Texture2D) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(Texture2D));
			}
			if (_texture == null) Log.Error("Can not find texture at " + _assetPath);
		}

		private void LoadColorMap()
		{
			_colorMap = null;
			if (_texture != null)
			{
				try
				{
					if (_texture.width > 0 && _texture.height > 0)
					{
						_colorMap = new UnityEngine.Color[_texture.width, _texture.height];
						for (int x = 0; x < _texture.width; x++)
						{
							for (int z = 0; z < _texture.height; z++)
							{
								_colorMap[x, z] = _texture.GetPixel(x, z);
							}
						}
					}
					_loadingError = false;
				}
				catch (Exception e)
				{
					_colorMap = null;
					Log.Error("Error accessing the texture. " + e.Message + " " + e.GetType());
					_loadingError = true;
				}
			}
		}

		public override void Update()
		{

		}

		public override Texture2D GetTexture(OutputSocket outSocket, Request request)
		{
			return _texture;
		}

		public UnityEngine.Color[,] GetColorMap(OutputSocket socket, Request request)
		{
			return _colorMap;
		}
	}
}
