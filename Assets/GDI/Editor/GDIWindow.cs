using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets.GDI.Code;
using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Editor.GDI;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Assets.GDI.Editor
{
	/// <summary>
	/// This class contains the logic of the editor window. It contains canvases that
	/// are containing graphs. It uses the Launcher to load, save and close Graphs.
	/// </summary>
	public class GDIWindow : EditorWindow
	{

		[NonSerialized] public const int TopOffset = 32;
		[NonSerialized] public const int BottomOffset = 20;
		[NonSerialized] public const int TopMenuHeight = 20;

		[SerializeField] private bool wasPlaying;

		private const int TabButtonWidth = 200;
		private const int TabButtonMargin = 4;
		private const int TabCloseButtonSize = TopMenuHeight;

		private const int WindowTitleHeight = 21;
		private const float GDICanvasZoomMin = 0.1f;
		private const float GDICanvasZoomMax = 2.0f;

		private Vector2 _nextTranlationPosition;

		private List<GDICanvas> _canvasList = new List<GDICanvas>();
		private GDICanvas _currentGDICanvas;
		private Rect _canvasRegion = new Rect();

		private AbstractSocket _dragSourceSocket = null;
		private Vector2 _lastMousePosition;

		private GenericMenu _menu;
		private Dictionary<string, Type> _menuEntryToNodeType;

		private List<Graph> _clonedEditModeGraphs = new List<Graph>();

		private int _hoveringTabIndex = -1;


		[MenuItem("Window/" + Config.WindowName)]
		static void OnCreateWindow()
		{
			GDIWindow window = GetWindow<GDIWindow>();
			//GDIWindow window = CreateInstance<GDIWindow>(); // to create a new window
			window.Show();
		}

		public void OnEnable()
		{
			Init();
		}

		public void Init()
		{
			EventManager.OnGraphRemovedByScript += OnGraphRemovedByScript;
			EventManager.OnGraphAddedByScript += OnGraphAddedByScript;
			EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
			// create GameObject and the Component if it is not added to the scene

			titleContent = new GUIContent(Config.WindowName);
			wantsMouseMove = true;
			EventManager.TriggerOnWindowOpen();
			_menuEntryToNodeType = CreateMenuEntries();
			_menu = CreateNodeMenu();

			_canvasList.Clear();
			_currentGDICanvas = null;

			if (Launcher.Instance.GetGraphCount() > 0) LoadGDICanvas(Launcher.Instance);
			UpdateGraphs();
			Repaint();
		}

		private void OnGraphRemovedByScript(Graph graph)
		{
			LoadGDICanvas(Launcher.Instance);
		}

		private void OnGraphAddedByScript(Graph graph)
		{
			LoadGDICanvas(Launcher.Instance);
		}

		private void OnPlaymodeStateChanged()
		{
			if (wasPlaying != Application.isPlaying)
			{
				if (Application.isPlaying) OnPlay();
				else OnStop();
			}
			if (wasPlaying == Application.isPlaying)
			{
				if (Application.isPlaying) BeforeStop();
				else BeforePlay();
			}
			wasPlaying = Application.isPlaying;
			UpdateGraphs();
			Repaint();
		}


		/// <summary>
		/// Caches the eidtor state and resets it for play mode.
		///</summary>
		private void BeforePlay()
		{
			// cache the edit mode graphs
			_clonedEditModeGraphs = Launcher.Instance.CloneGraphs();
			// clear the edit mode grahps
			Launcher.Instance.ClearGraphs(false);
			_canvasList.Clear();
			_currentGDICanvas = null;
		}

		private void BeforeStop()
		{

		}

		private void OnPlay()
		{

		}

		/// <summary>
		/// Sets the editor state to the editor mode.
		///</summary>
		private void OnStop()
		{
			// clear the playmode graphs
			_canvasList.Clear();
			_currentGDICanvas = null;
			Launcher.Instance.ClearGraphs(false);
			// load the edit mode gtaphs
			for (int i = 0; i < _clonedEditModeGraphs.Count; i++)
			{
				Graph g = _clonedEditModeGraphs[i];
				Launcher.Instance.AddGraph(g, false);
			}

			LoadGDICanvas(Launcher.Instance);
		}

		private void UpdateGraphs()
		{
			for (var i = 0; i < Launcher.Instance.GetGraphCount(); i++)
			{
				var graph = Launcher.Instance.GetGraph(i);
				graph.ForceUpdateNodes();
			}
		}

		private void LoadGDICanvas(Launcher launcher)
		{
			for (int index = 0; index < launcher.GetGraphCount(); index++)
			{
				var graph = launcher.GetGraph(index);
				LoadGDICanvas(graph);
			}
		}

		private void LoadGDICanvas(Graph graph)
		{
			_currentGDICanvas = new GDICanvas(graph);
			_canvasList.Add(_currentGDICanvas);
		}

		/// <summary>
		/// Creates a dictonary that maps a menu entry string to a node type using reflection.
		/// </summary>
		/// <returns>
		/// Dictonary that maps a menu entry string to a node type
		/// </returns>
		public Dictionary<string, Type> CreateMenuEntries()
		{
			Dictionary<string, Type> menuEntries = new Dictionary<string, Type>();

			IEnumerable<Type> classesExtendingNodes = Assembly.GetAssembly(typeof(Node)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Node)));

			foreach (Type type in classesExtendingNodes) menuEntries.Add(GetItemMenuName(type), type);

			menuEntries.OrderBy(x => x.Key);
			return menuEntries;
		}

		private string GetItemMenuName(Type type)
		{
			string path = Node.GetNodePath(type);
			string name = Node.GetNodeName(type);
			if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(name)) return path + "/" + name;
			if (string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(name)) return name;
			return "unsorted/" + type;
		}


		/// <summary>Draws the UI</summary>
		void OnGUI()
		{


			AboutPanel.Draw(position.width, position.height);
			if (AboutPanel.Show) return;

			PreferencesPanel.Draw(position.width, position.height);
			if (PreferencesPanel.Show) return;

			HandleShortCuts();
			HandleGDICanvasTranslation();
			HandleSocketDragAndDrop();


			DrawToolbar();

			if (Launcher.Instance == null) return;

			HandleTabButtons();

			if (_currentGDICanvas != null)
			{
				_canvasRegion.Set(0, TopOffset, Screen.width, Screen.height - 2 * TopOffset);
				_currentGDICanvas.Draw(this, _canvasRegion, _dragSourceSocket);
			}

			DrawTabTooltip();

			if (Event.current.type == EventType.ContextClick)
			{
				_menu.ShowAsContext();
				Event.current.Use();
			}

			_lastMousePosition = Event.current.mousePosition;


			if (Event.current.keyCode == KeyCode.Alpha0 || Event.current.keyCode == KeyCode.Keypad0)
			{
				if (_currentGDICanvas != null) _currentGDICanvas.Zoom = 1;
			}


			Repaint();
		}


		private void DrawTabTooltip()
		{
			if (_hoveringTabIndex > -1 && _hoveringTabIndex < _canvasList.Count)
			{
				Rect area = _canvasList[_hoveringTabIndex].TabButton;
				string text = _canvasList[_hoveringTabIndex].Graph.OriginFile;
				if (string.IsNullOrEmpty(text)) text = "Not saved.";
				area.y += area.height / 2f;
				area.width = GUI.skin.label.CalcSize(new GUIContent(text)).x + 10;
				EditorGUI.DrawRect(area, Config.SelectedTabColor);
				GUI.Label(area, text);
			}
		}

		public void OnDestroy()
		{

		}

		private void HandleShortCuts()
		{
			/*bool commandoKeyPressed = false;

			if (Application.platform == RuntimePlatform.OSXEditor) commandoKeyPressed = Event.current.command;
			else commandoKeyPressed = Event.current.control;*/
		}

		private void HandleTabButtons()
		{
			Color standardBackgroundColor = GUI.backgroundColor;
			int tabIndex = 0;
			GDICanvas canvasToClose = null;
			_hoveringTabIndex = -1;
			float xOffset = 0;
			for (int index = 0; index < _canvasList.Count; index++)
			{
				GDICanvas tmpGDICanvas = _canvasList[index];
				string tabName;

				if (string.IsNullOrEmpty(tmpGDICanvas.Graph.OriginFile)) tabName = Config.DefaultGraphName;
				else tabName = Path.GetFileName(tmpGDICanvas.Graph.OriginFile);

				float textWidth = GUI.skin.label.CalcSize(new GUIContent(tabName)).x + 10;

				float width = textWidth + TabButtonMargin + TabCloseButtonSize;

				tmpGDICanvas.TabButton.Set(xOffset, TopMenuHeight + TabButtonMargin, textWidth, TopMenuHeight);
				tmpGDICanvas.CloseTabButton.Set(xOffset + width - TabCloseButtonSize - TabButtonMargin - 4,
					TopMenuHeight + TabButtonMargin, TabCloseButtonSize, TabCloseButtonSize);

				xOffset = xOffset + width;

				bool isSelected = _currentGDICanvas == tmpGDICanvas;

				EditorGUI.DrawRect(tmpGDICanvas.TabButton, isSelected ? Config.SelectedTabColor : Config.TabColor);
				GUI.Label(tmpGDICanvas.TabButton, tabName);

				EditorGUI.DrawRect(tmpGDICanvas.CloseTabButton, isSelected ? Config.SelectedTabColor : Config.TabColor);
				GUI.Label(tmpGDICanvas.CloseTabButton, "X");

				if (tmpGDICanvas.TabButton.Contains(Event.current.mousePosition))
				{
					_hoveringTabIndex = index;
					if (Event.current.type == EventType.MouseUp)
					{
						SetCurrentGDICanvas(tmpGDICanvas);
					}
				}

				if (Event.current.type == EventType.MouseUp && tmpGDICanvas.CloseTabButton.Contains(Event.current.mousePosition))
				{
					canvasToClose = tmpGDICanvas;
				}

				tabIndex++;
			}

			GUI.backgroundColor = standardBackgroundColor;
			if (canvasToClose != null) 	CloseGDICanvas(canvasToClose);
		}


		private void SetCurrentGDICanvas(GDICanvas canvas)
		{
			UpdateGraphs();
			Repaint();
			if (canvas != null) EventManager.TriggerOnFocusGraph(canvas.Graph);
			_currentGDICanvas = canvas;
		}

		private void CloseGDICanvas(GDICanvas canvas)
		{
			bool doSave = EditorUtility.DisplayDialog("Do you want to save.", "Do you want to save the graph " + canvas.Graph.OriginFile + " ?",
				"Yes", "No");
			if (doSave)
			{
				if (canvas.Graph.OriginFile == null) OpenSaveDialog();
				else Launcher.Instance.SaveGraph(canvas.Graph, canvas.Graph.OriginFile);
			}
			EventManager.TriggerOnCloseGraph(canvas.Graph);
			Launcher.Instance.RemoveGraph(canvas.Graph, false);
			_canvasList.Remove(canvas);
			if (_canvasList.Count > 0) SetCurrentGDICanvas(_canvasList[0]);
			else SetCurrentGDICanvas(null);
		}

		private GenericMenu CreateNodeMenu()
		{
			GenericMenu m = new GenericMenu();
			foreach(KeyValuePair<string, Type> entry in _menuEntryToNodeType)
				m.AddItem(new GUIContent(entry.Key), false, OnGenericMenuClick, entry.Value);

			m.AddSeparator("");
			m.AddItem(new GUIContent("Paste"), false, OnPasteClipboard);
			return m;
		}

		private void OnPasteClipboard()
		{
			if (_currentGDICanvas != null) _currentGDICanvas.PasteFromClipboard();
		}


		private void OpenNodeMenu()
		{
			CreateNodeMenu().ShowAsContext();
		}

		private void OnGenericMenuClick(object item)
		{
			if (_currentGDICanvas != null)
			{
				Vector2 posisiton = _lastMousePosition;
				if (posisiton.y < 100) posisiton.y = 100;
				_currentGDICanvas.CreateNode((Type) item, posisiton);
			}
		}

		private void CreateGDICanvas(object path)
		{
			CreateGDICanvas((string) path);
		}

		private void CreateGDICanvas(string path)
		{
			GDICanvas canvas;
			if (path != null) canvas = new GDICanvas(Launcher.Instance.LoadGraph(path, false));
			else
			{
				Graph g = new Graph();
				canvas = new GDICanvas(g);
				Launcher.Instance.AddGraph(g, false);
			}

			canvas.Graph.OriginFile = path;
			_canvasList.Add(canvas);
			SetCurrentGDICanvas(canvas);
		}

		private void OpenSaveDialog()
		{
			string path = "";
			string fileName = Config.DefaultGraphName;
			if (!string.IsNullOrEmpty(_currentGDICanvas.Graph.OriginFile))
			{
				path = Path.GetFileName(_currentGDICanvas.Graph.OriginFile);
				fileName = Path.GetFileNameWithoutExtension(_currentGDICanvas.Graph.OriginFile);
			}

			path = EditorUtility.SaveFilePanel("save graph data", path, fileName, "json");
			if (!string.IsNullOrEmpty(path))
			{
				_currentGDICanvas.Graph.OriginFile = path;
				Launcher.Instance.SaveGraph(_currentGDICanvas.Graph, path);
			}
		}

		private void DrawToolbar()
		{
			EditorStyles.toolbarDropDown.fixedWidth = 60;
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			if (GUILayout.Button("Graph", EditorStyles.toolbarDropDown)) OpenGrapMenu();
			if (GUILayout.Button("Add", EditorStyles.toolbarDropDown)) OpenNodeMenu();
			if (GUILayout.Button("GDI", EditorStyles.toolbarDropDown)) OpenGDIMenu();
			GUILayout.FlexibleSpace();
			if (_currentGDICanvas != null) GUILayout.Label("nodes: " + _currentGDICanvas.Graph.GetNodeCount());
			if (Application.isPlaying) GUILayout.Label("Playmode");
			else GUILayout.Label("Editmode");
			GUILayout.EndHorizontal();
		}

		private void OpenGrapMenu()
		{
			GenericMenu m = new GenericMenu();
			m.AddItem(new GUIContent("New"), false, CreateGDICanvas, null);
			m.AddItem(new GUIContent("Open"), false, OpenGraph);
			m.AddItem(new GUIContent("Save"), false, OpenSaveDialog);
			m.ShowAsContext();
			Event.current.Use();
		}

		private void OpenGDIMenu()
		{
			GenericMenu m = new GenericMenu();
			m.AddItem(new GUIContent("About"), false, ShowAboutPanel);
			m.AddItem(new GUIContent("Preferences"), false, ShowPreferencesPanel);
			m.AddItem(new GUIContent("Documentation"), false, GDICanvas.OpenDocu);
			m.ShowAsContext();
			Event.current.Use();
		}

		private void ShowAboutPanel()
		{
			AboutPanel.Show = true;
		}

		private void ShowPreferencesPanel()
		{
			PreferencesPanel.Show = true;
		}

		private void OpenGraph()
		{
			var dir = Application.dataPath + Config.ExamplesPath;
			var path = EditorUtility.OpenFilePanel("load graph data", dir, "json");
			if (!path.Equals("")) CreateGDICanvas(path);
		}

		private void HandleGDICanvasTranslation()
		{
			if (_currentGDICanvas == null) return;

			// Zoom
			if (Event.current.type == EventType.ScrollWheel)
			{
				Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(Event.current.mousePosition);
				float zoomDelta = -Event.current.delta.y/150.0f;
				float oldZoom = _currentGDICanvas.Zoom;

				float nextZoom = _currentGDICanvas.Zoom + zoomDelta;
				//if (Mathf.Abs(1 - nextZoom) < 0.07) nextZoom = 1;

				_currentGDICanvas.Zoom = Mathf.Clamp(nextZoom, GDICanvasZoomMin, GDICanvasZoomMax);

				_nextTranlationPosition = _currentGDICanvas.Position + (zoomCoordsMousePos - _currentGDICanvas.Position) -
					(oldZoom/_currentGDICanvas.Zoom)*(zoomCoordsMousePos - _currentGDICanvas.Position);

				if (_nextTranlationPosition.x >= 0) _nextTranlationPosition.x = 0;
				if (_nextTranlationPosition.y >= 0) _nextTranlationPosition.y = 0;
				_currentGDICanvas.Position = _nextTranlationPosition;
				Event.current.Use();
				return;
			}

			// Translate
			if (Event.current.type == EventType.MouseDrag &&
			    (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
			    Event.current.button == 2)
			{
				Vector2 delta = Event.current.delta;
				delta /= _currentGDICanvas.Zoom * 2;

				_nextTranlationPosition = _currentGDICanvas.Position + delta;
				if (_nextTranlationPosition.x >= 0) _nextTranlationPosition.x = 0;
				if (_nextTranlationPosition.y >= 0) _nextTranlationPosition.y = 0;

				_currentGDICanvas.Position = _nextTranlationPosition;
				Event.current.Use();
			}
		}

		private void HandleSocketDrag(AbstractSocket dragSource)
		{
			if (dragSource != null)
			{
				if (dragSource.IsInput() && dragSource.IsConnected())
				{
					_dragSourceSocket = ((InputSocket) dragSource).Edge.GetOtherSocket(dragSource);
					_currentGDICanvas.Graph.UnLink((InputSocket) dragSource, (OutputSocket) _dragSourceSocket);
				}
				if (dragSource.IsOutput()) _dragSourceSocket = dragSource;
				Event.current.Use();
			}
			Repaint();
		}

		private void HandleSocketDrop(AbstractSocket dropTarget)
		{
			if (dropTarget != null && dropTarget.GetType() != _dragSourceSocket.GetType())
			{
				if (dropTarget.IsInput())
				{
					_currentGDICanvas.Graph.Link((InputSocket) dropTarget, (OutputSocket) _dragSourceSocket);
				}
				Event.current.Use();
			}
			_dragSourceSocket = null;
			Repaint();
		}

		private void HandleSocketDragAndDrop()
		{
			if (_currentGDICanvas == null) return;

			if (Event.current.type == EventType.MouseDown)
			{
				HandleSocketDrag(_currentGDICanvas.GetSocketAt(Event.current.mousePosition));
			}

			if (Event.current.type == EventType.MouseUp && _dragSourceSocket != null)
			{
				HandleSocketDrop(_currentGDICanvas.GetSocketAt(Event.current.mousePosition));
			}

			if (Event.current.type == EventType.MouseDrag)
			{
				if (_dragSourceSocket != null) Event.current.Use();
			}
		}

		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - _canvasRegion.TopLeft())/_currentGDICanvas.Zoom + _currentGDICanvas.Position;
		}

	}
}


