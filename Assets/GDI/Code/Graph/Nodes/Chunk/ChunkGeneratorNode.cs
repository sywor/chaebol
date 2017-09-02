using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Nodes.GameObject;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Chunk
{
	[Serializable]
	[GraphContextMenuItem("Chunk", "Chunk Generator")]
	public class ChunkGeneratorNode : Node
	{

		[SerializeField] private bool _autoUpdate;

		private InputSocket _inputSocketChunk;
		private InputSocket _inputSocketSize;
		private InputSocket _inputSocketChunkRadius;
		private InputSocket _inputSocketSeed;

		private InputSocket _inputSocketRequestCenter;

		private Rect _tmpRext;

		private Vector2 _requestCenterChunk;
		private UnityEngine.GameObject _chunkContainer;

		private UnityEngine.GameObject _requestObject;

		private bool initialUpdateDone;

		private List<Code.Chunk.Chunk> _chunks = new List<Code.Chunk.Chunk>();
		private List<Code.Chunk.Chunk> _removeList = new List<Code.Chunk.Chunk>();

		public ChunkGeneratorNode(int id, Graph parent) : base(id, parent)
		{
			_inputSocketChunk = new InputSocket(this, typeof(IChunkConnection));
			_inputSocketSize = new InputSocket(this, typeof(INumberConnection));
			_inputSocketSize.SetDirectInputNumber(10, false);
			_inputSocketChunkRadius = new InputSocket(this, typeof(INumberConnection));
			_inputSocketChunkRadius.SetDirectInputNumber(7, false);
			_inputSocketSeed = new InputSocket(this, typeof(INumberConnection));
			_inputSocketRequestCenter = new InputSocket(this, typeof(IGameObjectsConnection));

			Sockets.Add(_inputSocketChunk);
			Sockets.Add(_inputSocketSize);
			Sockets.Add(_inputSocketChunkRadius);
			Sockets.Add(_inputSocketSeed);
			Sockets.Add(_inputSocketRequestCenter);
			Height = 120;
			Width = 200;
		}

		protected override void OnGUI()
		{
			if (!initialUpdateDone)
			{
				Update();
				initialUpdateDone = true;
			}

			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			_tmpRext.Set(3, 0, 80, 20);
			GUI.Label(_tmpRext, "chunk");
			_tmpRext.Set(3, 20, 80, 20);
			GUI.Label(_tmpRext, "chunk size");
			_tmpRext.Set(3, 40, 80, 20);
			GUI.Label(_tmpRext, "req. radius");
			_tmpRext.Set(3, 60, 80, 20);
			GUI.Label(_tmpRext, "seed");
			_tmpRext.Set(3, 80, 100, 20);
			GUI.Label(_tmpRext, "request center");

			_tmpRext.Set(90, 3, 70, 20);
			if (GUI.Button(_tmpRext, "Update"))
			{
				_requestCenterChunk.Set(float.NaN, float.NaN);
				Update();
				RequestChunks(GetRequestCenter());
			}
			_tmpRext.Set(90, 23, 70, 20);
			if (GUI.Button(_tmpRext, "Clear")) ClearChunkContainer();

			_tmpRext.Set(90, 43, 90, 20);
			bool newAutoUpdate = GUI.Toggle(_tmpRext, _autoUpdate, "auto update");
			if (newAutoUpdate != _autoUpdate)
			{
				_autoUpdate = newAutoUpdate;
				Update();
				TriggerChangeEvent();
			}

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			UpdateChunks();
			if (_autoUpdate)
			{
				RequestChunks(GetRequestCenter());
			}
		}

		public override void Update()
		{
			_requestObject = GameObjectNode.GetInputGameObject(_inputSocketRequestCenter, new Request());
		}

		public UnityEngine.Vector3 GetRequestCenter()
		{
			if (_requestObject != null)
			{
				UnityEngine.Vector3 v = new UnityEngine.Vector3();
				v.Set(_requestObject.transform.position.x, _requestObject.transform.position.y, _requestObject.transform.position.z);
				return v;
			}
			return new UnityEngine.Vector3();

		}

		/// <summary>
		///	Calls the update method of all chunks. This is needed to run the chunks creation threads.
		///	Call this method on every update loop.
		/// </summary>
		public void UpdateChunks()
		{
			for (int i = 0; i < _chunks.Count; i++) _chunks[i].Update();
		}

		/// <summary>
		/// Requests the chunks relative to the assigned parameter.
		/// Does not request chunks that are already loaded.
		/// Does not request if no new chunks are in sight.
		/// </summary>
		/// <param name="requestCenterWorld">The position of the request center in world coordinates.</param>
		public void RequestChunks(UnityEngine.Vector3 requestCenterWorld)
		{
			float seed = AbstractNumberNode.GetInputNumber(_inputSocketSeed, new Request());
			var request = new Request();
			request.Seed = seed;
			int size = Mathf.CeilToInt(AbstractNumberNode.GetInputNumber(_inputSocketSize, request));
			int radius = Mathf.CeilToInt(AbstractNumberNode.GetInputNumber(_inputSocketChunkRadius, request));
			if (float.IsNaN(size) || float.IsNaN(radius) || size < 1 || radius < 1) return;

			Vector2 newCenterChunk = ToChunkPosition(requestCenterWorld.x, requestCenterWorld.z, size, size);

			bool hasMoved = !_requestCenterChunk.Equals(newCenterChunk);

			if (!hasMoved) return;

			_requestCenterChunk = newCenterChunk;

			RemoveNotInSightChunks(size, size, requestCenterWorld, radius);


			for (var x = (int) _requestCenterChunk.x - radius; x <= _requestCenterChunk.x + radius; x++)
			{
				for (var z = (int) _requestCenterChunk.y - radius; z <= _requestCenterChunk.y + radius; z++)
				{
					if (IsChunkInSight(x, z, size, size, requestCenterWorld, radius))
					{
						Assets.GDI.Code.Chunk.Chunk chunk = GetChunk(x, z, size, size, seed);
						if (chunk != null) chunk.Update();
					}
				}
			}
		}

		/// <summary>
		/// Requests a chunk defined by the parameters.
		///</summary>
		///<param name="chunkX">The x position in chunk cooridnates.</param>
		///<param name="chunkZ">The z position in chunk cooridnates.</param>
		///<param name="sizeX">The x size of the chunk.</param>
		///<param name="sizeZ">The z size of the chunk.</param>
		///<param name="seed">The seed for the chunk.</param>
		///<returns>The requested chunk.</returns>
		private Code.Chunk.Chunk RequestChunk(int chunkX, int chunkZ, int sizeX, int sizeZ, float seed)
		{
			return AbstractChunkNode.GetInputChunk(_inputSocketChunk, new Request(chunkX, 0, chunkZ, sizeX, 0, sizeZ, seed));
		}

		/// <summary>
		/// Returns the cached chunk defined by the parameters. Requests a new chunk if
		/// it is not in the chache.
		///</summary>
		///<param name="x">The x position in chunk cooridnates.</param>
		///<param name="z">The z position in chunk cooridnates.</param>
		///<param name="sizeX">The x size of the chunk.</param>
		///<param name="sizeZ">The z size of the chunk.</param>
		///<param name="seed">The seed for the chunk.</param>
		///<returns>The cached or requested chunk.</returns>
		private Code.Chunk.Chunk GetChunk(int x, int z, int sizeX, int sizeZ, float seed)
		{
			Code.Chunk.Chunk chunk = GetCachedChunk(x, z);
			if (chunk == null)
			{
				chunk = RequestChunk(x, z, sizeX, sizeZ, seed);
				if (chunk != null)
				{
					chunk.Init(GetChunkContainer().transform);
					_chunks.Add(chunk);
				}
			}
			return chunk;
		}

		/// <summary>
		/// Removes chunks from the cache that are not in the sight of the assigned parameters.
		///</summary>
		///<param name="sizeX">The x size of the chunk.</param>
		///<param name="sizeZ">The z size of the chunk.</param>
		///<param name="requestCenterWorld">The position of the request center in world coordinates.</param>
		///<param name="radius">The radius of the sight in chunk coordinates.</param>
		private void RemoveNotInSightChunks(int sizeX, int sizeZ, UnityEngine.Vector3 requestCenterWorld, int radius)
		{
			_removeList.Clear();
			for (var index = 0; index < _chunks.Count; index++)
			{
				var chunk = _chunks[index];
				if (!IsChunkInSight(chunk.X, chunk.Z, sizeX, sizeZ, requestCenterWorld, radius)) _removeList.Add(chunk);
			}
			for (var index = 0; index < _removeList.Count; index++) RemoveChunk(_removeList[index]);
		}

		/// <summary>
		/// Removes the assigned chunk from the cache and calls the destroy method.
		///</summary>
		///<param name="chunk">The chunk to remove.</param>
		public void RemoveChunk(Assets.GDI.Code.Chunk.Chunk chunk)
		{
			if (chunk == null) return;
			_chunks.Remove(chunk);
			chunk.Destroy();
			UnityEngine.GameObject.DestroyImmediate(chunk.Obj);
		}

		/// <summary>
		// Gets a cached chunk defined by the parameters.
		///</summary>
		///<param name="chunkX">The x position in chunk cooridnates.</param>
		///<param name="chunkZ">The z position in chunk cooridnates.</param>
		///<returns>The chunk or null.</returns>
		public Code.Chunk.Chunk GetCachedChunk(int chunkX, int chunkZ)
		{
			for (int index = 0; index < _chunks.Count; index++)
			{
				var chunk = _chunks[index];
				if (chunk.X == chunkX && chunk.Z == chunkZ) return chunk;
			}
			return null;
		}

		public static bool IsChunkInSight(int chunkX, int chunkZ, int sizeX, int sizeZ, UnityEngine.Vector3 requestCenterWorld, int radius)
		{
			Vector2 centerChunk = ToChunkPosition(requestCenterWorld.x, requestCenterWorld.z, sizeX, sizeZ);
			centerChunk.x -= chunkX;
			centerChunk.y -= chunkZ;
			return Mathf.Sqrt(centerChunk.x * centerChunk.x + centerChunk.y * centerChunk.y) <= radius;
		}


		public static Vector2 ToWorldPosition(int chunkX, int chunkZ, int chunkSizeX, int chunkSizeZ)
		{
			return new Vector2(chunkX * chunkSizeX, chunkZ * chunkSizeZ);
		}

		public static Vector2 ToChunkPosition(float worldX, float worldZ, int chunkSizeX, int chunkSizeZ)
		{
			return new Vector2(Mathf.FloorToInt(worldX / chunkSizeX), Mathf.FloorToInt(worldZ / chunkSizeZ));
		}

		public void ClearChunkContainer()
		{
			while (_chunks.Count > 0) RemoveChunk(_chunks[0]);
			var children = new List<UnityEngine.GameObject>();
			UnityEngine.GameObject chunkContainer = GetChunkContainer();
			if (chunkContainer != null)
			{
				foreach (Transform child in chunkContainer.transform) children.Add(child.gameObject);
				children.ForEach(child => UnityEngine.GameObject.DestroyImmediate(child));
			}
		}

		private UnityEngine.GameObject GetChunkContainer()
		{
			if (_chunkContainer == null) _chunkContainer = UnityEngine.GameObject.Find("ChunkContainer");
			if (_chunkContainer == null) _chunkContainer = new UnityEngine.GameObject("ChunkContainer");
			return _chunkContainer;
		}

	}
}
