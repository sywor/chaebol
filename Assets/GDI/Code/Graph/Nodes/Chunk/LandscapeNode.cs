using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Chunk
{
	[Serializable]
	[GraphContextMenuItem("Chunk", "Lanscape")]
	public class LandscapeNode : Node, ILandscapeConnection {

		[NonSerialized] private Rect _heightValueLabel;
		[NonSerialized] private Rect _colorRangeLabel;
		[NonSerialized] private Rect _materialLabel;
		[NonSerialized] private Rect _heightFactorLabel;

		[NonSerialized] private InputSocket _heightValueSocket;
		[NonSerialized] private InputSocket _colorSocket;
		[NonSerialized] private InputSocket _materialSocket;

		private InputSocket _inputSocketHeightFactor;

		public LandscapeNode(int id, Graph parent) : base(id, parent)
		{
			_heightValueLabel = new Rect(8, 0, 75, 20);
			_colorRangeLabel = new Rect(8, 20, 75, 20);
			_materialLabel = new Rect(8, 40, 75, 20);
			_heightFactorLabel = new Rect(8, 60, 75, 20);

				_heightValueSocket = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_heightValueSocket);
			_colorSocket = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(_colorSocket);
			_materialSocket = new InputSocket(this, typeof(IMaterialConnection));
			Sockets.Add(_materialSocket);
			_inputSocketHeightFactor = new InputSocket(this, typeof(INumberConnection));
			_inputSocketHeightFactor.SetDirectInputNumber(15, false);
			Sockets.Add(_inputSocketHeightFactor);

			Sockets.Add(new OutputSocket(this, typeof(ILandscapeConnection)));
		}

		protected override void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(_heightValueLabel, "height map");
			GUI.Label(_colorRangeLabel, "vertex color");
			GUI.Label(_materialLabel, "material");
			GUI.Label(_heightFactorLabel, "height factor");
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		public override void Update()
		{

		}

		public float GetNumber(OutputSocket socket, Request request)
		{
			return AbstractNumberNode.GetInputNumber(_heightValueSocket, request);
		}

		public float GetHeightFactor(Request request)
		{
			return AbstractNumberNode.GetInputNumber(_inputSocketHeightFactor, request);
		}

		public UnityEngine.Color GetColor(OutputSocket socket, Request request)
		{
			if (!_colorSocket.IsConnected()) return UnityEngine.Color.magenta;
			AbstractColorNode node = (AbstractColorNode) _colorSocket.GetConnectedSocket().Parent;
			UnityEngine.Color color = node.GetColor(_colorSocket.GetConnectedSocket(), request);
			return color;
		}

		public UnityEngine.Material GetMaterial(OutputSocket ouputSocket, Request request)
		{
			if (!_materialSocket.IsConnected()) return null;
			AbstractMaterialNode node = (AbstractMaterialNode) _materialSocket.GetConnectedSocket().Parent;
			return node.GetMaterial(_materialSocket.GetConnectedSocket(), new Request());
		}
	}
}
