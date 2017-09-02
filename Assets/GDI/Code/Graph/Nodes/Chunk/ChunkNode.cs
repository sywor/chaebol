using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Chunk
{
	[Serializable]
	[GraphContextMenuItem("Chunk", "Chunk")]
	public class ChunkNode : AbstractChunkNode
	{

		[SerializeField] private int _landscapeCount;
		[SerializeField] private int _entitiesCount;

		private Rect _tmpRect;

		public ChunkNode(int id, Graph parent) : base(id, parent)
		{
			Sockets.Add(new OutputSocket(this, typeof(IChunkConnection)));
			SocketTopOffsetInput = 50;
			Width = 130;
		}

		public override void OnDeserialization(SerializableNode sNode)
		{
			int serializedCount = _landscapeCount;
			_landscapeCount = 0;
			while (_landscapeCount < serializedCount) AddLandscapeSocket();
			serializedCount = _entitiesCount;
			_entitiesCount = 0;
			while (_entitiesCount < serializedCount) AddGameObjectsSocket();
		}

		private void AddLandscapeSocket()
		{
			var landscapeSocket = new InputSocket(this, typeof(ILandscapeConnection));
			Sockets.Insert(GetNextLandscapeSocketIndex(), landscapeSocket);
			_landscapeCount++;
		}

		private void AddGameObjectsSocket()
		{
			var gameObjectsSocket = new InputSocket(this, typeof(IEntitiesConnection));
			Sockets.Insert(GetNextGameObjectsSocketIndex(), gameObjectsSocket);
			_entitiesCount++;
		}

		private void RemoveSocket(AbstractSocket socket)
		{
			Sockets.Remove(socket);
			UpdateSocketCounts();
			TriggerChangeEvent();
		}

		private int GetNextLandscapeSocketIndex()
		{
			// after the last landscape:
			for (int i = Sockets.Count - 1; i >= 0; i--) if (Sockets[i].Type == typeof(ILandscapeConnection)) return i + 1;

			// before the first game objects
			for (int i = 0; i < Sockets.Count; i++) if (Sockets[i].Type == typeof(IEntitiesConnection)) return i;

			// at the end of the sockets
			return Sockets.Count;
		}

		private int GetNextGameObjectsSocketIndex()
		{

			// after the last game object:
			for (int i = Sockets.Count - 1; i >= 0; i--) if (Sockets[i].Type == typeof(IEntitiesConnection)) return i + 1;

			// after the last landscape:
			for (int i = Sockets.Count - 1; i >= 0; i--) if (Sockets[i].Type == typeof(ILandscapeConnection)) return i + 1;

			// at the end of the sockets
			return Sockets.Count;
		}

		private void UpdateSocketCounts()
		{
			_landscapeCount = 0;
			_entitiesCount = 0;
			for (int i = 0; i < Sockets.Count; i++)
			{
				AbstractSocket s = Sockets[i];
				if (s.Type == typeof(ILandscapeConnection)) _landscapeCount++;
				if (s.Type == typeof(IEntitiesConnection)) _entitiesCount++;
			}
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			Height = SocketTopOffsetInput + (_entitiesCount + _landscapeCount) * 20 + 25;

			_tmpRect.Set(3, 0, 120, 20);
			if (GUI.Button(_tmpRect, "add landscape"))
			{
				AddLandscapeSocket();
				TriggerChangeEvent();
			}

			_tmpRect.Set(3, 20, 120, 20);
			if (GUI.Button(_tmpRect, "add entities"))
			{
				AddGameObjectsSocket();
				TriggerChangeEvent();
			}

			float topOffset = SocketTopOffsetInput;

			InputSocket removeSocket = null;
			for (int i = 0; i < Sockets.Count; i++)
			{
				AbstractSocket socket = Sockets[i];
				if (socket.Type == typeof(ILandscapeConnection) || socket.Type == typeof(IEntitiesConnection) && socket.IsInput())
				{
					InputSocket s = socket as InputSocket;
					if (s.IsConnected())
					{
						_tmpRect.Set(3, topOffset, 100, 20);
						Node connectedNode = s.Edge.GetOtherSocket(s).Parent;
						GUI.Label(_tmpRect, connectedNode.Name + " (" + connectedNode.Id + ")");
					}
					else
					{
						_tmpRect.Set(3, topOffset, 20, 20);
						if (GUI.Button(_tmpRect, "x"))
						{
							removeSocket = s;
						}
					}
					topOffset += 20;
				}
			}

			if (removeSocket != null)
			{
				RemoveSocket(removeSocket);
			}
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public override Code.Chunk.Chunk GetChunk(OutputSocket outSocket, Request request)
		{
			Code.Chunk.Chunk chunk = new Code.Chunk.Chunk((int) request.X, (int) request.Z, (int) request.SizeX, (int) request.SizeZ, request.Seed);
			for (int i = 0; i < Sockets.Count; i++)
			{
				if (Sockets[i].IsInput() && Sockets[i].IsConnected())
				{
					InputSocket inputSocket = Sockets[i] as InputSocket;
					Node connectedNode = inputSocket.Edge.GetOtherSocket(inputSocket).Parent;
					if (inputSocket.Type == typeof(ILandscapeConnection)) chunk.AddLandscape((LandscapeNode) connectedNode);
					if (inputSocket.Type == typeof(IEntitiesConnection)) chunk.AddEntities((EntitiesNode) connectedNode);
				}
			}
			return chunk;
		}


	}
}
