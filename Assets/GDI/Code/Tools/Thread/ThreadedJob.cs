using System.Collections;

namespace Assets.GDI.Code.Tools.Thread
{
	public abstract class ThreadedJob  {

		private bool _IsDone;
		private object _Handle = new object();
		private bool _aborted;

		private System.Threading.Thread _Thread;

		public bool IsDone
		{
			get
			{
				bool tmp;
				lock (_Handle)
				{
					tmp = _IsDone;
				}
				return tmp;
			}
			set
			{
				lock (_Handle)
				{
					_IsDone = value;
				}
			}
		}

		public virtual void Start()
		{
			_Thread = new System.Threading.Thread(Run);
			_Thread.Start();
			_aborted = false;
		}

		public virtual void Abort()
		{
			_aborted = true;
			if (_Thread != null) _Thread.Abort();
		}

		protected abstract void ThreadFunction();

		protected abstract void OnFinished();

		public virtual bool Update()
		{
			if (IsDone && !_aborted)
			{
				OnFinished();
				return true;
			}
			return false;
		}

		public IEnumerator WaitFor()
		{
			while(!Update() && !_aborted)
			{
				yield return null;
			}
		}

		private void Run()
		{
			if (!_aborted) ThreadFunction();
			IsDone = true;
		}

		public bool IsStarted()
		{
			return _Thread != null && !_aborted;
		}
	}
}
