using ASCIIWorld.Data;
using System;

namespace ASCIIWorld.Generation
{
	public class ChunkGenerator : IGenerator<Chunk>
	{
		private BlockRegistry _blocks;

		public ChunkGenerator(BlockRegistry blocks)
		{
			if (blocks == null)
			{
				throw new ArgumentNullException("blocks");
			}
			_blocks = blocks;
		}

		public Chunk Generate()
		{
			var random = new Random();
			var chunk = new Chunk(_blocks);
			for (var row = 0; row < chunk.Rows; row++)
			{
				for (var column = 0; column < chunk.Columns; column++)
				{
					if (random.Next(4) == 1)
					{
						chunk[1, row, column] = 2;
					}
					else
					{
						chunk[1, row, column] = 1;
					}
				}
			}
			return chunk;
		}
	}
}
