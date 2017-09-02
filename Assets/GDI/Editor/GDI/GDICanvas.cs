using System;
using System.Collections.Generic;
using System.IO;
using Assets.GDI.Code;
using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Socket;
using UnityEditor;
using UnityEngine;

namespace Assets.GDI.Editor.GDI
{
	[Serializable]
	public class GDICanvas
	{
		public GUIStyle Style = new GUIStyle();

		public const float GDICanvasSize = 100000;

		public Rect DrawArea = new Rect();

		[SerializeField] public float Zoom = 1;
		[SerializeField] public Vector2 Position = new Vector2();

		public Graph Graph;

		public Rect TabButton = new Rect();
		public Rect CloseTabButton = new Rect();

		private Vector2 _tmpVector01;
		private Vector2 _tmpVector02;
		private Rect _tmpRect;
		private Vector3 _tmpVec01;
		private Vector3 _tmpVec02;

		private Rect _tmpNodeWindow;
		private string _currentHelpText;
		private GUIStyle _toastTextStyle;

		private Color _backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
		private Color _backgroundLineColor01 = new Color(0.14f, 0.14f, 0.14f, 1f);
		private Color _backgroundLineColor02 = new Color(0.10f, 0.10f, 0.10f, 1f);
		private Color _selectionBoxColor = new Color(0.8f, 0.8f, 0.8f, 0.2f);
		private Color _tooltipBoxColor = new Color(0.0f, 0.0f, 0.0f, 0.3f);

		private GUIStyle _windowSelectedStyle;
		private GUIStyle _windowStyle;

		private GUIStyle _centeredLabelStyle;

		private List<Node> _selectedNodes = new List<Node>();

		private int _draggingPathPointIndex = -1;
		private Edge _hoveringEdge;
		private KeyCode _lastKeyCode = KeyCode.None;

		private Rect _selectionBox;
		private bool _selectionBoxDraging;

		private int _renamingNodeId = -1;

		private static Dictionary<Color?, Texture2D> _solidTextures = new Dictionary<Color?, Texture2D>();

		private bool _initialEdgeCalcDone;

		public GDICanvas(Graph graph)
		{
			Graph = graph;
			Style.normal.background = CreateBackgroundTexture();
			Style.normal.background.wrapMode = TextureWrapMode.Repeat;
			Style.fixedHeight = GDICanvasSize;
			Style.fixedWidth = GDICanvasSize;
			_solidTextures.Clear();

			AddToSelection(graph.GetNodeAt(0));
		}

		private Texture2D CreateBackgroundTexture()
		{
			var texture = new Texture2D(100, 100, TextureFormat.ARGB32, true);
			for (var x = 0; x < texture.width; x++)
			{
				for (var y = 0; y < texture.width; y++)
				{
					bool isVerticalLine = x % 11 == 0;
					bool isHorizontalLine = y % 11 == 0;
					if (x == 0 || y == 0) texture.SetPixel(x, y, _backgroundLineColor02);
					else if (isVerticalLine || isHorizontalLine) texture.SetPixel(x, y, _backgroundLineColor01);
					else texture.SetPixel(x, y, _backgroundColor);
				}
			}
			texture.filterMode = FilterMode.Trilinear;
			texture.wrapMode = TextureWrapMode.Repeat;
			texture.Apply();
			return texture;
		}

