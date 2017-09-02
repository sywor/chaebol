
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes.Fabricator
{
    public abstract class AbstractFabricatorNode : Node, IResourceConnection
    {
        protected AbstractFabricatorNode(int _id, Graph _parent) : base(_id, _parent)
        {
        }
        
        public static Resource GetInputResource(InputSocket _socket, Request _request)
        {
            if (!_socket.IsConnected()) return null;
            var sampler = _socket.GetConnectedSocket().Parent as IResourceConnection;
            
            return sampler == null ? new Resource() : sampler.GetResource(_socket.GetConnectedSocket(), _request);
        }

        public abstract Resource GetResource(OutputSocket _outSocket, Request _request);
    }
}