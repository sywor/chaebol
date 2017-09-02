using System.Collections.Generic;
using Assets.GDI.Code.Chunk.Jobs;
using Assets.GDI.Code.Chunk.Landscape;
using Assets.GDI.Code.Graph.Nodes.Chunk;
using UnityEngine;

namespace Assets.GDI.Code.Chunk
{
	public class Chunk : ILandscapeCreatedListener, IEntitiesCreatedListener
	{
		private GameObject _gameObject;

		private Dictionary<int, AbstractGridMesh> _landscapeMeshList;
		private Dictionary<int, List<GameObject>> _entitiesList;

		private List<LandscapeNode> _landscapeNodes;
		private List<EntitiesNode> _entitiesNodes;

		private JobQueue _landscapeJobQueue;
		private JobQueue _entitiesJobQueue;

		private int _x;
		private int _z;
		private int _sizeX;
		private int _sizeZ;
		private int _resolution = 1;
		private float _seed;


		public int X { get { return _x; } }
		public int Z { get { return _z; } }
		public int SizeX { get { return _sizeX; } }
		public int SizeZ { get { return _sizeZ; } }
		public int Resolution { get { return _resolution; } }
		public GameObject Obj { get { return _gameObject; } }
		public bool IsInitialized { get { return _gameObject != null; } }
		public float Seed { get { return _seed; } }


		public Chunk(int chunkX, int chunkZ, int sizeX, int sizeZ, float seed)
		{
			_seed = seed;
			_x = chunkX;
			_z = chunkZ;
			_sizeX = sizeX;
			_sizeZ = sizeZ;
			_landscapeJobQueue = new JobQueue();
			_entitiesJobQueue = new JobQueue();
		}

		public void AddLandscape(LandscapeNode landscapeNode)
		{
			if (_landscapeNodes == null) _landscapeNodes = new List<LandscapeNode>();
			_landscapeNodes.Add(landscapeNode);
		}

		public void AddEntities(EntitiesNode entitiesNode)
		{
			if (_entitiesNodes == null) _entitiesNodes = new List<EntitiesNode>();
			_entitiesNodes.Add(entitiesNode);
		}


		public void Update()
		{
			_landscapeJobQueue.Update();
			_entitiesJobQueue.Update();
		}

		public void Init(Transform parent)
		{
			_gameObject = new GameObject("chunk x: " + _x + " z: " + _z);
			_gameObject.transform.parent = parent;
			_gameObject.transform.position = new Vector3(_x * _sizeX, 0, _z * _sizeZ);

			if (_landscapeNodes != null) for (int i = 0; i < _landscapeNodes.Count; i++) StartLandscapeJob(_landscapeNodes[i]);
			if (_entitiesNodes != null) for (int i = 0; i < _entitiesNodes.Count; i++) StartEntitiesJob(_entitiesNodes[i]);
		}

		public void StartLandscapeJob(LandscapeNode node)
		{
			if (!IsInitialized) return;
			if (_landscapeMeshList == null) _landscapeMeshList = new Dictionary<int, AbstractGridMesh>();
			if (IsWorkingOnLandscape(node.Id)) return;
			if (_landscapeMeshList.ContainsKey(node.Id)) return;
			LandscapeJob landscapeJob = new LandscapeJob(this, node, this);
			_landscapeJobQueue.Start(landscapeJob);
		}

		public void StartEntitiesJob(EntitiesNode node)
		{
			if (!IsInitialized) return;
			if (_entitiesList == null) _entitiesList = new Dictionary<int, List<GameObject>>();
			if (IsWorkingOnEntities(node.Id)) return;
			if (_entitiesList.ContainsKey(node.Id)) return;
			EntitiesJob entitiesJob = new EntitiesJob(this, node, this);
			_entitiesJobQueue.Start(entitiesJob);
		}

		public void OnLandscapeCreated(AbstractGridMesh landscape, int id)
		{
			_landscapeMeshList.Add(id, landscape);
		}

		public void OnEntitiesCreated(List<GameObject> entities, int id)
		{
			_entitiesList.Add(id, entities);
		}

		public bool IsWorkingOnLandscape(int id)
		{
			return _landscapeJobQueue.IsWorkingOn(this, id);
		}

		public bool IsWorkingOnEntities(int id)
		{
			return _entitiesJobQueue.IsWorkingOn(this, id);
		}

		public GameObject CreateChild(string name)
		{
			var obj = new GameObject(name);
			obj.transform.position = _gameObject.transform.position;
			obj.transform.parent = _gameObject.transform;
			return obj;
		}

		public void Destroy()
		{
			_landscapeJobQueue.Abort();
			_entitiesJobQueue.Abort();
			_landscapeJobQueue = null;
			_entitiesJobQueue = null;
			if (_landscapeMeshList != null) _landscapeMeshList.Clear();
			if (_entitiesList != null) _entitiesList.Clear();
			if (_landscapeNodes != null) _landscapeNodes.Clear();
			if (_entitiesNodes != null) _entitiesNodes.Clear();
		}

		public bool Equals(Chunk chunk)
		{
			if (chunk == null) return false;
			return chunk._x == _x && chunk._z == _z && chunk._sizeX == _sizeX && chunk._sizeZ == _sizeZ && chunk._resolution == _resolution;
		}

	}

	public interface ILandscapeCreatedListener
	{
		void OnLandscapeCreated(AbstractGridMesh landscape, int id);
	}

	public interface IEntitiesCreatedListener
	{
		void OnEntitiesCreated(List<GameObject> entities, int id);
	}
}