		public void Draw(EditorWindow window, Rect region, AbstractSocket currentDragingSocket)
		{
			Event e = Event.current;

			InitWindowStyles();

			if (e.type == EventType.MouseUp)
			{
				if (_draggingPathPointIndex != -1)
				{
					_draggingPathPointIndex = -1;
					Event.current.Use();
					OnEdgePointDragEnd();
				}

			}

			if (e.type == EventType.MouseDown)
			{
				_currentHelpText = null;
				_renamingNodeId = -1;
			}

			if (_centeredLabelStyle == null) _centeredLabelStyle = GUI.skin.GetStyle("Label");
			_centeredLabelStyle.alignment = TextAnchor.MiddleCenter;

			EditorZoomArea.Begin(Zoom, region);

			if (Style.normal.background == null) Style.normal.background = CreateBackgroundTexture();
			GUI.DrawTextureWithTexCoords(DrawArea, Style.normal.background, new Rect(0, 0, 1000, 1000));
			DrawArea.Set(Position.x, Position.y, GDICanvasSize, GDICanvasSize);
			GUILayout.BeginArea(DrawArea);
			DrawEdges(region);
			window.BeginWindows();
			DrawNodes(region);
			window.EndWindows();
			DrawDragEdge(currentDragingSocket);

			GUILayout.EndArea();
			EditorZoomArea.End();


			if (e.type == EventType.MouseDrag)
			{
				if (e.button == 0)
				{
					if (_draggingPathPointIndex > -1 && _hoveringEdge != null)
					{

						Vector2 pos = ProjectToGDICanvas(e.mousePosition);
						pos.y += Config.PathPointSize;
						_hoveringEdge.SetPathPointAtIndex(_draggingPathPointIndex, pos);
						Event.current.Use();
					}
				}
			}

			if (_hoveringEdge != null && Config.ShowEdgeHover)
			{
				string tooltipText = _hoveringEdge.Output.Parent.Name + "-> " + _hoveringEdge.Input.Parent.Name;
				float width = GUI.skin.textField.CalcSize(new GUIContent(tooltipText)).x;
				_tmpRect.Set(Event.current.mousePosition.x - width / 2f, e.mousePosition.y - 30, width, 20);
				DrawTooltip(_tmpRect, tooltipText);
			}


			HandleSelectionBox();
			DrawHelpText();


			if (!_initialEdgeCalcDone)
			{
				_initialEdgeCalcDone = true;
				Graph.UpdateEdgeBounds();
			}

			GUI.SetNextControlName(Config.NullControlName);
			GUI.TextField(new Rect(0, 0, 0, 0), "");
			if (GUI.GetNameOfFocusedControl().Equals(Config.NullControlName))
			{

				if (e.keyCode == KeyCode.Delete) DeleteSelectedNodes(null);
				if ((e.control || e.command) && e.keyCode == KeyCode.C && _lastKeyCode != KeyCode.C)
				{
					CopySelection();
				}
				if ((e.control || e.command) && e.keyCode == KeyCode.V && _lastKeyCode != KeyCode.V)
				{
					PasteFromClipboard();
				}
			}

			_lastKeyCode = e.keyCode;
		}

		private void ResetTextFieldFocus()
		{
			GUI.FocusControl(Config.NullControlName); // reset Focus
		}

		private void DrawTooltip(Rect rect, string text)
		{
			EditorGUI.DrawRect(rect, _tooltipBoxColor);
			GUI.Label(rect, text);
		}

		private void DrawHelpText()
		{
			if (_currentHelpText != null)
			{
				Vector2 size = _toastTextStyle.CalcSize(new GUIContent(_currentHelpText));
				float width = size.x + _toastTextStyle.padding.left;
				float height = size.y + _toastTextStyle.padding.top;
				_tmpRect.Set(30, 80, width, height);
				EditorGUI.DrawRect(_tmpRect, Color.white);
				GUI.TextArea(_tmpRect, _currentHelpText, _toastTextStyle);
				_tmpRect.Set(_tmpRect.x + _tmpRect.width - 20, _tmpRect.y, 20, 20);
			}
		}

		private void DrawDragEdge(AbstractSocket currentDragingSocket)
		{
			if (currentDragingSocket != null)
			{
				_tmpVector01 = Edge.GetEdgeSocketPosition(currentDragingSocket, _tmpVector01);
				_tmpVector02 = Edge.GetSocketTangentPosition(currentDragingSocket, _tmpVector01);
				Edge.DrawEdgeSegment(_tmpVector01, _tmpVector02, Event.current.mousePosition, Event.current.mousePosition,
					currentDragingSocket.Type, true, false);
			}
		}

