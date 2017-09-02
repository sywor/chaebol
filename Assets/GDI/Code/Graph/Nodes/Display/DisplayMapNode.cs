using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Interface;
using Assets.GDI.Code.Graph.Socket;
using Assets.GDI.Code.Tools;
using Assets.GDI.Code.Tools.Thread;
using Assets.GDI.Code.Tools.Thread.TextureJobs;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Display
{
	[Serializable]
	[GraphContextMenuItem("", "Display Map")]
	public class DisplayMapNode : AbstractNoiseNode
	{
		[SerializeField] private int _sizeModifcator;

		[SerializeField] private int _textureOffsetX = 0;
		[SerializeField] private int _textureOffsetZ = 0;

		[NonSerialized] private InputSocket _inputSocketNumber;
		[NonSerialized] private InputSocket _inputSocketColor;
		[NonSerialized] private InputSocket _inputSocketVec;


		[NonSerialized] private const int SizeStep = 50;
		[NonSerialized] private const int TextureLeft = 35;
		[NonSerialized] private const int TextureTop = 35;
		[NonSerialized] private const int TextureRight = 10;
		[NonSerialized] private const int TextureBottom = 50;

		private List<UnityEngine.Vector3> _lastVectors;

		private bool _initializedSize;
		private Rect _tmpRect;

		public DisplayMapNode(int id, Graph parent) : base(id, parent)
		{

			_inputSocketNumber = new InputSocket(this, typeof(INumberConnection));
			Sockets.Add(_inputSocketNumber);

			_inputSocketColor = new InputSocket(this, typeof(IColorConnection));
			Sockets.Add(_inputSocketColor);

			_inputSocketVec = new InputSocket(this, typeof(IVectorConnection));
			Sockets.Add(_inputSocketVec);

			_textures.Add(new GUIThreadedTexture()); // heightmap
			_textures.Add(new GUIThreadedTexture()); // points

			_tmpRect = new Rect();
			Width = 205;
			Height = 305;
		}

		protected override void OnGUI()
		{

			if (!_initializedSize) ChangeTextureSize(_sizeModifcator * SizeStep);


			if (!_textures[0].DoneInitialUpdate && _inputSocketNumber.IsConnected()) StartJob(_textures[0]);
			if (!_textures[1].DoneInitialUpdate && _inputSocketNumber.IsConnected()) StartJob(_textures[1]);

			float yOffset = CalcTextureHeight() + TextureTop + 25;

			if (!IsUpdatingTexture() && _textures[0] != null)
			{
				_tmpRect.Set(0, 0, 18, 20);
				if (GUI.Button(_tmpRect, "+"))
				{
					ChangeTextureSize(+SizeStep);
					_sizeModifcator++;
				}
				_tmpRect.Set(17, 0, 18, 20);
				if (CalcTextureWidth() > 100 && GUI.Button(_tmpRect, "-"))
				{
					ChangeTextureSize(-SizeStep);
					_sizeModifcator--;
				}

				_tmpRect.Set(34, 0, 27, 20);
				if (Width > 100 && GUI.Button(_tmpRect, "left"))
				{
					_textureOffsetX -= 50;
					Update();
				}

				_tmpRect.Set(59, 0, 36, 20);
				if (Width > 100 && GUI.Button(_tmpRect, "right"))
				{
					_textureOffsetX += 50;
					Update();
				}

				_tmpRect.Set(94, 0, 24, 20);
				if (Width > 100 && GUI.Button(_tmpRect, "up"))
				{
					_textureOffsetZ += 50;
					Update();
				}

				_tmpRect.Set(117, 0, 38, 20);
				if (Width > 100 && GUI.Button(_tmpRect, "down"))
				{
					_textureOffsetZ -= 50;
					Update();
				}

				float h = _textures[0].Height;
				float w = _textures[0].Width;

				if (!_inputSocketNumber.IsConnected() && _inputSocketVec.IsConnected()) h = _textures[1].Height;
				if (!_inputSocketNumber.IsConnected() && _inputSocketVec.IsConnected()) w = _textures[1].Width;

				NodeUtils.DrawVerticalLine(
					new Vector2(TextureLeft - 2, CalcTextureHeight() + TextureTop),
					new Vector2(TextureLeft - 2, TextureTop), Config.ArrowZ,
					"z", _textureOffsetZ + "", h + _textureOffsetZ + "");

				NodeUtils.DrawHorizontalLine(
					new Vector2(TextureLeft, CalcTextureHeight() + TextureTop),
					new Vector2(CalcTextureWidth() + TextureLeft, CalcTextureHeight() + TextureTop), Config.ArrowX,
					"x", _textureOffsetX + "", w + _textureOffsetX + "");
			}
			DrawTextures();

			GUI.skin.label.alignment = TextAnchor.MiddleLeft;

			if (_textures[0] != null && !float.IsNaN(_textures[0].LowestValue))
			{
				_tmpRect.Set(3, yOffset, 160, 20);
				string minMax = "y min " + Math.Round(_textures[0].LowestValue, 4);

				if (!float.IsNaN(_textures[0].HighestValue))
				{
					minMax += " max " + Math.Round(_textures[0].HighestValue, 4);
				}
				GUI.Label(_tmpRect, minMax);

				_tmpRect.Set(3, yOffset + 20, 160, 20);
				GUI.Label(_tmpRect, "load heightmap " + _textures[0].ElapsedMillis / 1000f + " s");
			}

			_tmpRect.Set(3, yOffset + 40, 100, 20);
			if (_lastVectors != null)
			{
				GUI.Label(_tmpRect, "vector count " + _lastVectors.Count);
			}
			else
			{
				GUI.Label(_tmpRect, "no vectors");
			}

			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		}

		private void ChangeTextureSize(int size)
		{
			_initializedSize = true;
			Width += size;
			Height += size;
			Update();
		}

		private void StartJob(GUIThreadedTexture t)
		{
			HeightMapUpdateJob job = new HeightMapUpdateJob();
			job.Request(_textureOffsetX, _textureOffsetZ, CalcTextureWidth(), CalcTextureHeight(),
				GetNumberSampler(), _inputSocketNumber.GetConnectedSocket(),
				GetColorSampler(), _inputSocketColor.GetConnectedSocket());

			t.StartTextureUpdateJob(TextureLeft, TextureTop, CalcTextureWidth(), CalcTextureHeight(), job);
		}

		private int CalcTextureWidth()
		{
			return (int) Width - 10 - TextureLeft - TextureRight;
		}

		private int CalcTextureHeight()
		{
			return (int) Height - 70 - TextureBottom - TextureTop;
		}

		public override void Update()
		{
			if (Collapsed) return;

			if (_inputSocketNumber.IsConnected()) StartJob(_textures[0]);
			else _textures[0].Hide();


			if (_inputSocketVec.CanGetResult())
			{
				UpdateVectorTexture();
			}
			else
			{
				if (_textures[1] != null) _textures[1].Hide();
				_lastVectors = null;
			}
		}

		private void UpdateVectorTexture()
		{
			Request request = new Request();
			request.X = _textureOffsetX;
			request.Z = _textureOffsetZ;
			request.SizeX = CalcTextureWidth();
			request.SizeZ = CalcTextureHeight();
			_lastVectors = GetPositionSampler().GetVector3List(_inputSocketVec.GetConnectedSocket(), request);

			if (_lastVectors != null)
			{

				// the vector list must be cleaned up here because the basic Vector2Node ignores the request parameter.
				List<UnityEngine.Vector3> removeList = new List<UnityEngine.Vector3>();
				for (int i = 0; i < _lastVectors.Count; i++)
				{
					UnityEngine.Vector3 v = _lastVectors[i];
					if (!(v.x >= request.X && v.x < request.X + request.SizeX && v.z >= request.Z && v.z < request.Z + request.SizeZ))
					{
						removeList.Add(v);
					}
				}
				for (int i = 0; i < removeList.Count; i++) _lastVectors.Remove(removeList[i]);

				HeightMapUpdateJob job = new HeightMapUpdateJob();
				job.Request(_textureOffsetX, _textureOffsetZ, CalcTextureWidth(), CalcTextureHeight(), _lastVectors);
				_textures[1].StartTextureUpdateJob(TextureLeft, TextureTop, CalcTextureWidth(), CalcTextureHeight(), job);
			}
		}

		public override float GetNumber(OutputSocket outSocket, Request request)
		{
			return GetInputNumber(_inputSocketNumber, request);
		}

		private IColorConnection GetColorSampler()
		{
			if (_inputSocketColor.CanGetResult()) return (IColorConnection) _inputSocketColor.GetConnectedSocket().Parent;
			return null;
		}

		private INumberConnection GetNumberSampler()
		{
			if (_inputSocketNumber.IsInDirectInputMode()) return new SingleNumberSampler(GetInputNumber(_inputSocketNumber, new Request()));
			if (_inputSocketNumber.CanGetResult()) return (INumberConnection) _inputSocketNumber.GetConnectedSocket().Parent;
			return null;
		}

		private IVectorConnection GetPositionSampler()
		{
			if (_inputSocketVec.CanGetResult()) return (IVectorConnection) _inputSocketVec.GetConnectedSocket().Parent;
			return null;
		}


	}
}

