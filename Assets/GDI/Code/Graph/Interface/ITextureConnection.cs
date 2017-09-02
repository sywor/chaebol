using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface ITextureConnection
	{
		Texture2D GetTexture(OutputSocket outputSocket, Request request);
	}
}

