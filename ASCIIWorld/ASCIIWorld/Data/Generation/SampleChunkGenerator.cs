using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation
{
	public class SampleChunkGenerator : BaseChunkGenerator
	{
		private int _grassId;
		private int _waterId;

		public SampleChunkGenerator(Dictionary<int, string> blocks, int width, int height, string seed)
			: base(width, height, seed)
		{
			_grassId = blocks.Single(x => x.Value == "Grass").Key;
			_waterId = blocks.Single(x => x.Value == "Water").Key;
		}

		#region Properties

		public override float AmbientLightLevel
		{
			get
			{
				return 8.0f;
			}
		}

		#endregion


		public override Chunk Generate(IProgress<string> progress, int chunkX, int chunkY)
		{
			Reseed(chunkX, chunkY);

			progress.Report("Generating chunk.");

			var chunk = new Chunk(Width, Height);

			var random = new Random();
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
