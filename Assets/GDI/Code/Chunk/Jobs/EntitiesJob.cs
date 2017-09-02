using System.Collections.Generic;
using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Nodes.Chunk;
using UnityEngine;

namespace Assets.GDI.Code.Chunk.Jobs
{
	public class EntitiesJob : AbstractChunkJob
	{

		private IEntitiesCreatedListener _listener;
		private EntitiesNode _node;
		private int _sizeX;
		private int _sizeZ;

		private List<Vector3> _positions;
		private List<Vector3> _rotations;
		private List<Vector3> _scales;
		private int _requestOffsetX;
		private int _requestOffsetZ;

		public EntitiesJob(Chunk chunk, EntitiesNode node, IEntitiesCreatedListener listener) : base(chunk, node.Id)
		{
			_chunk = chunk;
			_listener = listener;
			_node = node;
			_sizeX = chunk.SizeX;
			_sizeZ = chunk.SizeZ;
			_requestOffsetX = chunk.X * _sizeX;
			_requestOffsetZ = chunk.Z * _sizeZ;
		}

		protected override void ThreadFunction()
		{
			Request request = new Request(_requestOffsetX, 0, _requestOffsetZ, _sizeX, 0, _sizeZ, _chunk.Seed);
			_positions = _node.GetPositions(request);

			if (_positions != null && _positions.Count > 0)
			{
				_rotations = new List<Vector3>();
				_scales = new List<Vector3>();

				Request r = new Request();

				for (int i = 0; i < _positions.Count; i++)
				{
					Vector3 p = _positions[i];
					r.X = p.x;
					r.Z = p.z;
					r.Seed = _chunk.Seed;
					Vector3 rotation = new Vector3();
					rotation.Set(_node.GetRotationX(r), _node.GetRotationY(r), _node.GetRotationZ(r));
					_rotations.Add(rotation);
					Vector3 scale = new Vector3();
					scale.Set(_node.GetScaleX(r), _node.GetScaleY(r), _node.GetScaleZ(r));
					_scales.Add(scale);
				}
			}
		}

		protected override void OnFinished()
		{
			if (_positions == null)
			{
				_listener.OnEntitiesCreated(null, _node.Id);
				return;
			}

			Request request = new Request();
			request.Seed = _chunk.Seed;
			GameObject original = _node.GetGameObject(null, request);

			List<GameObject> gameObjects = new List<GameObject>();
			GameObject entitiesContainer = _chunk.CreateChild("Entities " + _node.Id);

			for (int i = 0; i < _positions.Count; i++)
			{
				GameObject clone = (GameObject) Object.Instantiate(original, entitiesContainer.transform);
				clone.transform.position = _positions[i];
				if (_rotations != null && i < _rotations.Count) clone.transform.eulerAngles += _rotations[i];
				if (_scales != null && i < _scales.Count) clone.transform.localScale = _scales[i];
				gameObjects.Add(clone);
			}
			_listener.OnEntitiesCreated(gameObjects, _node.Id);
		}
	}
}
