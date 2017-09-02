using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractStringNode : Node, IStringConnection {

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(0.54f, 0.45f, 0.39f, 1f);

		protected AbstractStringNode(int id, Graph parent) : base(id, parent)
		{
		}


		public static string GetInputString(InputSocket socket, Request request)
		{
			if (!socket.IsConnected()) return null;
			IStringConnection sampler = socket.GetConnectedSocket().Parent as IStringConnection;
			if (sampler == null) return null;
			return sampler.GetString(socket.GetConnectedSocket(), request);
		}

		public abstract string GetString(OutputSocket outSocket, Request request);
	}
}
