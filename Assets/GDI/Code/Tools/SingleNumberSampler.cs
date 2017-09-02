using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;

namespace Assets.GDI.Code.Tools
{
	public class SingleNumberSampler : INumberConnection
	{
		private float _number;

		public SingleNumberSampler(float number)
		{
			_number = number;
		}

		public float GetNumber(OutputSocket s, Request request)
		{
			return _number;
		}

		public int GetId()
		{
			return -1;
		}
	}
}
