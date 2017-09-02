using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IChunkConnection
	{
		Chunk.Chunk GetChunk(OutputSocket outSocket, Request request);
	}
}
