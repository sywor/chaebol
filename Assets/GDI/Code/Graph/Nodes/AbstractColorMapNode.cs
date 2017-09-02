using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractColorMapNode : Node, IColorMapConnection {

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(0, 255 / 255f, 55 / 255f, 1f);

		protected AbstractColorMapNode(int id, Graph parent) : base(id, parent)
		{
		}

		public static UnityEngine.Color[,] GetInputColorMap(InputSocket socket, Request request)
		{
			if (!socket.IsConnected()) return null;
			IColorMapConnection sampler = socket.GetConnectedSocket().Parent as IColorMapConnection;
			if (sampler == null) return null;
			return sampler.GetColorMap(socket.GetConnectedSocket(), request);
		}

		public abstract UnityEngine.Color[,] GetColorMap(OutputSocket socket, Request request);
	}
}
