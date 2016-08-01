using System;

namespace ASCIIWorld.Data
{
	public class BlockBehavior
	{
		public virtual void Update(TimeSpan elapsed, Level level, ChunkLayer layer, int blockX, int blockY)
		{
		}
	}
}
