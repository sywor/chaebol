using System;
using System.Collections.Generic;
using System.IO;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph
{
    /// <summary>
    /// This class implements the logic of a graph. It contains nodes and can be serialized
    /// as a json file. It contains methods to deal with nodes such as create, add, remove, link etc.
    /// </summary>
    [Serializable]
    public class Graph : ISerializationCallbackReceiver
    {
        [SerializeField] public string Name;
        [SerializeField] public string OriginFile;

        private List<Node> _nodes = new List<Node>();

        [HideInInspector] [SerializeField]
        private List<SerializableEdge> _serializedEdges = new List<SerializableEdge>();

        [HideInInspector] [SerializeField]
        private List<SerializableNode> _serializedNodes = new List<SerializableNode>();

        [SerializeField] public string Version = Config.Version;

        // be warned to allow circles.. if you parse the graph you can end up in
        // an endless recursion this can crash unity.
        [HideInInspector] [SerializeField] public bool AllowCicles = false;

        private bool _invalidating;

        private bool _needsUpdate = true;
        [NonSerialized] public bool TriggerEvents = true;

        /// <summary>Returns an id for a Node that is unique for this Graph.</summary>
        /// <returns> An id for a Node that is unique for this Graph.</returns>
        public int ObtainUniqueNodeId()
        {
            var tmpId = 0;
            while (GetNode(tmpId) != null) tmpId++;
            return tmpId;
        }

        /// <summary>Returns an id for a Node that is unique for this Graph and the assigned nodes.</summary>
        /// <param name="nodes">Additional nodes list.</param>
        /// <returns> An id for a Node that is unique for this Graph.</returns>
        public int ObtainUniqueNodeId(List<Node> nodes)
        {
            var tmpId = 0;
            while (GetNode(tmpId) != null || ContainsNode(nodes, tmpId)) tmpId++;
            return tmpId;
        }


        /// <summary>Creates a Node of the given type. Type must inherit from Node.
        /// Does not add the Node to the Graph.</summary>
        /// <returns>The created Node of the given Type.</returns>
        public Node CreateNode<T>()
        {
            return CreateNode<T>(ObtainUniqueNodeId());
        }

        /// <summary>Creates a Node of the given type with the assigned id. Type must inherit from Node.
        /// Does not add the Node to the Graph.</summary>
        /// <returns>The created Node of the given Type with the assigned id.</returns>
        public Node CreateNode<T>(int id)
        {
            return CreateNode(typeof(T), id);
        }

        /// <summary>Creates a Node of the given type. Type must inherit from Node.
        /// Does not add the Node to the Graph.</summary>
        /// <param name="type">The Type of the Node to create.</param>
        /// <returns>The created Node of the given Type.</returns>
        public Node CreateNode(Type type)
        {
            return CreateNode(type, ObtainUniqueNodeId());
        }

        /// <summary>Creates a Node of the given type with the assigned id. Type must inherit from Node.
        /// Does not add the Node to the Graph.</summary>
        /// <param name="type">The Type of the Node to create.</param>
        /// <param name="id">The id of the Node to create.</param>
        /// <returns>The created Node of the given Type with the assigned id.</returns>
        public Node CreateNode(Type type, int id)
        {
            if (type == null) return null;
            _needsUpdate = true;
            try
            {
                return (Node) Activator.CreateInstance(type, id, this);
            }
            catch (Exception exception)
            {
                Debug.LogErrorFormat("Node {0} could not be created " + exception.Message, type.FullName);
            }
            return null;
        }

        /// <summary>Returns the Node with the assigned id or null.</summary>
        /// <param name="nodeId">The id of the Node to get.</param>
        /// <returns>The Node with the assigned id or null.</returns>
        public Node GetNode(int nodeId)
        {
            return GetNode(nodeId, _nodes);
        }

        public Node GetNode(int nodeId, List<Node> nodes)
        {
            if (nodes == null) return null;
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                if (node.Id == nodeId) return node;
            }
            return null;
        }


        /// <summary>Returns the first node with the assigned type or null.</summary>
        ///<param name="nodeType">The type of the node to get.</param>
        ///<returns>The first node with the assigned type or null.</returns>
        public Node GetFirstNodeWithType(Type nodeType)
        {
            if (_nodes == null) return null;
            for (int index = 0; index < _nodes.Count; index++)
            {
                var node = _nodes[index];
                if (node.GetType() == nodeType) return node;
            }
            return null;
        }

        /// <summary>Returns the count of Nodes in this Graph.</summary>
        /// <returns>The count of Nodes in this Graph.</returns>
        public int GetNodeCount()
        {
            return _nodes.Count;
        }

        public int GetConnectedSocketCount()
        {
            int count = 0;
            for (int index = 0; index < _nodes.Count; index++)
            {
                var node = _nodes[index];
                count += node.GetEdgeCount();
            }
            return count;
        }

        /// <summary>Returns the Node at the assigned index.</summary>
        /// <param name="index">The index of the Node to get.</param>
        /// <returns>The Node at the assigned index.</returns>
        public Node GetNodeAt(int index)
        {
            if (index >= _nodes.Count) return null;
            return _nodes[index];
        }

        /// <summary>Adds a node to the Graph. Does not add Nodes with an id that is already taken.
        /// Triggers a 'AddedNode' event. </summary>
        /// <param name="node">The Node to add.</param>
        /// <returns>True if the node was added.</returns>
        public bool AddNode(Node node)
        {
            if (GetNode(node.Id) != null) return false;
            _needsUpdate = true;
            _nodes.Add(node);
            if (TriggerEvents) EventManager.TriggerOnAddedNode(this, node);
            return true;
        }

        public void AddNodes(List<Node> nodes)
        {
            if (nodes != null)
            {
                for (var i = 0; i < nodes.Count; i++) AddNode(nodes[i]);
            }
        }

        /// <summary>Removes the assigned Node from this Graph.</summary>
        /// <param name="node">The Node to remove.</param>
        /// <returns>True if the Node was removed.</returns>
        public bool RemoveNode(Node node)
        {
            if (node == null) return false;
            _needsUpdate = true;

            for (int index = 0; index < node.Sockets.Count; index++)
            {
                var socket = node.Sockets[index];
                UnLink(socket);
            }
            bool removed = _nodes.Remove(node);
            if (TriggerEvents) EventManager.TriggerOnNodeRemoved(this, node);
            return removed;
        }

        /// <summary>Removes the Node with the assigned id from this Graph.</summary>
        /// <param name="id">The id of the Node to remove.</param>
        /// <returns>True if the Node was removed.</returns>
        public bool RemoveNode(int id)
        {
            return RemoveNode(GetNode(id));
        }

        public bool AreConected(InputSocket inputSocket, OutputSocket outputSocket)
        {
            if (inputSocket == null || outputSocket == null || inputSocket.Edge == null ||
                outputSocket.Edges.Count == 0) return false;
            return outputSocket.Edges.Contains(inputSocket.Edge);
        }

        /// <summary>Unlinkes the assigned sockets. Triggeres 'Unlink' events.</summary>
        public void UnLink(InputSocket inputSocket, OutputSocket outputSocket)
        {
            _needsUpdate = true;

            if (inputSocket == null || outputSocket == null || !inputSocket.IsConnected() ||
                !outputSocket.IsConnected()) return;
            if (!AreConected(inputSocket, outputSocket)) return;

            if (TriggerEvents)
            {
                EventManager.TriggerOnUnLinkSockets(this, inputSocket, outputSocket);
            }

            var index = outputSocket.Edges.IndexOf(inputSocket.Edge);
            if (index > -1)
            {
                outputSocket.Edges[index].Input = null;
                outputSocket.Edges[index].Output = null;
                outputSocket.Edges.RemoveAt(index);
            }

            inputSocket.Edge.Input = null;
            inputSocket.Edge.Output = null;
            inputSocket.Edge = null;

            if (TriggerEvents)
            {
                EventManager.TriggerOnUnLinkedSockets(this, inputSocket, outputSocket);
            }
        }

        public void UnLink(AbstractSocket socket)
        {
            if (socket == null || !socket.IsConnected()) return;


            if (socket.IsInput())
            {
                InputSocket inputSocket = (InputSocket) socket;
                if (inputSocket.Edge != null) UnLink(inputSocket, inputSocket.Edge.Output);
            }

            if (socket.IsOutput())
            {
                OutputSocket outputSocket = (OutputSocket) socket;
                Edge[] edgeCopy = new Edge[outputSocket.Edges.Count];
                outputSocket.Edges.CopyTo(edgeCopy);
                for (int index = 0; index < edgeCopy.Length; index++)
                {
                    var edge = edgeCopy[index];
                    UnLink(edge.Input, outputSocket);
                }
            }
        }

        public bool Link(InputSocket inputSocket, OutputSocket outputSocket)
        {
            if (!CanBeLinked(inputSocket, outputSocket))
            {
                Debug.LogWarning("Sockets can not be linked.");
                return false;
            }
            _needsUpdate = true;


            Edge edge = new Edge(outputSocket, inputSocket);
            Edge oldEdge = inputSocket.Edge;
            inputSocket.Edge = edge;
            outputSocket.Edges.Add(edge);

            if (!AllowCicles && HasCycle())
            {
                // revert
                inputSocket.Edge = oldEdge;
                outputSocket.Edges.Remove(edge);
                Log.Info("Can not link sockets. Circles are not allowed.");
                return false;
            }

            if (TriggerEvents)
            {
                EventManager.TriggerOnLinkEdge(this, edge);
            }

            return true;
        }

        private void StartVisitRun()
        {
            if (_invalidating)
            {
                throw new UnityException("Graph is already invalidating");
            }

            _invalidating = true;
            ResetVisitCount();
        }

        public void ResetVisitCount()
        {
            for (int index = 0; index < _nodes.Count; index++)
            {
                var node = _nodes[index];
                node.VisitCount = 0;
            }
        }

        private void EndVisitRun()
        {
            _invalidating = false;
        }


        public bool HasCycle()
        {
            for (int index = 0; index < _nodes.Count; index++)
            {
                var node = _nodes[index];
                StartVisitRun(); // resets visit counter
                bool foundCicle = IsConnectedToItself(node);
                EndVisitRun();
                if (foundCicle) return true;
            }
            return false;
        }

        /// <summary>
        /// This method returns true is a node is connected to itself along the output path.
        /// Reset the 'VisitCount' counter of each node before calling! Call this
        /// method ignoring the optional parameter. This is used internaly for recursion.
        /// <param name="searchForNode">The node you want to check</param>
        /// <param name="recursionNode">Do not use this parameter or assign null</param>
        /// </summary>
        private bool IsConnectedToItself(Node searchForNode, Node recursionNode = null)
        {
            bool firstRun = recursionNode == null;
            if (firstRun) recursionNode = searchForNode;
            if (!firstRun && recursionNode == searchForNode) return true; // visited the first node after recursion
            if (recursionNode.VisitCount > 0) return false; // already checked
            recursionNode.VisitCount++;

            for (int index = 0; index < recursionNode.Sockets.Count; index++)
            {
                var socket = recursionNode.Sockets[index];
                if (socket.IsOutput() && socket.IsConnected())
                {
                    OutputSocket outputSocket = (OutputSocket) socket;
                    for (int i = 0; i < outputSocket.Edges.Count; i++)
                    {
                        var edge = outputSocket.Edges[i];
                        if (edge.Input.Parent != null && IsConnectedToItself(searchForNode, edge.Input.Parent))
                            return true;
                    }
                }
            }
            return false;
        }

        public void UpdateDependingNodes(Node node)
        {
            Log.Info("Update Depending Nodes.");
            StartVisitRun();
            UpdateOutputPath(node);
            EndVisitRun();
        }

        /// <summary>
        /// This method follows the ouput path of the node and updates all visited nodes.
        /// Reset the 'VisitCount' counter of each node before calling!
        /// </summary>
        /// <param name="node">The node to update and its ouput path nodes</param>
        private void UpdateOutputPath(Node node)
        {
            if (node.VisitCount > 0) return; // already updated
            node.Update();
            node.VisitCount++;

            for (int index = 0; index < node.Sockets.Count; index++)
            {
                var socket = node.Sockets[index];
                if (socket.IsOutput() && socket.IsConnected())
                {
                    OutputSocket outputSocket = (OutputSocket) socket;
                    for (int i = 0; i < outputSocket.Edges.Count; i++)
                    {
                        var edge = outputSocket.Edges[i];
                        UpdateOutputPath(edge.Input.Parent);
                    }
                }
            }
        }


        /// <summary> Returns true if the sockets can be linked.</summary>
        /// <param name="inSocket"> The input socket</param>
        /// <param name="outSocket"> The output socket</param>
        /// <returns>True if the sockets can be linked.</returns>
        public bool CanBeLinked(InputSocket inSocket, OutputSocket outSocket)
        {
            return inSocket != null && outSocket != null && 
                   (outSocket.Type == inSocket.Type || inSocket.Type.BaseType == outSocket.Type.BaseType);
        }

        public void LogCircleError()
        {
            Debug.LogError("The graph contains ciclyes.");
        }

        // === SERIALIZATION ===

        /// <summary>
        /// Creates a clone of this graph.
        ///</summary>
        ///<returns>A clone of this graph.</returns>
        public Graph Clone()
        {
            var json = JsonUtility.ToJson(this);
            return FromJson(json);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static Graph FromJson(string json)
        {
            Graph g = JsonUtility.FromJson<Graph>(json);
            //listener.OnCreate(g);
            EventManager.TriggerOnCreateGraph(g);
            return g;
        }

        public static bool Save(string fileName, Graph graph)
        {
            var file = File.CreateText(fileName);
            file.Write(graph.ToJson());
            file.Close();
            return true;
        }

        public static Graph Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                var file = File.OpenText(fileName);
                var json = file.ReadToEnd();
                file.Close();
                Graph deserializedGraph = FromJson(json);
                if (deserializedGraph.Version != Config.Version)
                {
                    Debug.LogWarning("You loading a graph with a different version number: " +
                                     deserializedGraph.Version +
                                     " the current version is " + Config.Version);
                }
                return deserializedGraph;
            }
            else
            {
                Debug.Log("Could not Open the file: " + fileName);
                return null;
            }
        }

        public void UpdateNodes()
        {
            if (!_needsUpdate) return;

            for (int index = 0; index < _nodes.Count; index++)
            {
                var node = _nodes[index];
                node.Update();
            }
            _needsUpdate = false;
        }

        public void ForceUpdateNodes()
        {
            _needsUpdate = true;
            UpdateNodes();
        }

        public void UpdateEdgeBounds()
        {
            for (var index = 0; index < _nodes.Count; index++)
            {
                _nodes[index].UpdateEdgeBounds();
            }
        }

        /// <summary>Unity serialization callback.</summary>
        public void OnBeforeSerialize()
        {
            if (_nodes.Count == 0) return; // nothing to serialize


            _serializedEdges.Clear();
            _serializedNodes.Clear();
            // serialize data
            SerializationContainer container = Serialize(_nodes);
            if (container != null)
            {
                _serializedEdges = container.Edges;
                _serializedNodes = container.Nodes;
            }
        }

        public void SetNewIds(List<Node> nodes)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].Id = ObtainUniqueNodeId(nodes);
            }
        }

        public static bool ContainsNode(List<Node> nodes, int id)
        {
            if (nodes == null) return false;
            for (var i = 0; i < nodes.Count; i++) if (nodes[i].Id == id) return true;
            return false;
        }

        public void MoveNodes(List<Node> nodes, Vector2 offset)
        {
            if (nodes != null)
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    nodes[i].WindowRect.x += offset.x;
                    nodes[i].WindowRect.y += offset.y;
                }
                UpdateEdgeBounds();
            }
        }

        public SerializationContainer Serialize(List<Node> nodes)
        {
            if (nodes == null) return null;
            bool wasTriggering = TriggerEvents;
            TriggerEvents = false;

            SerializationContainer container = new SerializationContainer();

            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                container.Nodes.Add(node.ToSerializedNode());
                for (var i = 0; i < node.Sockets.Count; i++)
                {
                    var socket = node.Sockets[i];
                    if (socket.IsInput() && socket.IsConnected())
                        // serialize only input socket edges to avoid double edge serialization
                    {
                        InputSocket inputSocket = (InputSocket) socket;
                        container.Edges.Add(inputSocket.Edge.ToSerializedEgde());
                    }
                }
            }
            TriggerEvents = wasTriggering;
            return container;
        }


        public List<Node> Deserialize(List<SerializableNode> nodes, List<SerializableEdge> edges)
        {
            if (nodes.Count == 0) return null; // Nothing to deserialize.
            List<Node> returnValue = new List<Node>();

            bool wasTriggering = TriggerEvents;
            TriggerEvents = false;

            // deserialize nodes
            for (var index = 0; index < nodes.Count; index++)
            {
                var sNode = nodes[index];
                Type nodeType = Type.GetType(sNode.type);
                if (nodeType == null)
                {
                    Debug.LogWarning("Unknown node type: " + sNode.type);
                    continue;
                }
                Node n = CreateNode(nodeType, sNode.id);
                if (n != null)
                {
                    JsonUtility.FromJsonOverwrite(sNode.data, n);
                    n.OnDeserialization(sNode);
                    n.X = sNode.X;
                    n.Y = sNode.Y;

                    for (var i = 0; i < sNode.directInputValues.Length; i++)
                    {
                        if (i < n.Sockets.Count)
                        {
                            if (n.Sockets[i].IsInput())
                            {
                                InputSocket inputSocket = (InputSocket) n.Sockets[i];
                                inputSocket.SetDirectInputNumber(sNode.directInputValues[i], false);
                            }
                        }
                    }

                    if (sNode.Collapsed) n.Collapse();
                    returnValue.Add(n);
                }
            }

            // deserialize edges
            for (var index = 0; index < edges.Count; index++)
            {
                var sEdge = edges[index];
                Node inputNode = GetNode(sEdge.InputNodeId, returnValue);
                Node outputNode = GetNode(sEdge.OutputNodeId, returnValue);
                if (inputNode == null || outputNode == null)
                {
                    Log.Error("Try to create an edge but can not find the nodes.");
                    continue;
                }

                if (sEdge.OutputSocketIndex > outputNode.Sockets.Count ||
                    sEdge.InputSocketIndex > inputNode.Sockets.Count)
                {
                    Log.Error("Try to create an edge but can not find the sockets.");
                    continue;
                }

                if (sEdge.InputSocketIndex < inputNode.Sockets.Count &&
                    sEdge.OutputSocketIndex < outputNode.Sockets.Count)
                {
                    if (inputNode.Sockets[sEdge.InputSocketIndex].GetType() == typeof(InputSocket)
                        && outputNode.Sockets[sEdge.OutputSocketIndex].GetType() == typeof(OutputSocket))
                    {
                        InputSocket inputSocket = (InputSocket) inputNode.Sockets[sEdge.InputSocketIndex];
                        OutputSocket outputSocket = (OutputSocket) outputNode.Sockets[sEdge.OutputSocketIndex];
                        Edge edge = new Edge(outputSocket, inputSocket);
                        edge.Path = sEdge.Path;
                        inputSocket.Edge = edge;
                        outputSocket.Edges.Add(edge);
                    }
                    else
                    {
                        Log.Error("Can not connect node " + inputNode.Id + " with node " + outputNode.Id +
                                  ". Socket type error.");
                    }
                }
                else
                {
                    Log.Error("Can not connect node " + inputNode.Id + " with node " + outputNode.Id +
                              ". Socket index out of range.");
                }
            }
            TriggerEvents = wasTriggering;
            return returnValue;
        }


        /// <summary>Unity serialization callback.</summary>
        public void OnAfterDeserialize()
        {
            if (_serializedNodes.Count == 0) return; // Nothing to deserialize.
            _nodes.Clear(); // clear original data.
            _nodes.AddRange(Deserialize(_serializedNodes, _serializedEdges));
        }
    }

    [Serializable]
    public class SerializationContainer
    {
        [SerializeField] public List<SerializableEdge> Edges = new List<SerializableEdge>();
        [SerializeField] public List<SerializableNode> Nodes = new List<SerializableNode>();
    }
}