using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IGameObjectsConnection
	{
		GameObject GetGameObject(OutputSocket ouputSocket, Request request);

	}
}
