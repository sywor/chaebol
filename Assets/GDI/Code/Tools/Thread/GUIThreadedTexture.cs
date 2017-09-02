using Assets.GDI.Code.Tools.Thread.TextureJobs;
using UnityEngine;

namespace Assets.GDI.Code.Tools.Thread
{
	public class GUIThreadedTexture
	{

		private Rect _textureArea;
		private bool _isUpdatingTexture;
		private AbstractTextureJob _job;
		private Texture2D _texture;
		private bool _initialUpdate;

		public float LowestValue = float.NaN;
		public float HighestValue = float.NaN;
		public long ElapsedMillis = -1;


		public bool IsUpdating
		{
			get { return _isUpdatingTexture; }
		}

		public bool DoneInitialUpdate
		{
			get { return _initialUpdate; }
		}

		public float X
		{
			get { return _textureArea.x; }
			set { _textureArea.x = value; }
		}

		public float Y
		{
			get { return _textureArea.y; }
			set { _textureArea.y = value; }
		}

		public float Width
		{
			get { return _textureArea.width; }
		}

		public float Height
		{
			get { return _textureArea.height; }
		}

		public GUIThreadedTexture()
		{
			_textureArea = new Rect(6, 0, 0, 0);
			_job = new HeightMapUpdateJob();
		}

		public GUIThreadedTexture(AbstractTextureJob job)
		{
			_textureArea = new Rect(6, 0, 0, 0);
			_job = job;
		}

		public void OnGUI()
		{
			if (!DoneInitialUpdate) return;
			_isUpdatingTexture = UpdateTextureJob();
			if (_texture != null && !_isUpdatingTexture)
			{
				_textureArea.Set(_textureArea.x, _textureArea.y, _texture.width, _texture.height);
				GUI.DrawTexture(_textureArea, _texture);
			}
		}

		public void StartTextureUpdateJob(int x, int y, int width, int height, AbstractTextureJob job)
		{
			_job = job;
			InitJob(x, y, width, height);
			_job.Start();
		}

		public void StartTextureUpdateJob(int width, int height, AbstractTextureJob job)
		{
			StartTextureUpdateJob((int) _textureArea.x, (int) _textureArea.y, width, height, job);
		}

		private void InitJob(int x, int y, int width, int height)
		{
			_textureArea.Set(x, y, width, height);
			_initialUpdate = true;
			if (_texture == null) CreateTexture(width, height);
		}

		private bool UpdateTextureJob()
		{
			if (_job == null) return false;
			_job.Update();

			if (!_job.IsDone) return true;
			_texture = _job.Texture;
			LowestValue = _job.ResultLowestValue;
			HighestValue = _job.ResultHighestValue;
			ElapsedMillis = _job.ElapsedMillis;
			_job.Abort();
			_job = null;
			return false;
		}

		private void CreateTexture(int width, int height)
		{
			if (_texture != null) Texture2D.DestroyImmediate(_texture);
			_texture = new Texture2D(width, height, TextureFormat.RGB24, false);
			_textureArea.Set(_textureArea.x, _textureArea.y, width, height);
		}

		public void Hide()
		{
			if (_job != null) _job.Abort();
			if (_texture != null) Texture2D.DestroyImmediate(_texture);
			_job = null;
			_textureArea.Set(_textureArea.x, _textureArea.y, 0, 0);
		}
	}



}

