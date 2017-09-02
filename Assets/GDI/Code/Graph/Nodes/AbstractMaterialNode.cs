using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractMaterialNode : Node, IMaterialConnection {

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(254 / 255f, 215 / 255f, 0f, 1f);

		protected AbstractMaterialNode(int id, Graph parent) : base(id, parent)
		{
		}

		public static UnityEngine.Material GetInputMaterial(InputSocket socket, Request request)
		{
			if (!socket.IsConnected()) return null;
			IMaterialConnection sampler = socket.GetConnectedSocket().Parent as IMaterialConnection;
			if (sampler == null) return null;
			return sampler.GetMaterial(socket.GetConnectedSocket(), request);
		}

		public abstract UnityEngine.Material GetMaterial(OutputSocket ouputSocket, Request request);
	}
}
