using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Vector3
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Operator")]
	public class OperatorNode : AbstractVector3Node
	{

		[SerializeField] private int _selectedMode;
		[NonSerialized] public static string[] Operations = {"add", "sub", "mul", "div"};

		private InputSocket _inputSocketVectors;
		private InputSocket _inputSocketX;
		private InputSocket _inputSocketY;
		private InputSocket _inputSocketZ;

		private Rect _tmpRect;

		public OperatorNode(int id, Graph parent) : base(id, parent)
		{
			_tmpRect = new Rect();
			_inputSocketVectors = new InputSocket(this, typeof(IVectorConnection));

			_inputSocketX = new InputSocket(this, typeof(INumberConnection));
			_inputSocketY = new InputSocket(this, typeof(INumberConnection));
			_inputSocketZ = new InputSocket(this, typeof(INumberConnection));

			Sockets.Add(_inputSocketVectors);
			Sockets.Add(_inputSocketX);
			Sockets.Add(_inputSocketY);
			Sockets.Add(_inputSocketZ);

			Sockets.Add(new OutputSocket(this, typeof(IVectorConnection)));
			Width = 100;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 60, 20);
			GUI.Label(_tmpRect, "vec");

			_tmpRect.Set(3, 20, 60, 20);
			GUI.Label(_tmpRect, "x");

			_tmpRect.Set(3, 40, 60, 20);
			GUI.Label(_tmpRect, "y");

			_tmpRect.Set(3, 60, 60, 20);
			GUI.Label(_tmpRect, "z");

			_tmpRect.Set(50, 0, 40, 80);
			int newMode = GUI.SelectionGrid(_tmpRect, _selectedMode, Operations, 1, "toggle");
			if (newMode != _selectedMode)
			{
				_selectedMode = newMode;
				TriggerChangeEvent();
			}

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket s, Request request)
		{
			Request r = new Request();
			List<UnityEngine.Vector3> vectors = GetInputVector3List(_inputSocketVectors, request);
			if (vectors != null)
			{
				for (var i = 0; i < vectors.Count; i++)
				{
					UnityEngine.Vector3 v = vectors[i];
					r.X = v.x;
					r.Y = v.y;
					r.Z = v.z;
					r.Seed = request.Seed;
					float valueX = AbstractNumberNode.GetInputNumber(_inputSocketX, r);
					float valueY = AbstractNumberNode.GetInputNumber(_inputSocketY, r);
					float valueZ = AbstractNumberNode.GetInputNumber(_inputSocketZ, r);

					if (_selectedMode == 0) v.Set(v.x + valueX, v.y + valueY, v.z + valueZ);
					if (_selectedMode == 1) v.Set(v.x - valueX, v.y - valueY, v.z - valueZ);
					if (_selectedMode == 2) v.Set(v.x * valueX, v.y * valueY, v.z * valueZ);
					if (_selectedMode == 3) v.Set(valueX != 0 ? v.x / valueX : v.x, valueY != 0 ? v.y / valueY : v.y, valueZ != 0 ? v.z / valueZ : valueZ);
					vectors[i] = v;
				}
			}
			return vectors;
		}
	}
}
