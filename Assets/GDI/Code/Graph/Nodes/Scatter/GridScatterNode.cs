using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Scatter
{
	[Serializable]
	[GraphContextMenuItem("Vector", "Grid")]
	public class GridScatterNode : AbstractVector3Node
	{

		private InputSocket _inputX;
		private InputSocket _inputZ;

		private Rect _tmpRect;

		public GridScatterNode(int id, Graph parent) : base(id, parent)
		{
			_inputX = new InputSocket(this, typeof(INumberConnection));
			_inputZ = new InputSocket(this, typeof(INumberConnection));

			_inputX.SetDirectInputNumber(5, false);
			_inputZ.SetDirectInputNumber(5, false);

			Sockets.Add(_inputX);
			Sockets.Add(_inputZ);

			Sockets.Add(new OutputSocket(this, typeof(IVectorConnection)));

			_tmpRect = new Rect();

			Height = 60;
			Width = 50;
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRect.Set(3, 0, 50, 20);
			GUI.Label(_tmpRect, "scale x");
			_tmpRect.Set(3, 20, 50, 20);
			GUI.Label(_tmpRect, "scale z");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override List<UnityEngine.Vector3> GetVector3List(OutputSocket s, Request request)
		{
			float left = request.X;
			float right = request.X + request.SizeX;

			if (request.SizeX < 0)
			{
				left = request.X + request.SizeX;
				right = request.X;
			}

			left = Mathf.Floor(left);
			right = Mathf.Ceil(right);

			float bottom = request.Z;
			float top = request.Z + request.SizeZ;

			if (request.SizeZ < 0)
			{
				bottom = request.Z + request.SizeZ;
				top = request.Z;
			}

			bottom = Mathf.Floor(bottom);
			top = Mathf.Ceil(top);

			float scaleX = AbstractNumberNode.GetInputNumber(_inputX, request);
			float scaleZ = AbstractNumberNode.GetInputNumber(_inputZ, request);


			List<UnityEngine.Vector3> positions = new List<UnityEngine.Vector3>();
			for (int leftIndex = Mathf.FloorToInt(left / scaleX); leftIndex <= Mathf.CeilToInt(right / scaleX); leftIndex++)
			{
				for (int bottomIndex = Mathf.FloorToInt(bottom / scaleZ); bottomIndex <= Mathf.CeilToInt(top / scaleZ); bottomIndex++)
				{
					if (leftIndex * scaleX >= left && leftIndex * scaleX < right)
					{
						if (bottomIndex * scaleZ >= bottom && bottomIndex * scaleZ < top)
						{
							positions.Add(new UnityEngine.Vector3(leftIndex * scaleX, 0, bottomIndex * scaleZ));
						}
					}
				}
			}
			return positions;
		}
	}
}
