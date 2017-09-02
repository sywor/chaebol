using System;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Number.Const
{
	[Serializable]
	[GraphContextMenuItem("Number/Const", "NaN")]
	public class NaNNode : AbstractNumberNode {

		public NaNNode(int id, Graph parent) : base(id, parent)
		{
			Sockets.Add(new OutputSocket(this, typeof(INumberConnection)));
			Width = 35;
			Height = 40;
		}

		protected override void OnGUI()
		{

		}

		public override void Update()
		{

		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			return float.NaN;
		}
	}
}
