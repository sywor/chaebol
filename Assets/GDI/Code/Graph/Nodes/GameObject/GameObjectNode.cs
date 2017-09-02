using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.GameObject
{

	[Serializable]
	[GraphContextMenuItem("GameObject", "GameObject")]
	public class GameObjectNode : Node, IGameObjectsConnection
	{
		private InputSocket _inputSocketPosition;
		private InputSocket _inputSocketRotation;
		private InputSocket _inputSocketScale;
		private InputSocket _inputSocketMaterial;

		[SerializeField] private string _objectIdentifier;

		private bool _initialLoadingDone;

		private UnityEngine.GameObject _gameObject;
		private Rect _tmpRect;

		public GameObjectNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketPosition = new InputSocket(this, typeof(IVectorConnection));
			_inputSocketRotation = new InputSocket(this, typeof(IVectorConnection));
			_inputSocketScale = new InputSocket(this, typeof(IVectorConnection));
			_inputSocketMaterial = new InputSocket(this, typeof(IMaterialConnection));

			Sockets.Add(_inputSocketPosition);
			Sockets.Add(_inputSocketRotation);
			Sockets.Add(_inputSocketScale);
			Sockets.Add(_inputSocketMaterial);

			Sockets.Add(new OutputSocket(this, typeof(IGameObjectsConnection)));

			SocketTopOffsetInput = 23;
			SocketTopOffsetOutput = 3;
			Height = 123;
			Width = 160;
			_tmpRect = new Rect();
		}

		protected override void OnGUI()
		{
			if (!_initialLoadingDone)
			{
				LoadGameObject(_objectIdentifier, true);
				_initialLoadingDone = true;
			}

			GUI.skin.label.alignment = TextAnchor.MiddleLeft;

			if (_gameObject == null)
			{
				_tmpRect.Set(2, 2, 152, 22);
				EditorGUI.DrawRect(_tmpRect, UnityEngine.Color.magenta);
			}

			_tmpRect.Set(3, 3, 150, 20);
			UnityEngine.GameObject newObj = (UnityEngine.GameObject) EditorGUI.ObjectField(_tmpRect, "", _gameObject, typeof(UnityEngine.GameObject), true);
			if (newObj != _gameObject)
			{
				_gameObject = newObj;

				if (_gameObject != null)
				{
					string path = AssetDatabase.GetAssetPath(_gameObject);
					if (string.IsNullOrEmpty(path))
					{
						_objectIdentifier = _gameObject.name;
					}
					else
					{
						_objectIdentifier = path;
					}
				}
				TriggerChangeEvent();
			}


			_tmpRect.Set(3, 23, 70, 20);
			GUI.Label(_tmpRect, "position");

			_tmpRect.Set(3, 43, 70, 20);
			GUI.Label(_tmpRect, "rotation");

			_tmpRect.Set(3, 63, 50, 20);
			GUI.Label(_tmpRect, "scale");

			_tmpRect.Set(3, 83, 50, 20);
			GUI.Label(_tmpRect, "material");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		}

		private void LoadGameObject(string name, bool triggerEvent)
		{
			if (!string.IsNullOrEmpty(name))
			{
				// try to load prefab
				_gameObject = (UnityEngine.GameObject) AssetDatabase.LoadAssetAtPath(name, typeof(UnityEngine.GameObject));

				if (_gameObject == null)
				{
					// try to load scene object
					_gameObject = UnityEngine.GameObject.Find(name);
					if (_gameObject == null) Log.Error("Can not find GameObject " + name);
				}
				if (triggerEvent) TriggerChangeEvent();
			}
		}

		public override void Update()
		{

		}

		public UnityEngine.GameObject GetGameObject(OutputSocket ouputSocket, Request request)
		{
			if (!_initialLoadingDone)
			{
				LoadGameObject(_objectIdentifier, false);
				_initialLoadingDone = true;
			}

			List<UnityEngine.Vector3> position = AbstractVector3Node.GetInputVector3List(_inputSocketPosition, request);

			if (position != null && position.Count > 0 && _gameObject != null)
			{
				_gameObject.transform.position = position[0];
			}

			UnityEngine.Material material = AbstractMaterialNode.GetInputMaterial(_inputSocketMaterial, request);
			if (_gameObject != null && material != null) {

				Renderer renderer = _gameObject.GetComponent<Renderer>();
				if (renderer == null)
				{
					renderer = _gameObject.AddComponent<MeshRenderer>();
				}
				renderer.material = material;
			}

			List<UnityEngine.Vector3> scale = AbstractVector3Node.GetInputVector3List(_inputSocketScale, request);
			if (scale != null && scale.Count > 0 && _gameObject != null)
			{
				_gameObject.transform.localScale = scale[0];
			}

			List<UnityEngine.Vector3> rotation = AbstractVector3Node.GetInputVector3List(_inputSocketRotation, request);
			if (rotation != null && rotation.Count > 0 && _gameObject != null)
			{
				_gameObject.transform.localRotation = Quaternion.FromToRotation(UnityEngine.Vector3.up, rotation[0]);
			}

			return _gameObject;
		}

		public static UnityEngine.GameObject GetInputGameObject(InputSocket inputSocket, Request request)
		{
			if (!inputSocket.IsConnected()) return null;
			IGameObjectsConnection sampler = inputSocket.GetConnectedSocket().Parent as IGameObjectsConnection;
			if (sampler == null) return null;
			return sampler.GetGameObject(inputSocket.GetConnectedSocket(), request);

		}
	}
}