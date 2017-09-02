using Assets.GDI.Code.Tools.Thread;

namespace Assets.GDI.Code.Chunk.Jobs
{
	public abstract class AbstractChunkJob : ThreadedJob
	{

		protected Chunk _chunk;
		private int _chunkEntityId;

		protected AbstractChunkJob(Chunk chunk, int chunkEntityId)
		{
			_chunk = chunk;
			_chunkEntityId = chunkEntityId;
		}


		public bool IsWorkingOn(Chunk chunk, int entityId)
		{
			return _chunk.Equals(chunk) && _chunkEntityId.Equals(entityId);
		}

	}
}
