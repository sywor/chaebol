using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IColorConnection
	{
		Color GetColor(OutputSocket socket, Request request);
	}
}
