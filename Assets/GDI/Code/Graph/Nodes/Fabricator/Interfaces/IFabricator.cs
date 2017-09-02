using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Fabricator
{
    public interface IFabricator
    {
        Resource GetResource(OutputSocket _outSocket, Request _request);
    }
}