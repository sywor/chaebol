using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractVector3Node : Node, IVectorConnection
	{

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(0.9f, 0.9f, 0.9f, 1f);

		protected AbstractVector3Node(int id, Graph parent) : base(id, parent)
		{

		}

		public static List<UnityEngine.Vector3> GetInputVector3List(InputSocket socket, Request request)
		{
			if (!socket.CanGetResult()) return null;
			IVectorConnection sampler = socket.GetConnectedSocket().Parent as IVectorConnection;
			if (sampler == null) return null;
			return sampler.GetVector3List(socket.GetConnectedSocket(), request);
		}

		public abstract List<UnityEngine.Vector3> GetVector3List(OutputSocket outSocket, Request request);
	}
}
