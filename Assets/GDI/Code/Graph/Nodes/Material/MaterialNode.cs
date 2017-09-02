using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Material
{

	[Serializable]
	[GraphContextMenuItem("Material", "Material")]
	public class MaterialNode : AbstractMaterialNode
	{

		private UnityEngine.Material _material;
		[SerializeField] private string _assetPath;
		private bool initialLoadingDone = false;
		private Rect _tmpRect;

		private InputSocket _inputSocketColor;

		public MaterialNode(int id, Graph parent) : base(id, parent)
		{
			Width = 160;
			Height = 80;
			Sockets.Add(new OutputSocket(this, typeof(IMaterialConnection)));

			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(_inputSocketColor);
			SocketTopOffsetInput = 28;
		}

		protected override void OnGUI()
		{
			LoadMaterial();
			DrawMaterialField();
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 25, 100, 20);
			GUI.Label(_tmpRect, "color");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		}

		private void DrawMaterialField()
		{
			if (_material == null)
			{
				_tmpRect.Set(2, 2, 152, 22);
				EditorGUI.DrawRect(_tmpRect, UnityEngine.Color.magenta);
			}
			_tmpRect.Set(3, 3, 150, 20);
			UnityEngine.Material newMaterial = (UnityEngine.Material) EditorGUI.ObjectField(_tmpRect, "", _material, typeof(UnityEngine.Material), false);
			if (_material != newMaterial)
			{
				_material = newMaterial;
				_assetPath = AssetDatabase.GetAssetPath(_material);
				TriggerChangeEvent();
			}
		}

		private void LoadMaterial()
		{
			if (_material == null && !string.IsNullOrEmpty(_assetPath) && !initialLoadingDone)
			{
				_material = (UnityEngine.Material) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(UnityEngine.Material));
				if (_material == null)
				{
					Log.Error("Can not find material at " + _assetPath);
				}

			}
		}

		public override void Update()
		{
			LoadMaterial();
		}

		public override UnityEngine.Material GetMaterial(OutputSocket outSocket, Request request)
		{
			if (_inputSocketColor.CanGetResult())
			{
				UnityEngine.Color c = AbstractColorNode.GetInputColor(_inputSocketColor, request);
				_material.color = c;
			}

			return _material;
		}
	}
}