using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IStringConnection
	{
		string GetString(OutputSocket outSocket, Request request);
	}
}
