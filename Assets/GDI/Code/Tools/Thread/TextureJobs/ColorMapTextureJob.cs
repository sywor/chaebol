using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Tools.Thread.TextureJobs
{
	public class ColorMapTextureJob : AbstractTextureJob
	{

		private IColorMapConnection _colorMapSampler;
		private OutputSocket _colorSamplerRequestSocket;

		private Color[,] _colors;

		public void Request(IColorMapConnection colorMapSampler, OutputSocket colorSamplerRequestSocket)
		{
			_colorMapSampler = colorMapSampler;
			_colorSamplerRequestSocket = colorSamplerRequestSocket;
		}

		protected override void ThreadFunction()
		{
			_colors = _colorMapSampler.GetColorMap(_colorSamplerRequestSocket, new Request());
		}

		protected override void Finish()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			if (_colors != null)
			{
				Texture = new Texture2D(_colors.GetLength(0), _colors.GetLength(1), TextureFormat.RGBA32, false);
				Texture.SetPixels(NodeUtils.ToTextureColorMap(_colors));
				Texture.Apply();
			}
		}
	}
}

