using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Socket
{
	public class OutputSocket : AbstractSocket
	{

		public List<Edge> Edges;

		public OutputSocket(Node parent, Type type) : base(parent, type)
		{
			Edges = new List<Edge>();
		}

		public override bool IsConnected()
		{
			return Edges.Count > 0;
		}

		public override bool Intersects(Vector2 nodePosition)
		{
			if (Parent.Collapsed) return false;
			return BoxRect.Contains(nodePosition);
		}

		public override int GetEdgeCount()
		{
			return Edges.Count;
		}

		public override void UpdateEdgeBounds()
		{
			for (var i = 0; i < Edges.Count; i++)
			{
				Edges[i].UpdateBounds();
			}
		}

		protected override void OnDraw()
		{
			GUI.Box(BoxRect, ">");
		}
	}
}
