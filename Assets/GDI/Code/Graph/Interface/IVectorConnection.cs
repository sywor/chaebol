using System.Collections.Generic;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IVectorConnection
	{
		List<Vector3> GetVector3List(OutputSocket outSocket, Request request);
	}
}
