using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Tools.Thread.TextureJobs
{
	public class ProfileTextureUpdateJob : AbstractTextureJob {

		private INumberConnection _numberSampler;
		private OutputSocket _numberSamplerRequestSocket;

		private IColorConnection _samplerColor;
		private OutputSocket _samplerColorRequestSocket;

		private float[,] _values;

		private int _startX;
		private int _startZ;

		public void Request(int startX, int startZ, int width, int height,
			INumberConnection sampler, OutputSocket numberSamplerRequestSocket,
			IColorConnection colorSampler = null, OutputSocket colorSamplerRequestSocket = null)
		{
			_startX = startX;
			_startZ = startZ;
			_numberSampler = sampler;
			_numberSamplerRequestSocket = numberSamplerRequestSocket;
			_samplerColorRequestSocket = colorSamplerRequestSocket;

			_samplerColor = colorSampler;
			_values = new float[width, height];
			Width = width;
			Height = height;
		}

		protected override void ThreadFunction()
		{
			Request r = new Request();
			if (_numberSampler == null) return;
			for (var x = _startX; x < _startX + Width; x++)
			{
				for (var y = _startZ; y < _startZ + Height; y++)
				{
					r.X = x;
					float v = _numberSampler.GetNumber(_numberSamplerRequestSocket, r);
					if (float.IsNaN(ResultLowestValue) || ResultLowestValue > v) ResultLowestValue = v;
					if (float.IsNaN(ResultHighestValue) || ResultHighestValue < v) ResultHighestValue = v;
					float relativeHeight = (v + 1) / 2f * Height;
					if (relativeHeight > y) _values[x, y] = y / (float) Height * 2 - 1;
					else _values[x, y] = float.NaN;
				}
			}
		}

		protected override void Finish()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			Texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
			if (_numberSampler != null) Texture.SetPixels(NodeUtils.ToColorMap(_values, _samplerColor, _samplerColorRequestSocket));
			Texture.Apply();
		}
	}
}
