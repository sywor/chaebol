using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Split")]
	public class SplitNode : AbstractVector3Node
	{
		private InputSocket _inputSocketVector;
		private InputSocket _inputSocketMask;

		private OutputSocket _outputSocket01;
		private OutputSocket _outputSocket02;

		private Rect _tmpRect;

		public SplitNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketMask = new InputSocket(this, typeof(INumberConnection));
			_inputSocketVector = new InputSocket(this, typeof(IVectorConnection));

			_outputSocket01 = new OutputSocket(this, typeof(IVectorConnection));
			_outputSocket02 = new OutputSocket(this, typeof(IVectorConnection));

			Sockets.Add(_inputSocketVector);
			Sockets.Add(_inputSocketMask);

			Sockets.Add(_outputSocket01);
			Sockets.Add(_outputSocket02);
			Height = 60;

		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 40, 20);
			GUI.Label(_tmpRect, "vec");

			_tmpRect.Set(3, 20, 40, 20);
			GUI.Label(_tmpRect, "mask");

			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			_tmpRect.Set(45, 0, 50, 20);
			GUI.Label(_tmpRect, ">=0");

			_tmpRect.Set(45, 20, 50, 20);
			GUI.Label(_tmpRect, "<0");

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket outSocket, Request request)
		{
			List<UnityEngine.Vector3> vec = GetInputVector3List(_inputSocketVector, request);
			if (vec == null) return null;

			List<UnityEngine.Vector3> removeList = new List<UnityEngine.Vector3>();

			Request r = new Request();
			for (var i = 0; i < vec.Count; i++)
			{
				r.X = vec[i].x;
				r.Y = vec[i].y;
				r.Z = vec[i].z;
				r.Seed = request.Seed;
				float maskValue = AbstractNumberNode.GetInputNumber(_inputSocketMask, r);
				if (maskValue < 0 && outSocket == _outputSocket01) removeList.Add(vec[i]);
				if (maskValue >= 0 && outSocket == _outputSocket02) removeList.Add(vec[i]);
			}

			for (int index = 0; index < removeList.Count; index++)
			{
				var remove = removeList[index];
				vec.Remove(remove);
			}
			return vec;
		}
	}
}
