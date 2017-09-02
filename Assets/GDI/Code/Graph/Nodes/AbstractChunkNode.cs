using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractChunkNode : Node, IChunkConnection {

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(0, 35 / 255f, 35 / 255f, 1f);

		protected AbstractChunkNode(int id, Graph parent) : base(id, parent)
		{
		}

		public abstract Code.Chunk.Chunk GetChunk(OutputSocket outSocket, Request request);

		public static Code.Chunk.Chunk GetInputChunk(InputSocket socket, Request request)
		{
			if (!socket.IsConnected()) return null;
			IChunkConnection sampler = socket.GetConnectedSocket().Parent as IChunkConnection;
			if (sampler == null) return null;
			return sampler.GetChunk(socket.GetConnectedSocket(), request);
		}
	}
}
