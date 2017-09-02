using Assets.GDI.Code.Graph.Nodes.Chunk;
using Assets.GDI.Code.Tools;
using UnityEngine;

namespace Assets.GDI.Code.Behaviour
{
	/// <summary>
	/// This is an example how to load a graph that contains a chunk generator
	/// and how to use this generator programmatically.
	/// It also adds a 'WASD' control to the 'Main Camera'.
	///</summary>
	public class GhunkGeneratorBehaviour : MonoBehaviour
	{
		private ChunkGeneratorNode _chunkGenerator;
		private GameObject _camera;

		void Start ()
		{
			// Load the graph..
			string filePath = Application.streamingAssetsPath + "/Examples/NewWorld.json";
			Graph.Graph graph = Launcher.Instance.LoadGraph(filePath);

			if (graph == null)
			{
				Log.Error("Can not find graph file " + filePath);
				return;
			}

			// Get the chunk generator node..
			graph.ForceUpdateNodes();
			_chunkGenerator = (ChunkGeneratorNode) graph.GetFirstNodeWithType(typeof(ChunkGeneratorNode));

			if (_chunkGenerator == null) Log.Error("Can not find a chunk generator node in the graph.");

			// Get the main camera.
			_camera = GameObject.Find("Main Camera");
			if (_camera == null)
			{
				Log.Error("Can not camera with the name 'Main Camera'.");
				return;
			}

			_camera.AddComponent<KeyboardControls>();


		}


		void Update () {
			if (_chunkGenerator != null && _camera != null)
			{
				// Update and request chunks...
				_chunkGenerator.UpdateChunks();
				_chunkGenerator.RequestChunks(_camera.transform.position);
			}
		}

		void OnApplicationQuit()
		{

		}
	}
}
