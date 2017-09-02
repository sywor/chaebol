using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface INumberConnection
	{
		float GetNumber(OutputSocket outSocket, Request request);
	}
}
