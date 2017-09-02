using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Tools
{
	public class Vector3DisplayColorSampler : IColorConnection {

		private UnityEngine.Color transparentColor = new UnityEngine.Color(0f, 0f, 0f, 0f);

		public UnityEngine.Color GetColor(OutputSocket s, Request request)
		{
			if (request.X == 1f) return UnityEngine.Color.white;
			return transparentColor;
		}

		public int GetId()
		{
			return -1;
		}
	}
}
