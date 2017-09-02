using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Nodes.Display;
using Assets.GDI.Code.Graph.Nodes.HeightMap;
using Assets.GDI.Code.Graph.Nodes.Number;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Behaviour
{
	/// <summary>
	/// This class shows how to create graphs programmatically.
	///</summary>
	public class GraphByScriptBehaviour : MonoBehaviour {


		void Start () {

			// create the graph
			Graph.Graph graph = new Graph.Graph();

			// create an operator node
			var operator01 = (NumberOperatorNode) graph.CreateNode<NumberOperatorNode>();
			operator01.X = 200;
			operator01.Y = 40;
			operator01.SetMode(Operator.Add);
			graph.AddNode(operator01);

			// create a display node
			var diplay01 = (NumberDisplayNode) graph.CreateNode<NumberDisplayNode>();
			diplay01.X = 330;
			diplay01.Y = 80;
			graph.AddNode(diplay01);

			// link the nodes
			graph.Link(
				(InputSocket) diplay01.GetSocket(typeof(INumberConnection), typeof(InputSocket), 0),
				(OutputSocket) operator01.GetSocket(typeof(INumberConnection), typeof(OutputSocket), 0));

			// cerate a perlin noise node
			var perlinNoise = graph.CreateNode<UnityPerlinNoiseNode>();
			perlinNoise.X = 80;
			perlinNoise.Y = 250;
			graph.AddNode(perlinNoise);

			// create a display map node
			var displayMap = graph.CreateNode<DisplayMapNode>();
			displayMap.X = 300;
			displayMap.Y = 280;
			graph.AddNode(displayMap);

			// link the nodes
			graph.Link(
				(InputSocket) displayMap.GetSocket(typeof(INumberConnection), typeof(InputSocket), 0),
				(OutputSocket) perlinNoise.GetSocket(typeof(INumberConnection), typeof(OutputSocket), 0));


			// to test the serialization...

			// create a json out of the graph
			var serializedJSON = graph.ToJson();
			// dezeiralize the json back to a graph
			var deserializedGraph = Graph.Graph.FromJson(serializedJSON);

			// add the graph to the launcher to see it in the editor.
			Launcher.Instance.AddGraph(deserializedGraph);

		}


		void Update () {

		}
	}
}
