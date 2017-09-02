using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractColorNode : Node, IColorConnection
	{

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(0.54f, 0.70f, 0.50f, 1);

		protected AbstractColorNode(int id, Graph parent) : base(id, parent)
		{

		}

		public static UnityEngine.Color GetInputColor(InputSocket socket, Request request)
		{
			if (!socket.IsConnected()) return UnityEngine.Color.magenta;
			IColorConnection sampler = socket.GetConnectedSocket().Parent as IColorConnection;
			if (sampler == null) return UnityEngine.Color.magenta;
			return sampler.GetColor(socket.GetConnectedSocket(), request);
		}

		public abstract UnityEngine.Color GetColor(OutputSocket socket, Request request);
	}
}
