using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.GameObject
{
	[Serializable]
	[GraphContextMenuItem("GameObject", "Properties")]
	public class GameObjectPropertiesNode : Node, IVectorConnection
	{
		private InputSocket _inputSocketGameObject;

		private OutputSocket _outputSocketPosition;
		private OutputSocket _outputSocketRotation;
		private OutputSocket _outputSocketScale;

		private Rect _tmpRect;

		public GameObjectPropertiesNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketGameObject = new InputSocket(this, typeof(IGameObjectsConnection));
			Sockets.Add(_inputSocketGameObject);

			_outputSocketPosition = new OutputSocket(this, typeof(IVectorConnection));
			_outputSocketRotation = new OutputSocket(this, typeof(IVectorConnection));
			_outputSocketScale = new OutputSocket(this, typeof(IVectorConnection));

			Sockets.Add(_outputSocketPosition);
			Sockets.Add(_outputSocketRotation);
			Sockets.Add(_outputSocketScale);

			Height = 80;
		}

		protected override void OnGUI()
		{
			_tmpRect.Set(3, 0, 100, 20);
			GUI.Label(_tmpRect, "obj");

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(Width - 50, 0, 47, 20);
			GUI.Label(_tmpRect, "position");
			_tmpRect.Set(Width - 50, 20, 47, 20);
			GUI.Label(_tmpRect, "rotation");
			_tmpRect.Set(Width - 50, 40, 47, 20);
			GUI.Label(_tmpRect, "scale");
		}

		public override void Update()
		{

		}

		public List<UnityEngine.Vector3> GetVector3List(OutputSocket outSocket, Request request)
		{
			UnityEngine.GameObject gameObject = GameObjectNode.GetInputGameObject(_inputSocketGameObject, request);

			if (gameObject != null)
			{
				if (outSocket == _outputSocketPosition) return new List<UnityEngine.Vector3> {gameObject.transform.position};
				if (outSocket == _outputSocketScale) return new List<UnityEngine.Vector3> {gameObject.transform.localScale};
				if (outSocket == _outputSocketRotation) return new List<UnityEngine.Vector3> {gameObject.transform.localRotation.eulerAngles};
			}
			return null;
		}
	}
}
