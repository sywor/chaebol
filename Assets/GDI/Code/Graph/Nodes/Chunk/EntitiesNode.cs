using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Nodes.GameObject;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Chunk
{
	[Serializable]
	[GraphContextMenuItem("Chunk", "Entities")]
	public class EntitiesNode : Node, IGameObjectsConnection
	{

		private InputSocket _inputSocketGameObject;
		private InputSocket _inputSocketPositions;
		private InputSocket _inputSocketRotationX;
		private InputSocket _inputSocketRotationY;
		private InputSocket _inputSocketRotationZ;

		private InputSocket _inputSocketScaleX;
		private InputSocket _inputSocketScaleY;
		private InputSocket _inputSocketScaleZ;


		private Rect _tmpRect;

		public EntitiesNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketGameObject = new InputSocket(this, typeof(IGameObjectsConnection));
			_inputSocketPositions = new InputSocket(this, typeof(IVectorConnection));

			_inputSocketRotationZ = new InputSocket(this, typeof(INumberConnection));
			_inputSocketRotationX = new InputSocket(this, typeof(INumberConnection));
			_inputSocketRotationY = new InputSocket(this, typeof(INumberConnection));

			_inputSocketScaleZ = new InputSocket(this, typeof(INumberConnection));
			_inputSocketScaleZ.SetDirectInputNumber(1, false);
			_inputSocketScaleX = new InputSocket(this, typeof(INumberConnection));
			_inputSocketScaleX.SetDirectInputNumber(1, false);
			_inputSocketScaleY = new InputSocket(this, typeof(INumberConnection));
			_inputSocketScaleY.SetDirectInputNumber(1, false);


			Sockets.Add(_inputSocketGameObject);
			Sockets.Add(_inputSocketPositions);

			Sockets.Add(_inputSocketRotationX);
			Sockets.Add(_inputSocketRotationY);
			Sockets.Add(_inputSocketRotationZ);

			Sockets.Add(_inputSocketScaleX);
			Sockets.Add(_inputSocketScaleY);
			Sockets.Add(_inputSocketScaleZ);

			Sockets.Add(new OutputSocket(this, typeof(IEntitiesConnection)));
			Height = 180;
		}

		protected override void OnGUI()
		{
			_tmpRect.Set(2, 0, 70, 20);
			GUI.Label(_tmpRect, "game object");

			_tmpRect.Set(2, 20, 70, 20);
			GUI.Label(_tmpRect, "positions");

			_tmpRect.Set(3, 40, 70, 20);
			GUI.Label(_tmpRect, "rotation x");
			_tmpRect.Set(3, 60, 70, 20);
			GUI.Label(_tmpRect, "rotation y");
			_tmpRect.Set(3, 80, 70, 20);
			GUI.Label(_tmpRect, "rotation z");

			_tmpRect.Set(3, 100, 70, 20);
			GUI.Label(_tmpRect, "scale x");
			_tmpRect.Set(3, 120, 70, 20);
			GUI.Label(_tmpRect, "scale y");
			_tmpRect.Set(3, 140, 70, 20);
			GUI.Label(_tmpRect, "scale z");
		}

		public override void Update()
		{

		}

		public List<UnityEngine.Vector3> GetPositions(Request request)
		{
			return AbstractVector3Node.GetInputVector3List(_inputSocketPositions, request);
		}

		public float GetRotationX(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketRotationX, request);
		}

		public float GetRotationY(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketRotationY, request);
		}

		public float GetRotationZ(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketRotationZ, request);
		}


		public float GetScaleX(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketScaleX, request);
		}

		public float GetScaleY(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketScaleY, request);
		}

		public float GetScaleZ(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketScaleZ, request);
		}


		public UnityEngine.GameObject GetGameObject(OutputSocket ouputSocket, Request request)
		{
			return GameObjectNode.GetInputGameObject(_inputSocketGameObject, request);
		}
	}
}
