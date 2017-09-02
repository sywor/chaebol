using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractNumberNode : Node, INumberConnection
	{
		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(0.32f, 0.58f, 0.86f, 1);

		protected AbstractNumberNode(int id, Graph parent) : base(id, parent)
		{

		}

		public static float GetInputNumber(InputSocket socket, Request request)
		{
			if (socket.IsInDirectInputMode()) return socket.GetDirectInputNumber();
			if (!socket.IsConnected()) return float.NaN;
			INumberConnection sampler = socket.GetConnectedSocket().Parent as INumberConnection;
			if (sampler == null) return float.NaN;
			return sampler.GetNumber(socket.GetConnectedSocket(), request);
		}

		public abstract float GetNumber(OutputSocket outSocket, Request request);
	}
}
