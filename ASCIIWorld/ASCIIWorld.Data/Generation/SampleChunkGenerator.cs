﻿using ASCIIWorld.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Generation
{
	public class SampleChunkGenerator : IGenerator<Chunk>
	{
		private int _grassId;
		private int _waterId;

		public SampleChunkGenerator(Dictionary<int, string> blocks)
		{
			_grassId = blocks.Single(x => x.Value == "Grass").Key;
			_waterId = blocks.Single(x => x.Value == "Water").Key;
		}

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
						chunk[ChunkLayer.Floor, x, y] = _waterId;
					}
					else
					{
						chunk[ChunkLayer.Floor, x, y] = _grassId;
					}
				}
			}

			progress.Report("Done generating chunk.");
			return chunk;
		}
	}
}
