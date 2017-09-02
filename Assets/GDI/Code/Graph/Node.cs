using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Nodes;
using Assets.GDI.Code.Graph.Nodes.Fabricator;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph
{

	/// <summary>
	/// A node that is part of a graph. It can contain input and output sockets and
	/// can be connected to other nodes. It can have a GUI and the GUI can be updated
	/// by external update calls.
	/// This is class is abstract. All nodes must inerhit from this class and implement
	/// the abstract methods.
	/// </summary>
	public abstract class Node : IUpdateable
	{
 		[NonSerialized] public List<AbstractSocket> Sockets = new List<AbstractSocket>();
		[NonSerialized] public int Id;
		[NonSerialized] private Graph _parent;

		[NonSerialized] public Rect WindowRect;
		[NonSerialized] public int VisitCount = 0;
		[NonSerialized] public Rect ContentRect;
		[NonSerialized] public static int LastFocusedNodeId;
		[NonSerialized] public bool Resizable = true;
		[NonSerialized] public Rect ResizeArea;
		[NonSerialized] public float SocketTopOffsetInput;
		[NonSerialized] public float SocketTopOffsetOutput;

		[NonSerialized] public bool Collapsed;
		[NonSerialized] public bool IsResizing;
		[NonSerialized] public bool IsDragging;

		[NonSerialized] public float Height;

		[NonSerialized] private Vector3 _tmpVec01;
		[NonSerialized] private Vector3 _tmpVec02;

		[NonSerialized] private Rect _resizeRect;

		[NonSerialized] public bool Selected;

		public string Name;

		public bool CustomStyle;
		public Color CustomBackgroundColor;
		public Color CustomTextColor;

		protected Node(int id, Graph parent)
		{
			ResizeArea = new Rect();
			Id = id;
			// default size
			Width = 100;
			Height = 100;
			Name = GetNodeName(GetType());
			_parent = parent;

		}

		public int GetId()
		{
			return Id;
		}

		public void Draw()
		{
			if (Collapsed) return;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			OnGUI();
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}


		public string DrawRenamingField()
		{
			GUI.SetNextControlName("RenamingField");
			string newName = GUI.TextField(new Rect(WindowRect.x, WindowRect.y - 20, WindowRect.width, 20), Name);
			GUI.FocusControl("RenamingField");
			return newName;
		}

		public virtual void OnResize()
		{
			Width = Event.current.mousePosition.x;
			Height = Event.current.mousePosition.y + 21;

		}

		protected abstract void OnGUI();

		public abstract void Update();

		public virtual void OnSerialization(SerializableNode sNode)
		{
			// for overriding: gets called before this Node gets serialized
		}

		public virtual void OnDeserialization(SerializableNode sNode)
		{
			// for overriding: gets called after this Node has been deserialized
		}

		/// The x position of the node
		public float X
		{
			get { return WindowRect.x; }
			set { WindowRect.x = value; }
		}

		/// The y position of the node
		public float Y
		{
			get { return WindowRect.y; }
			set { WindowRect.y = value; }
		}

		/// The width of the node
		public float Width
		{
			get { return WindowRect.width; }
			set { WindowRect.width = value; }
		}

		/// <summary>Returns true if the node is focused.</summary>
		/// <returns>True if the node is focused.</returns>
		public bool HasFocus()
		{
			return LastFocusedNodeId == Id;
		}

		public virtual void OnFocus()
		{
			if (_parent.TriggerEvents)
			{
				EventManager.TriggerOnFocusNode(_parent, this);
			}
		}

		public void Collapse()
		{
			WindowRect.Set(WindowRect.x, WindowRect.y, WindowRect.width, 20);
			Collapsed = true;
			OnCollapsed();
		}

		protected virtual void OnCollapsed() {}

		public void Expand()
		{
			WindowRect.Set(WindowRect.x, WindowRect.y, WindowRect.width, Height);
			Collapsed = false;
			Update();
			OnExpanded();
		}

		protected virtual void OnExpanded() {}

		/// <summary>Returns true if this assigned position intersects the node.</summary>
		/// <param name="canvasPosition">The position in canvas coordinates.</param>
		/// <returns>True if this assigned position intersects the node.</returns>
		public bool Intersects(Vector2 canvasPosition)
		{
			return WindowRect.Contains(canvasPosition);
		}

		/// <summary> Returns true if this node contains the assigned socket.</summary>
		/// <param name="socket"> The socket to use.</param>
		/// <returns>True if this node contains the assigned socket.</returns>
		public bool ContainsSocket(AbstractSocket socket)
		{
			return Sockets.Contains(socket);
		}

		public int GetInputSocketCount()
		{
			var count = 0;
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				if (socket.IsInput()) count++;
			}
			return count;
		}


		public AbstractSocket GetSocket(Type edgeType, Type socketType, int index)
		{
			var searchIndex = -1;
			for (int i = 0; i < Sockets.Count; i++)
			{
				var socket = Sockets[i];
				if (socket.Type == edgeType && socket.GetType() == socketType)
				{
					searchIndex++;
					if (searchIndex == index) return socket;
				}
			}
			return null;
		}

		/// <summary>Projects the assigned position to a node relative position.</summary>
		/// <param name="canvasPosition">The position in canvas coordinates.</param>
		/// <returns>The position in node coordinates</returns>
		public Vector2 ProjectToNode(Vector2 canvasPosition)
		{
			canvasPosition.Set(canvasPosition.x - WindowRect.x, canvasPosition.y - WindowRect.y);
			return canvasPosition;
		}

		/// <summary> Searches for a socket that intesects the assigned position.</summary>
		/// <param name="canvasPosition">The position for intersection in canvas coordinates.</param>
		/// <returns>The at the position or null.</returns>
		public AbstractSocket SearchSocketAt(Vector2 canvasPosition)
		{
			//Vector2 nodePosition = ProjectToNode(canvasPosition);
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				if (socket.Intersects(canvasPosition)) return socket;
			}
			return null;
		}

		/// <summary> Triggers the OnChangedNode event from within a Node.
		/// Call this method if your nodes content has changed. </summary>
		public void TriggerChangeEvent()
		{
			if (_parent.TriggerEvents)
			{
				EventManager.TriggerOnChangedNode(_parent, this);
			}
		}

		/// <summary> Returns true if all input Sockets are connected.</summary>
		/// <returns> True if all input Sockets are connected.</returns>
		public bool AllInputSocketsConnected()
		{
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				if (!socket.IsConnected() && socket.IsInput()) return false;
			}
			return true;
		}

		/// <summary> Returns the total number of input edges connected into this node.</summary>
		public int GetConnectedInputCount() {
			int count = 0;
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				if (socket.IsInput() && socket.IsConnected())
				{
					count++;
				}
			}
			return count;
		}

		public void UpdateEdgeBounds()
		{
			GUIAlignSockets();
			for (int index = 0; index < Sockets.Count; index++)
			{
				Sockets[index].UpdateEdgeBounds();
			}
		}

		public int GetEdgeCount()
		{
			int count = 0;
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				count += socket.GetEdgeCount();
			}
			return count;
		}

		/// <summary> Returns true if this node has no input edges.</summary>
		public bool IsRootNode()
		{
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				if (socket.IsInput() && socket.IsConnected()) return false;
			}
			return true;
		}

		public void GUIDrawSockets()
		{
			if (!Collapsed)
				for (int index = 0; index < Sockets.Count; index++)
				{
					var socket = Sockets[index];
					socket.Draw();
				}
		}

		public void GUIDrawEdge(InputSocket socket, bool highlight, bool hover)
		{
			if (socket.IsConnected()) socket.Edge.Draw(highlight, hover);
		}

		/// <summary> Aligns the position of the sockets of this node </summary>
		public void GUIAlignSockets()
		{
			var leftCount = 0;
			var rightCount = 0;
			for (int index = 0; index < Sockets.Count; index++)
			{
				var socket = Sockets[index];
				if (socket.IsInput())
				{
					socket.X = -Config.SocketSize + WindowRect.x;
					socket.Y = GUICalcSocketTopOffset(leftCount) + WindowRect.y + SocketTopOffsetInput;
					leftCount++;
				}
				else
				{
					socket.X = WindowRect.width + WindowRect.x;
					socket.Y = GUICalcSocketTopOffset(rightCount) + WindowRect.y + SocketTopOffsetOutput;
					rightCount++;
				}
			}
		}

		/// <summary> Calculates the offset of a socket from the top of this node </summary>
		private int GUICalcSocketTopOffset(int socketTopIndex)
		{
			return Config.SocketOffsetTop + (socketTopIndex*Config.SocketSize)
				+ (socketTopIndex*Config.SocketMargin);
		}

		/// <summary> Gets the menu entry name of this node </summary>
		public static string GetNodeName(Type nodeType)
		{
			object[] attrs = nodeType.GetCustomAttributes(typeof(GraphContextMenuItem), true);
			if (attrs.Length > 0)
			{
				GraphContextMenuItem attr = (GraphContextMenuItem) attrs[0];
				return string.IsNullOrEmpty(attr.Name) ? nodeType.Name : attr.Name;
			}
			Log.Error("Can not add node of type " + nodeType + ". Node name annotation is missing.");
			return nodeType.ToString();
		}

		/// <summary> Gets the menu entry path of this node </summary>
		public static string GetNodePath(Type nodeType)
		{
			object[] attrs = nodeType.GetCustomAttributes(typeof(GraphContextMenuItem), true);
			if (attrs.Length > 0)
			{
				GraphContextMenuItem attr = (GraphContextMenuItem) attrs[0];
				return string.IsNullOrEmpty(attr.Path) ? null : attr.Path;
			}
			Log.Error("Can not add node of type " + nodeType + ". Node path annotation is missing.");
			return null;
		}

		/// <summary> Converts this node to a SerializableNode </summary>
		public SerializableNode ToSerializedNode()
		{
			SerializableNode n = new SerializableNode();
			n.type = GetType().FullName;
			n.id = Id;
			n.X = WindowRect.xMin;
			n.Y = WindowRect.yMin;
			n.Collapsed = Collapsed;
			n.directInputValues = new float[Sockets.Count];

			for (var i = 0; i < n.directInputValues.Length; i++)
			{
				if (Sockets[i].IsInput())
				{
					InputSocket inputSocket = (InputSocket) Sockets[i];
					if (inputSocket.IsInDirectInputMode()) n.directInputValues[i] = inputSocket.GetDirectInputNumber();
				}

			}

			n.data = JsonUtility.ToJson(this); // custom node data can be used
			OnSerialization(n);
			return n;
		}

		/// <summary>
		/// Returns the color of the edge displayed in the editor.
		///</summary>
		///<param name="connectionType">The type of the connection</param>
		///<returns>The color of the edge.</returns>
		public static Color GetEdgeColor(Type connectionType)
		{
			if (connectionType == typeof(INumberConnection)) 		return AbstractNumberNode.EdgeColor;
			if (connectionType == typeof(IColorConnection)) 		return AbstractColorNode.EdgeColor;
			if (connectionType == typeof(IStringConnection)) 		return AbstractStringNode.EdgeColor;
			if (connectionType == typeof(IVectorConnection)) 		return AbstractVector3Node.EdgeColor;
			if (connectionType == typeof(IMaterialConnection)) 	return AbstractMaterialNode.EdgeColor;
			if (connectionType == typeof(ITextureConnection)) 	return AbstractTextureNode.EdgeColor;
			if (connectionType == typeof(IColorMapConnection)) 	return AbstractColorMapNode.EdgeColor;
			if (connectionType == typeof(IChunkConnection)) 		return AbstractChunkNode.EdgeColor;
			if (connectionType == typeof(ILandscapeConnection)) 	return Color.green;
			if (connectionType == typeof(IGameObjectsConnection)) return Color.cyan;
			if (connectionType == typeof(IEntitiesConnection)) 	return Color.blue;
			if (connectionType == typeof(IResourceConnection)) return Color.green;
			if (connectionType == typeof(IProductConnection)) return Color.red;
			if (connectionType == typeof(IByProductConnection)) return Color.blue;
			
			return Color.black;
		}
	}

	/// <summary> A class to serialize a Node.</summary>
	[Serializable] public class SerializableNode
	{
		[SerializeField] public string type;
		[SerializeField] public int id;
		[SerializeField] public float X;
		[SerializeField] public float Y;
		[SerializeField] public string data;
		[SerializeField] public float[] directInputValues;
		[SerializeField] public bool Collapsed;
	}

	/// <summary> Annotation to register menu entries of Nodes to the editor.</summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class GraphContextMenuItem : Attribute
	{
		private  string _menuPath;
		public string Path { get { return _menuPath; } }

		private  string _itemName;
		public string Name { get { return _itemName; } }

		public GraphContextMenuItem(string menuPath) : this(menuPath, null)
		{
		}

		public GraphContextMenuItem(string menuPath, string itemName)
		{
			_menuPath = menuPath;
			_itemName = itemName;
		}
	}


}