		public static Texture2D GetSolidTexture(Color color)
		{
			if (_solidTextures.ContainsKey(color))
			{
				return _solidTextures[color];
			}
			Texture2D texture = NodeUtils.MakeTex(1, 1, color);
			_solidTextures.Add(color, texture);
			return texture;
		}

		private void InitWindowStyles()
		{
			if (_windowSelectedStyle == null)
			{
				_windowSelectedStyle = new GUIStyle();
				_windowSelectedStyle.padding.bottom = 5;
				_windowSelectedStyle.padding.top = 5;
				_windowSelectedStyle.padding.left = 5;
				_windowSelectedStyle.padding.right = 5;
				_windowSelectedStyle.normal.textColor = Color.grey;
				//_windowSelectedStyle.normal.background = NodeUtils.MakeTex(1, 1, Config.SelectedNodeColor);
				_windowSelectedStyle.normal.background = GetSolidTexture(Config.SelectedNodeColor);
				_windowSelectedStyle.fontStyle = FontStyle.Bold;
			}

			if (_windowStyle == null)
			{
				_windowStyle = new GUIStyle();
				_windowStyle.padding.bottom = 5;
				_windowStyle.padding.top = 5;
				_windowStyle.padding.left = 5;
				_windowStyle.padding.right = 5;
				_windowStyle.normal.textColor = Color.white;
				_windowStyle.normal.background = GetSolidTexture(Config.NodeColor);
				_windowStyle.fontStyle = FontStyle.Bold;
			}

			if (_toastTextStyle == null)
			{
				_toastTextStyle = new GUIStyle();
				_toastTextStyle.richText = true;
				_toastTextStyle.padding.left = 10;
				_toastTextStyle.padding.top = 10;
				_toastTextStyle.fontSize = 13;
			}
		}

		public bool CanvasOverlapsWindow(Rect canvasRect, Rect windowRect)
		{
			Vector2 topLeft = ProjectToWindow(new Vector3(canvasRect.x, canvasRect.y));
			Vector2 bottomRight = ProjectToWindow(new Vector3(canvasRect.x + canvasRect.width, canvasRect.y + canvasRect.height));
			Rect projectedCanvas = new Rect();
			projectedCanvas.Set(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
			return windowRect.Overlaps(projectedCanvas);
		}

		public void DrawNodes(Rect canvasAreaInWindow)
		{
			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				if (!node.Collapsed) node.WindowRect.height = node.Height;

				bool isInBounds = CanvasOverlapsWindow(node.WindowRect, canvasAreaInWindow);

				if (isInBounds)
				{
					var style = GetNodeStyle(node);
					_tmpNodeWindow = GUI.Window(node.Id, node.WindowRect, GUIDrawNodeWindow, node.Name + "", style);
					HandleNodeDraging(node);
					HandleNodeRenaming(node);
				}

				node.GUIAlignSockets();
				node.GUIDrawSockets();

				if (Config.ShowSocketHover) DrawSocketTooltip(node);

			}
		}

		private void HandleNodeRenaming(Node node)
		{
			if (_renamingNodeId == node.Id)
			{
				string newName = node.DrawRenamingField();
				if (newName != null && node.Name != newName)
				{
					string oldName = node.Name;
					node.Name = newName;
					EventManager.TriggerOnRenameNode(Graph, node, oldName, newName);
				}

				if (Event.current.keyCode == KeyCode.Escape)
				{
					OnRenamingEnds();
				}
			}
		}

		private void HandleNodeDraging(Node node)
		{
			if (!node.IsResizing)
			{
				bool nodeHasMoved = !_tmpNodeWindow.Equals(node.WindowRect);
				if (nodeHasMoved)
				{

					if (!node.IsDragging)
					{
						node.IsDragging = true;
						OnDragStart(node);
					}
					node.IsDragging = true;
					node.UpdateEdgeBounds();

					OnDrag(node, _tmpNodeWindow.x - node.WindowRect.x, _tmpNodeWindow.y - node.WindowRect.y);
				}
				node.WindowRect = _tmpNodeWindow;
			}
		}

