using System;

namespace ASCIIWorld.Data
{
	public class PlantBlockBehavior : BlockBehavior
	{
		private TimeSpan _totalElapsedTime;

		public PlantBlockBehavior()
		{
			_totalElapsedTime = TimeSpan.Zero;
		}

		public override void Update(TimeSpan elapsed, Level level, ChunkLayer layer, int blockX, int blockY)
		{
			_totalElapsedTime += elapsed;
			if (_totalElapsedTime.TotalSeconds > 5)
			{
				_totalElapsedTime = TimeSpan.Zero;

				var oldMetadata = level.GetMetadata(layer, blockX, blockY);
				var newMetadata = oldMetadata + 1;
				if (newMetadata <= 7)
				{
					level.SetMetadata(layer, blockX, blockY, newMetadata);
				}
			}
		}

		// TODO: What should this block drop when it breaks?
	}
}
