using System.Diagnostics;
using UnityEngine;

namespace Assets.GDI.Code.Tools.Thread.TextureJobs
{
	public abstract class AbstractTextureJob : ThreadedJob {

		public Texture2D Texture;
		public float ResultLowestValue = float.NaN;
		public float ResultHighestValue = float.NaN;

		protected int Width;
		protected int Height;

		public long ElapsedMillis = -1;

		private Stopwatch _stopwatch = new Stopwatch();

		public override void Start() {
			_stopwatch.Start();
			base.Start();
		}

		protected override void OnFinished()
		{
			_stopwatch.Stop();
			ElapsedMillis = _stopwatch.ElapsedMilliseconds;
			_stopwatch.Reset();
			Finish();
		}

		protected abstract void Finish();

	}
}
