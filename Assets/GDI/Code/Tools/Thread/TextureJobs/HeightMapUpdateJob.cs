using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Tools.Thread.TextureJobs
{
	public class HeightMapUpdateJob : AbstractTextureJob
	{

		private INumberConnection _numberSampler;
		private OutputSocket _numberSamplerRequestSocket;

		private IColorConnection _samplerColor;
		private OutputSocket _colorSamplerRequestSocket;

		private List<Vector3> _positions;

		private float[,] _values;

		private int _startX;
		private int _startZ;

		public void Request(int startX, int startZ, int width, int height,
			INumberConnection numberSampler, OutputSocket numberSamplerRequestSocket,
			IColorConnection colorSampler = null, OutputSocket colorSamplerOutSocket = null)
		{
			_startX = startX;
			_startZ = startZ;
			_numberSampler = numberSampler;
			_numberSamplerRequestSocket = numberSamplerRequestSocket;
			_samplerColor = colorSampler;
			_colorSamplerRequestSocket = colorSamplerOutSocket;
			_values = new float[width, height];
			Width = width;
			Height = height;
		}

		public void Request(int startX, int startZ, int width, int height, List<UnityEngine.Vector3> positions)
		{
			_startX = startX;
			_startZ = startZ;
			_positions = positions;
			_samplerColor = new Vector3DisplayColorSampler();
			Width = width;
			Height = height;
		}

		protected override void ThreadFunction()
		{
			try
			{
				Request r = new Request();
				if (_numberSampler == null) return;
				for (var x = _startX; x < _startX + Width; x++)
				{
					for (var z = _startZ; z < _startZ + Height; z++)
					{
						r.X = x;
						r.Z = z;
						float v = _numberSampler.GetNumber(_numberSamplerRequestSocket, r);
						if (float.IsNaN(ResultLowestValue) || ResultLowestValue > v) ResultLowestValue = v;
						if (float.IsNaN(ResultHighestValue) || ResultHighestValue < v) ResultHighestValue = v;
						_values[x - _startX, z - _startZ] = v;
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.Message + "\n" + e.StackTrace);
			}
		}

		protected override void Finish()
		{
			if (Texture != null) Texture2D.DestroyImmediate(Texture);
			Texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);

			if (_numberSampler != null) Texture.SetPixels(NodeUtils.ToColorMap(_values, _samplerColor, _colorSamplerRequestSocket));
			else if (_positions != null)
			{
				Texture.SetPixels(new UnityEngine.Color[Width * Height]);
				for (int index = 0; index < _positions.Count; index++)
				{
					var position = _positions[index];
					Texture.SetPixel((int) position.x - _startX, (int) position.z - _startZ, UnityEngine.Color.white);
				}
			}
			Texture.Apply();
		}
	}
}
