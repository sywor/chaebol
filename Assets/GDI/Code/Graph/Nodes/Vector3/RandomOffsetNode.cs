using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Random Offset")]
	public class RandomOffsetNode : AbstractVector3Node
	{

		[SerializeField] private bool _offsetX = true;
		[SerializeField] private bool _offsetY = true;
		[SerializeField] private bool _offsetZ = true;

		private InputSocket _inputSocketVec;
		private InputSocket _inputSocketOffset;
		private InputSocket _inputSocketNoise;

		private Rect _tmpRect;

		public RandomOffsetNode(int id, Graph parent) : base(id, parent)
		{

			_inputSocketVec = new InputSocket(this, typeof(IVectorConnection));
			_inputSocketOffset = new InputSocket(this, typeof(INumberConnection));
			_inputSocketNoise = new InputSocket(this, typeof(INumberConnection));

			Sockets.Add(_inputSocketVec);
			Sockets.Add(_inputSocketOffset);
			Sockets.Add(_inputSocketNoise);

			Sockets.Add(new OutputSocket(this, typeof(IVectorConnection)));
			Width = 90;
			Height = 80;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "vec");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "offset");
			_tmpRect.Set(3, 40, 50, 20);
			GUI.Label(_tmpRect, "noise");

			_tmpRect.Set(50, 0, 50, 20);
			var currentOffsetX = GUI.Toggle(_tmpRect, _offsetX, "x");
			_tmpRect.Set(50, 20, 50, 20);
			var currentOffsetY = GUI.Toggle(_tmpRect, _offsetY, "y");
			_tmpRect.Set(50, 40, 50, 20);
			var currentOffsetZ = GUI.Toggle(_tmpRect, _offsetZ, "z");

			bool needsUpdate = currentOffsetX != _offsetX || currentOffsetY != _offsetY || currentOffsetZ != _offsetZ;
			_offsetX = currentOffsetX;
			_offsetY = currentOffsetY;
			_offsetZ = currentOffsetZ;
			if (needsUpdate) TriggerChangeEvent();

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket socket, Request request)
		{
			Request r = new Request();
			List<UnityEngine.Vector3> vec = GetInputVector3List(_inputSocketVec, request);
			if (vec == null) return null;

			for (var i = 0; i < vec.Count; i++)
			{
				UnityEngine.Vector3 v = vec[i];
				r.X = v.x;
				r.Y = v.y;
				r.Z = v.z;
				r.Seed = request.Seed;
				float noise = AbstractNumberNode.GetInputNumber(_inputSocketNoise, r);
				float offset = AbstractNumberNode.GetInputNumber(_inputSocketOffset, r);

				if (float.IsNaN(noise) || float.IsNaN(offset))	continue;

				if (_offsetX) v.x += offset * noise;
				if (_offsetY) v.y += offset * noise;
				if (_offsetZ) v.z += offset * noise;

				vec[i] = v;
			}

			return vec;
		}
	}
}
