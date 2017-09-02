using System;
using System.IO;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Texture
{
	[Serializable]
	[GraphContextMenuItem("Texture", "Save File")]
	public class SaveTextureNode : Node
	{

		private InputSocket _inputSocketColorMap;

		private Rect _tmpRect;
		private UnityEngine.Color[,] colorMap;

		public SaveTextureNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketColorMap = new InputSocket(this, typeof(IColorMapConnection));
			Sockets.Add(_inputSocketColorMap);
			Height = 70;
			Width = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 80, 20);
			GUI.Label(_tmpRect, "color map");

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			if (colorMap != null)
			{
				_tmpRect.Set(3, 23, 72, 20);
				if (GUI.Button(_tmpRect, "Save"))
				{
					string path = EditorUtility.SaveFilePanel("Save Texture", Application.dataPath, "image", "png");
					if (!string.IsNullOrEmpty(path))
					{

						UnityEngine.Color[] textureColorMap = NodeUtils.ToTextureColorMap(colorMap);

						Texture2D texture2D = new Texture2D(colorMap.GetLength(0), colorMap.GetLength(1), TextureFormat.ARGB32, false);
						texture2D.SetPixels(textureColorMap);
						texture2D.Apply();
						byte[] bytes = texture2D.EncodeToPNG();
						Texture2D.DestroyImmediate(texture2D);
						File.WriteAllBytes(path, bytes);
					}
				}
			}
		}

		public override void Update()
		{
			colorMap = AbstractColorMapNode.GetInputColorMap(_inputSocketColorMap, new Request());
		}
	}
}
