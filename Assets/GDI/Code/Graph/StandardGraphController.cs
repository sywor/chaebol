using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph
{
	/// <summary>
	/// This class registeres to graph events and updates the graph.
	/// </summary>
	public class StandardGraphController
	{

		public void Register()
		{
			EventManager.OnCreateGraph += OnCreate;
			EventManager.OnFocusGraph += OnFocus;
			EventManager.OnCloseGraph += OnClose;
			EventManager.OnLinkEdge += OnLink;
			EventManager.OnUnLinkSockets += OnUnLink;
			EventManager.OnUnLinkedSockets += OnUnLinked;
			EventManager.OnAddedNode += OnNodeAdded;
			EventManager.OnNodeRemoved += OnNodeRemoved;
			EventManager.OnChangedNode += OnNodeChanged;
			EventManager.OnFocusNode += OnFocusNode;
			EventManager.OnEditorWindowOpen += OnWindowOpen;
			EventManager.OnNodeRenamed += OnNodeRenamed;
			Log.Info("Registered " + GetType() + " to EventManager.");
		}

		private void OnWindowOpen()
		{

		}

		public void OnCreate(Assets.GDI.Code.Graph.Graph graph)
		{
			graph.UpdateNodes();
		}

		public void OnFocus(Assets.GDI.Code.Graph.Graph graph)
		{
			Log.Info("OnFocus " + graph);
		}

		public void OnClose(Assets.GDI.Code.Graph.Graph graph)
		{
			Log.Info("OnClose " + graph);
		}

		// ======= Events =======
		public void OnLink(Assets.GDI.Code.Graph.Graph graph, Edge edge)
		{
			Log.Info("OnLink: Node " + edge.Output.Parent.Id + " with Node " + edge.Input.Parent.Id);
			graph.UpdateDependingNodes(edge.Output.Parent);
		}

		public void OnUnLink(Assets.GDI.Code.Graph.Graph graph, AbstractSocket s01, AbstractSocket s02)
		{
			// Log.Info("OnUnLink: Node " + s01.Edge.Output.Parent.Id + " from Node " + s02.Edge.Input.Parent.Id);
		}

		public void OnUnLinked(Assets.GDI.Code.Graph.Graph graph, AbstractSocket s01, AbstractSocket s02)
		{
			Log.Info("OnUnLinked: Socket " + s02 + " and Socket " + s02);
			var input = s01.IsInput () ? s01 : s02;
			graph.UpdateDependingNodes(input.Parent);
		}

		public void OnNodeAdded(Assets.GDI.Code.Graph.Graph graph, Node node)
		{
			Log.Info("OnNodeAdded: Node " + node.GetType() + " with id " + node.Id);
		}

		public void OnNodeRemoved(Assets.GDI.Code.Graph.Graph graph, Node node)
		{
			Log.Info("OnNodeRemoved: Node " + node.GetType() + " with id " + node.Id);
		}

		public void OnNodeChanged(Assets.GDI.Code.Graph.Graph graph, Node node)
		{
			Log.Info("OnNodeChanged: Node " + node.GetType() + " with id " + node.Id);
			graph.UpdateDependingNodes(node);
		}

		public void OnFocusNode(Assets.GDI.Code.Graph.Graph graph, Node node)
		{
			Log.Info("OnFocus: " + node.Id);
		}

		public void OnNodeRenamed(Assets.GDI.Code.Graph.Graph graph, Node node, string oldName, string newName)
		{
			Log.Info("OnRename: " + node.Id + " from " + oldName + " to " + newName);
		}
	}
}
