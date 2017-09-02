using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Vector3")]
	public class Vector3Node : AbstractVector3Node
	{

		private InputSocket _inputX;
		private InputSocket _inputY;
		private InputSocket _inputZ;

		private Rect _tmpRect;

		public Vector3Node(int id, Graph parent) : base(id, parent)
		{
			_tmpRect = new Rect();

			_inputX = new InputSocket(this, typeof(INumberConnection));
			_inputY = new InputSocket(this, typeof(INumberConnection));
			_inputZ = new InputSocket(this, typeof(INumberConnection));


			Sockets.Add(_inputX);
			Sockets.Add(_inputY);
			Sockets.Add(_inputZ);

			Sockets.Add(new OutputSocket(this, typeof(IVectorConnection)));

			Width = 60;
			Height = 84;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "x");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "y");
			_tmpRect.Set(3, 40, 50, 20);
			GUI.Label(_tmpRect, "z");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket outSocket, Request request)
		{
			float valueX = AbstractNumberNode.GetInputNumber(_inputX, request);
			float valueY = AbstractNumberNode.GetInputNumber(_inputY, request);
			float valueZ = AbstractNumberNode.GetInputNumber(_inputZ, request);

			if (float.IsNaN(valueX) || float.IsNaN(valueY) || float.IsNaN(valueZ)) return null;

			List<UnityEngine.Vector3> positions = new List<UnityEngine.Vector3>();
			positions.Add(new UnityEngine.Vector3(valueX, valueY, valueZ));
			return positions;
		}
	}

}
