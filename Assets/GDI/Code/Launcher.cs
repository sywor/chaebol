using System.Collections.Generic;
using Assets.GDI.Code.Graph;
using UnityEngine;

namespace Assets.GDI.Code
{
	/// <summary>
	/// A class to controll the creation of Graphs. It contains loaded Grpahs.
	/// (A gameobject with this script is created by the editor if it is not in the scene)
	/// </summary>
	[ExecuteInEditMode]
	[System.Serializable]
	public class Launcher : MonoBehaviour
	{
		private static Launcher _instance = null;

		private List<Graph.Graph> _graphs = new List<Graph.Graph>();
		private StandardGraphController _controller;

		/// <summary>
		/// Loads a graph by its path, adds it to the internal list
		/// and returns it.
		/// <param name="path">The path to load the graph.</param>
		///<param name="triggerEvent">Optional. For internal usage.</param>
		/// </summary>
		public Graph.Graph LoadGraph(string path, bool triggerEvent = true)
		{
			Graph.Graph g;
			//if (path.Equals(Config.DefaultGraphName)) g = CreateDefaultGraph();
			g = Graph.Graph.Load(path);
			g.Name = path;
			g.OriginFile = path;
			AddGraph(g, triggerEvent);
			CreateGraphController(g);
			g.UpdateNodes();
			return g;
		}

		/// <summary>
		/// Saves a graph by its path.
		/// </summary>
		public void SaveGraph(Graph.Graph g, string path)
		{
			g.OriginFile = path;
			Graph.Graph.Save(path, g);
		}

		/// <summary>
		/// Removes a Graph from the internal list.
		///<param name="triggerEvent">Optional. For internal usage.</param>
		/// </summary>
		public void RemoveGraph(Graph.Graph g, bool triggerEvent = true)
		{
			_graphs.Remove(g);
			if (triggerEvent) EventManager.TriggerOnGraphRemoved(g);
		}

		/// <summary>
		/// Returns the graph at the index
		/// </summary>
		/// <returns>The graph or null.</returns>
		public Graph.Graph GetGraph(int index)
		{
			if (_graphs != null && index < _graphs.Count) return _graphs[index];
			return null;
		}

		/// <summary>
		/// Clears the graphs.
		///<param name="triggerEvent">Optional. For internal usage.</param>
		///</summary>
		public void ClearGraphs(bool triggerEvent = true)
		{
			while (_graphs.Count > 0)
			{
				RemoveGraph(_graphs[0], triggerEvent);
			}
		}

		/// <summary>
		/// Creates a list of cloned graphs.
		///</summary>
		///<returns>A list of cloned graphs.</returns>
		public List<Graph.Graph> CloneGraphs()
		{
			List<Graph.Graph> clones = new List<Graph.Graph>();
			for (int i = 0; i < _graphs.Count; i++)
			{
				var clone = _graphs[i].Clone();
				clones.Add(clone);
			}
			return clones;
		}

		/// <summary>
		/// Returns the count of graphs.
		///</summary>
		///<returns>The count of graphs.</returns>
		public int GetGraphCount()
		{
			if (_graphs != null) return _graphs.Count;
			return 0;
		}

		/// <summary>
		/// Adds the graph to the launcher.
		///</summary>
		///<param name="graph">The graph to add.</param>
		///<param name="triggerEvent">Optional. For internal usage.</param>
		public void AddGraph(Graph.Graph graph, bool triggerEvent = true)
		{
			if (graph != null)
			{
				_graphs.Add(graph);
				if (triggerEvent) EventManager.TriggerOnGraphAdded(graph);
			}
		}

		/// <summary>
		/// Create a controller for the assigned Graph.
		/// </summary>
		private void CreateGraphController(Graph.Graph graph)
		{
			// in this case we create one controller for all graphs
			// you could also create different controllers for different graphs
			//if (_controller == null) _controller = new StandardGraphController();
		}

		public void OnEnable()
		{
			if (_controller == null) _controller = new StandardGraphController();
			_controller.Register();
		}

		/// <summary>
		/// Gets the instance of the lauchner. Creates the gameobject if it does not exist.
		///</summary>
		public static Launcher Instance
		{
			get
			{
				if (_instance != null) return _instance;

				if (GameObject.Find(Config.GameObjectName) == null)
				{
					new GameObject(Config.GameObjectName);
					Log.Info("Created GameObject '" + Config.GameObjectName + "'");
				}
				if (GameObject.Find(Config.GameObjectName).GetComponent<Launcher>() == null)
				{
					Log.Info("Added Launcher component to the GameObject '" + Config.GameObjectName + "'");
					GameObject.Find(Config.GameObjectName).AddComponent<Launcher>();
				}
				return GameObject.Find(Config.GameObjectName).GetComponent<Launcher>();
			}
		}

	}
}