		private GUIStyle GetNodeStyle(Node node)
		{
			GUIStyle style;
			if (_selectedNodes.Contains(node)) style = _windowSelectedStyle;
			else
			{
				if (node.CustomStyle)
				{
					style = new GUIStyle(_windowStyle);
					Texture2D t = GetSolidTexture(node.CustomBackgroundColor);
					style.normal.background = t;
					style.normal.textColor = node.CustomTextColor;
					style.fontStyle = FontStyle.Bold;
				}
				else
				{
					style = _windowStyle;
				}
			}
			return style;
		}

		private void DrawSocketTooltip(Node node)
		{
			Vector2 m = Event.current.mousePosition;
			AbstractSocket socket = node.SearchSocketAt(m);
			if (socket != null)
			{
				string text = socket.Type + "";
				int lastIndex = text.LastIndexOf(".");
				if (lastIndex > -1) text = text.Substring(lastIndex + 1);
				float width = GUI.skin.label.CalcSize(new GUIContent(text)).x + 8;
				float xOffset = -width - 10;
				if (socket.IsOutput()) xOffset = 10;
				_tmpRect.Set(m.x + xOffset, m.y - 10, width, 20);
				DrawTooltip(_tmpRect, text);
			}
		}

		private void OnRenamingEnds()
		{
			_renamingNodeId = -1;
		}

		private void OnDragStart(Node node)
		{
		}

		private void OnDragEnd(Node node)
		{

		}

		private void OnEdgePointDragEnd()
		{

		}

		private void OnDrag(Node node, float offsetX, float offsetY)
		{

			if (_selectedNodes.Contains(node))
			{
				for (int index = 0; index < _selectedNodes.Count; index++)
				{
					var selectedNode = _selectedNodes[index];
					selectedNode.WindowRect.x += offsetX;
					selectedNode.WindowRect.y += offsetY;

					for (var socketIndex = 0; socketIndex < selectedNode.Sockets.Count; socketIndex++)
					{
						InputSocket s = selectedNode.Sockets[socketIndex] as InputSocket;
						if (s != null && s.IsConnected() && _selectedNodes.Contains(s.GetConnectedSocket().Parent))
						{
							// move path points if both nodes are draging
							s.Edge.MoveAllPathPoints(offsetX, offsetY);
						}
					}

					selectedNode.UpdateEdgeBounds();
				}
			}
		}

		private void OnFocusNode(Node node)
		{
			node.OnFocus();
		}

		void GUIDrawNodeWindow(int nodeId)
		{
			Node node = Graph.GetNode(nodeId);
			node.ContentRect.Set(0, Config.SocketOffsetTop,
				node.Width, node.Height - Config.SocketOffsetTop);

			if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && _draggingPathPointIndex == -1)
			{
				if (_selectedNodes.Count == 0 || (_selectedNodes.Count == 1 && _selectedNodes.Contains(node))) ShowNodeMenu(node);
				else
				{
					if (_selectedNodes.Count > 0)
					{
						if (_selectedNodes.Contains(node))
						{
							ShowSelectionMenu();
						}
						else
						{
							ShowNodeMenu(node);
						}
					}
				}
			}

			if (!node.Collapsed)
			{
				GUILayout.BeginArea(node.ContentRect);
				GUI.color = Color.white;
				node.Draw();
				GUILayout.EndArea();
			}

			if (Event.current.type == EventType.MouseUp)
			{
				_selectionBoxDraging = false;
				if (node.IsDragging)
				{
					node.IsDragging = false;
					OnDragEnd(node);
					if (_selectedNodes.Count == 0) AddToSelection(node);
				}
				else
				{
					if (!Event.current.shift) ClearSelection();
					AddToSelection(node);
				}
			}

