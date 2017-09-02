using System.Collections.Generic;

namespace Assets.GDI.Code.Chunk.Jobs
{


	public class JobQueue {

		private List<AbstractChunkJob> _jobs;
		private List<AbstractChunkJob> _removeList;

		public JobQueue()
		{
			_jobs = new List<AbstractChunkJob>();
			_removeList = new List<AbstractChunkJob>();
		}

		public void Start(AbstractChunkJob job)
		{
			_jobs.Add(job);
			job.Start();
		}

		public void Update()
		{
			_removeList.Clear();
			for (var i = 0; i < _jobs.Count; i++)
			{
				AbstractChunkJob job = _jobs[i];
				job.Update();
				if (job.IsDone)
				{
					_removeList.Add(job);
				}
			}
			for (var i = 0; i < _removeList.Count; i++)
			{
				_jobs.Remove(_removeList[i]);
			}
		}

		public bool IsWorkingOn(Chunk chunk, int chunkEntityId)
		{
			for (int i = 0; i < _jobs.Count; i++)
			{
				AbstractChunkJob job = _jobs[i];
				if (job.IsWorkingOn(chunk, chunkEntityId))
				{
					return true;
				}
			}
			return false;
		}

		public void Abort()
		{
			for (var i = 0; i < _jobs.Count; i++)
			{
				_jobs[i].Abort();
			}
			_jobs.Clear();
		}

	}
}
