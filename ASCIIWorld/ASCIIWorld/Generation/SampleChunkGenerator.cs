﻿using ASCIIWorld.Data;
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
			for (var row = 0; row < chunk.Rows; row++)
			{
				for (var column = 0; column < chunk.Columns; column++)
				{
					if (random.Next(4) == 1)
					{
						chunk[ChunkLayer.Floor, row, column] = 2;
					}
					else
					{
						chunk[ChunkLayer.Floor, row, column] = 1;
					}
				}
			}

			progress.Report("Done generating chunk.");
			return chunk;
		}
	}
}