using System;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Socket
{

	public abstract class AbstractSocket
	{
		public Type Type;
		public Node Parent;

		protected Rect BoxRect;
		protected RectOffset Padding;

		protected AbstractSocket(Node parent, Type type)
		{
			Type = type;
			Parent = parent;
			BoxRect.width = Config.SocketSize;
			BoxRect.height = Config.SocketSize;
		}

		/// The x position of the node
		public float X
		{
			get { return BoxRect.x; }
			set { BoxRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return BoxRect.y; }
			set { BoxRect.y = value; }
		}

		public abstract bool IsConnected();
		protected abstract void OnDraw();
		public abstract bool Intersects(Vector2 nodePosition);
		public abstract int GetEdgeCount();

		public abstract void UpdateEdgeBounds();

		public void Draw()
		{
			if (Padding == null) Padding = new RectOffset(0, 0, -2, 0);

			GUI.skin.box.normal.textColor = Node.GetEdgeColor(Type);
			GUI.skin.box.padding = Padding;
			GUI.skin.box.fontSize = 14;
			GUI.skin.box.fontStyle = FontStyle.Bold;
			OnDraw();
		}

		public bool IsInput()
		{
			return GetType() == typeof(InputSocket);
		}

		public bool IsOutput()
		{
			return GetType() == typeof(OutputSocket);
		}

	}

}


