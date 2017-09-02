using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes
{
	public abstract class AbstractTextureNode : Node, ITextureConnection {

		public static readonly UnityEngine.Color EdgeColor = new UnityEngine.Color(254 / 255f, 55 / 255f, 55 / 255f, 1f);

		protected AbstractTextureNode(int id, Graph parent) : base(id, parent)
		{

		}

		public static Texture2D GetInputTexture(InputSocket socket, Request request)
		{
			if (!socket.IsConnected()) return null;
			ITextureConnection sampler = socket.GetConnectedSocket().Parent as ITextureConnection;
			if (sampler == null) return null;
			return sampler.GetTexture(socket.GetConnectedSocket(), request);
		}

		public abstract Texture2D GetTexture(OutputSocket outputSocket, Request request);
	}
}
