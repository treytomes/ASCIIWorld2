using ASCIIWorld.Data;
using System;

namespace ASCIIWorld.Generation
{
	public class SampleChunkGenerator : IGenerator<Chunk>
	{
		public Chunk Generate(IProgress<string> progress)
		{
			progress.Report("Generating chunk.");

			var random = new Random();
			var chunk = new Chunk();
			for (var y = 0; y < chunk.Height; y++)
			{
				for (var x = 0; x < chunk.Width; x++)
				{
					if (random.Next(4) == 1)
					{
						chunk[ChunkLayer.Floor, x, y] = 2;
					}
					else
					{
						chunk[ChunkLayer.Floor, x, y] = 1;
					}
				}
			}

			progress.Report("Done generating chunk.");
			return chunk;
		}
	}
}
