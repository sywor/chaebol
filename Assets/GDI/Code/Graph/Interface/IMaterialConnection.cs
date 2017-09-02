using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IMaterialConnection
	{

		Material GetMaterial(OutputSocket ouputSocket, Request request);
	}
}