			GUI.DragWindow();


			if (Event.current.GetTypeForControl(node.Id) == EventType.Used)
			{
				if (Node.LastFocusedNodeId != node.Id) OnFocusNode(node);
				Node.LastFocusedNodeId = node.Id;
			}

			DrawWindowBorder(node);
		}

		private void AddToSelection(Node node)
		{
			if (node != null)
			{
				ResetTextFieldFocus();
				if (!_selectedNodes.Contains(node)) _selectedNodes.Add(node);
				node.Selected = true;
			}
		}

		private void ClearSelection()
		{
			ResetTextFieldFocus();
			for (int i = 0; i < _selectedNodes.Count; i++)
			{
				_selectedNodes[i].Selected = false;
			}
			_selectedNodes.Clear();
		}

		private void AddToSelection(List<Node> nodes)
		{
			for (int index = 0; index < nodes.Count; index++)
			{
				AddToSelection(nodes[index]);
			}
		}

		private void DrawWindowBorder(Node node)
		{
			Handles.color = Color.black;
			_tmpVec01.Set(0, 0, 0);
			_tmpVec02.Set(0, node.WindowRect.height, 0);
			Handles.DrawLine(_tmpVec01, _tmpVec02);
			_tmpVec01.Set(0, 0, 0);
			_tmpVec02.Set(node.WindowRect.width, 0, 0);
			Handles.DrawLine(_tmpVec01, _tmpVec02);
			_tmpVec01.Set(0, node.WindowRect.height - 0.5f, 0);
			_tmpVec02.Set(node.WindowRect.width - 0.5f, node.WindowRect.height - 0.5f, 0);
			Handles.DrawLine(_tmpVec01, _tmpVec02);

			_tmpVec01.Set(node.WindowRect.width - 0.5f, node.WindowRect.height - 0.5f, 0);
			_tmpVec02.Set(node.WindowRect.width - 0.5f, 0, 0);
			Handles.DrawLine(_tmpVec01, _tmpVec02);
		}


		private void HandleResize(Node node)
		{

		}

		private void ShowNodeMenu(Node node)
		{
			ResetTextFieldFocus();
			Matrix4x4 m4 = GUI.matrix;
			GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / Zoom, 1 / Zoom, 1 / Zoom));

			GenericMenu m = new GenericMenu();
			m.AddDisabledItem(new GUIContent(node.Name + " (Id: " + node.Id + ")"));
			m.AddSeparator("");
			m.AddItem(new GUIContent("Copy"), false, CopyNode, node.Id);
			m.AddItem(new GUIContent("Rename"), false, StartRenameNode, node.Id);
			m.AddItem(new GUIContent("Delete"), false, DeleteNode, node.Id);
			if (node.Collapsed) m.AddItem(new GUIContent("Expand"), false, ExpandNode, node.Id);
			else m.AddItem(new GUIContent("Collapse"), false, CollapseNode, node.Id);
			m.AddItem(new GUIContent("Help"), false, ShowHelpText, node.Id);
			m.ShowAsContext();

			GUI.matrix = m4;
			Event.current.Use();
		}

		private void ShowSelectionMenu()
		{
			ResetTextFieldFocus();
			Matrix4x4 m4 = GUI.matrix;
			GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / Zoom, 1 / Zoom, 1 / Zoom));

			GenericMenu m = new GenericMenu();
			m.AddDisabledItem(new GUIContent("Selected " + _selectedNodes.Count));
			m.AddSeparator("");
			m.AddItem(new GUIContent("Copy"), false, CopySelection);
			m.AddItem(new GUIContent("Delete"), false, DeleteSelectedNodes, null);
			m.AddItem(new GUIContent("Collapse"), false, CollapseSelectedNodes, null);
			m.AddItem(new GUIContent("Expand"), false, ExpandSelectedNodes, null);
			//if (node.Collapsed) m.AddItem(new GUIContent("Expand"), false, ExpandNode, node.Id);
			//else m.AddItem(new GUIContent("Collapse"), false, CollapseNode, node.Id);
			m.ShowAsContext();

			GUI.matrix = m4;
			Event.current.Use();
		}

		private void HandleSelectionBox()
		{
			if (_draggingPathPointIndex > -1) return;

			if (Event.current.type == EventType.MouseDown)
			{
				_selectionBox.Set(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);
				_selectionBoxDraging = true;
			}

			if (Event.current.type == EventType.MouseDrag)
			{
				_selectionBox.Set(_selectionBox.x, _selectionBox.y, Event.current.mousePosition.x - _selectionBox.x,
					Event.current.mousePosition.y - _selectionBox.y);
			}

			if (Event.current.type == EventType.MouseUp)
			{
				_selectionBox.Set(_selectionBox.x, _selectionBox.y, Event.current.mousePosition.x - _selectionBox.x,
					Event.current.mousePosition.y - _selectionBox.y);
				ClearSelection();
				AddToSelection(GetIntersetctingNodes(_selectionBox));
				_selectionBoxDraging = false;
			}

			_tmpVector01.Set(_selectionBox.x, _selectionBox.y);
			_tmpVector02.Set(_selectionBox.x + _selectionBox.width, _selectionBox.y + _selectionBox.height);
			if (DrawArea.Contains(_tmpVector01) && DrawArea.Contains(_tmpVector02) && _selectionBoxDraging)
			{
				EditorGUI.DrawRect(_selectionBox, _selectionBoxColor);
				Handles.color = Color.black;
				Handles.DrawLine(_tmpVector01, new Vector2(_selectionBox.x + _selectionBox.width, _selectionBox.y));
				Handles.DrawLine(new Vector2(_selectionBox.x + _selectionBox.width, _selectionBox.y), _tmpVector02);
				Handles.DrawLine(_tmpVector02, new Vector2(_selectionBox.x, _selectionBox.y + _selectionBox.height));
				Handles.DrawLine(new Vector2(_selectionBox.x, _selectionBox.y + _selectionBox.height), _tmpVector01);
			}

			if (Event.current.type == EventType.Ignore) _selectionBoxDraging = false;
		}

		private List<Node> GetIntersetctingNodes(Rect rect)
		{
			List<Node> intersectingNodes = new List<Node>();
			_tmpVector01.Set(rect.x, rect.y);
			_tmpVector01 = ProjectToGDICanvas(_tmpVector01);
			_tmpVector02.Set(rect.x + rect.width, rect.y + rect.height);
			_tmpVector02 = ProjectToGDICanvas(_tmpVector02);
			_tmpRect = NormalizeRect(_tmpVector01.x, _tmpVector01.y, _tmpVector02.x - _tmpVector01.x,
				_tmpVector02.y - _tmpVector01.y);

			for (int i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node n = Graph.GetNodeAt(i);
				if (n != null)
				{
					if (_tmpRect.Overlaps(n.WindowRect))
					{
						intersectingNodes.Add(n);
					}
				}

			}
			return intersectingNodes;
		}

		private Rect NormalizeRect(float x, float y, float width, float height)
		{
			if (width < 0)
			{
				x = x + width;
				width = Mathf.Abs(width);
			}
			if (height < 0)
			{
				y = y + height;
				height = Mathf.Abs(height);
			}
			_tmpRect.Set(x, y, width, height);
			return _tmpRect;
		}

		private void CollapseNode(object nodeId)
		{
			ResetTextFieldFocus();
			Node node = Graph.GetNode((int) nodeId);
			if (!node.Collapsed) node.Collapse();
		}

		private void CollapseSelectedNodes(object nodeId)
		{
			for (int index = 0; index < _selectedNodes.Count; index++)
			{
				var node = _selectedNodes[index];
				CollapseNode(node.Id);
			}
		}

		private void ExpandSelectedNodes(object nodeId)
		{
			for (int index = 0; index < _selectedNodes.Count; index++)
			{
				var node = _selectedNodes[index];
				ExpandNode(node.Id);
			}
		}

		private void ExpandNode(object nodeId)
		{
			ResetTextFieldFocus();
			Node n = Graph.GetNode((int) nodeId);
			if (n.Collapsed) n.Expand();
		}


		private void ShowHelpText(object nodeId)
		{
			Node n = Graph.GetNode((int) nodeId);
			if (n != null)
			{
				string t = (n.GetType() + "");
				string nodeTapeName = t.Substring(t.LastIndexOf(".") + 1);
				string path = GetDocuPath() + "/html/nodes/" + nodeTapeName + ".html";
				if (File.Exists(path))
				{
					path = path.Replace(" ", "%20");
					Application.OpenURL("file://" + path);
				}
				else
				{
					OpenDocu();
				}
			}
		}

		public static void OpenDocu()
		{
			string path = GetDocuPath();
			path = path.Replace(" ", "%20");
			Application.OpenURL("file://" + path + "/index.html");
		}

		public static string GetDocuPath()
		{
			return Application.dataPath + Config.DocuPath;
		}

		private void DeleteSelectedNodes(object o)
		{
			for (int index = 0; index < _selectedNodes.Count; index++)
			{
				var node = _selectedNodes[index];
				DeleteNode(node.Id);
			}
			ClearSelection();
		}


		private void StartRenameNode(object nodeId)
		{
			_renamingNodeId = (int) nodeId;
		}

		private void DeleteNode(object nodeId)
		{
			Graph.RemoveNode((int) nodeId);
		}


		public void DrawEdges(Rect canvasAreaInWindow)
		{
			if (_draggingPathPointIndex == -1)
			{
				_hoveringEdge = null;
			}

			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);


				for (var iu = 0; iu < node.Sockets.Count; iu++)
				{
					AbstractSocket socket = node.Sockets[iu];
					if (socket.IsInput() && socket.IsConnected()) // draw only input sockets to avoid double drawing of edges
					{
						InputSocket inputSocket = (InputSocket) socket;
						if (CanvasOverlapsWindow(inputSocket.Edge.Bounds, canvasAreaInWindow))
						{
							bool highlight = _selectedNodes.Contains(node)
							                 || _selectedNodes.Contains(inputSocket.Edge.Output.Parent);
							int segmentIndex = inputSocket.Edge.IntersectsPathSegment(Event.current.mousePosition);
							bool hover = segmentIndex > -1;
							node.GUIDrawEdge(inputSocket, highlight, hover);
							if (hover && _draggingPathPointIndex == -1)
							{
								HandleEdgeHover(inputSocket.Edge, segmentIndex);
								_hoveringEdge = inputSocket.Edge;
							}
						}
					}
				}
			}
		}

		private void CopySelection()
		{
			CopyNodes(_selectedNodes);
		}

		private void CopyNode(object nodeId)
		{
			Node n = Graph.GetNode((int) nodeId);
			if (n != null) CopyNodes(new List<Node> {n});
		}

		private void CopyNodes(List<Node> nodes)
		{
			if (nodes != null && nodes.Count > 0)
			{
				SerializationContainer container = Graph.Serialize(nodes);
				string json = JsonUtility.ToJson(container);
				GUIUtility.systemCopyBuffer = json;
			}
		}

		public void PasteFromClipboard()
		{
			string json = GUIUtility.systemCopyBuffer;
			if (!string.IsNullOrEmpty(json))
			{
				try
				{
					SerializationContainer container = JsonUtility.FromJson<SerializationContainer>(json);
					if (container != null)
					{
						List<Node> nodes = Graph.Deserialize(container.Nodes, container.Edges);
						if (nodes != null && nodes.Count > 0)
						{
							Vector2 pastePosition = ProjectToGDICanvas(new Vector2(Screen.width / 2f, Screen.height / 2f));
							Vector2 pasteOffset = pastePosition - new Vector2(nodes[0].WindowRect.x, nodes[0].WindowRect.y);
							Graph.MoveNodes(nodes, pasteOffset);
							Graph.SetNewIds(nodes);
							bool wasTrigger = Graph.TriggerEvents;
							Graph.TriggerEvents = false;
							Graph.AddNodes(nodes);
							Graph.UpdateEdgeBounds();
							Graph.TriggerEvents = wasTrigger;
							_selectedNodes.Clear();
							_selectedNodes = nodes;
							ResetTextFieldFocus();
						}
					}
				}
				catch (ArgumentException e)
				{
					Log.Info("Can not parse clipboard content. " + e.Message);
				}
			}
		}

		private void HandleEdgeHover(Edge edge, int segmentIndex)
		{

			if (Event.current.type == EventType.MouseDown)
			{
				if (Event.current.button == 0)
				{
					_draggingPathPointIndex = edge.GetPathPointIndex(Event.current.mousePosition);
				}
				else
				{
					_draggingPathPointIndex = -1;
				}

				Event.current.Use();
			}

			if (Event.current.type == EventType.MouseUp)
			{
				if (_draggingPathPointIndex == -1)
				{
					if (Event.current.button == 0) edge.AddPathPointAt(Event.current.mousePosition, segmentIndex);
					if (Event.current.button == 1) edge.RemovePathPointAt(Event.current.mousePosition);
				}
				Event.current.Use();
			}

		}

		public Node GetFocusedNode()
		{
			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				if (node.HasFocus()) return node;
			}
			return null;
		}

		/// <summary> Returns the socket at the window position.</summary>
		/// <param name="windowPosition"> The position to get the Socket from in window coordinates</param>
		/// <returns>The socket at the posiiton or null or null.</returns>
		public AbstractSocket GetSocketAt(Vector2 windowPosition)
		{
			Vector2 projectedPosition = ProjectToGDICanvas(windowPosition);

			for (var i = 0; i < Graph.GetNodeCount(); i++)
			{
				Node node = Graph.GetNodeAt(i);
				AbstractSocket socket = node.SearchSocketAt(projectedPosition);
				if (socket != null)
				{
					return socket;
				}
			}
			return null;
		}

		public Node CreateNode(Type nodeType, Vector2 windowPosition)
		{

			Node node = (Node) Graph.CreateNode(nodeType);
			var position = ProjectToGDICanvas(windowPosition);
			node.X = position.x;
			node.Y = position.y;
			Graph.AddNode(node);
			return node;
		}

		public void RemoveFocusedNode()
		{

			Node node = GetFocusedNode();
			if (node != null) Graph.RemoveNode(node);
		}

		/// <summary>
		/// Projects a position in unity window coordinates to canvas coordinates.
		/// </summary>
		/// <param name="windowPosition">The position in window coordinates.</param>
		/// <returns>The position in canvas coordinates.</returns>
		public Vector2 ProjectToGDICanvas(Vector2 windowPosition)
		{
			windowPosition.y += 21 - GDIWindow.TopOffset * 2;
			windowPosition = windowPosition / Zoom;
			windowPosition.x -= DrawArea.x;
			windowPosition.y -= DrawArea.y;
			return windowPosition;
		}

		/// <summary>
		/// Projects a position in canvas coordinates to
		/// </summary>
		/// <param name="canvasPosition"></param>
		/// <returns></returns>
		public Vector2 ProjectToWindow(Vector2 canvasPosition)
		{
			canvasPosition.x += DrawArea.x;
			canvasPosition.y += DrawArea.y;
			canvasPosition = canvasPosition * Zoom;
			canvasPosition.y -= 21 - GDIWindow.TopOffset * 2;
			return	canvasPosition;
		}

	}



}


