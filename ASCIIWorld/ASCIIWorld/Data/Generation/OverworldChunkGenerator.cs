using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation
{
	public class OverworldChunkGenerator : BaseChunkGenerator
	{
		#region Fields

		private int _chunkX;
		private int _chunkY;

		private int _dirtId;
		private int _waterId;
		private int _sandId;
		private int _grassId;
		private int _stoneId;

		#endregion

		#region Constructors

		public OverworldChunkGenerator(int width, int height, string seed, int chunkX, int chunkY)
			: base(width, height, seed)
		{
			_chunkX = chunkX;
			_chunkY = chunkY;

			_dirtId = BlockRegistry.Instance.GetByName("Dirt").Id;
			_waterId = BlockRegistry.Instance.GetByName("Water").Id;
			_sandId = BlockRegistry.Instance.GetByName("Sand").Id;
			_grassId = BlockRegistry.Instance.GetByName("Grass").Id;
			_stoneId = BlockRegistry.Instance.GetByName("Stone").Id;
		}

		#endregion

		#region Methods

		public override Chunk Generate(IProgress<string> progress)
		{
			var chunk = new Chunk(Width, Height);
			Fill(chunk, ChunkLayer.Background, _dirtId);
			Fill(chunk, ChunkLayer.Floor, _grassId);

			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					var value = SimplexNoise.Generate((_chunkX * Width + x) / 256.0f, (_chunkY * Height + y) / 256.0f);
					if (value < -0.5)
					{
						chunk[ChunkLayer.Floor, x, y] = _waterId;
						chunk[ChunkLayer.Background, x, y] = _waterId;
					}
					else if (value < -0.25)
					{
						chunk[ChunkLayer.Floor, x, y] = _sandId;
						chunk[ChunkLayer.Background, x, y] = _sandId;
					}
					else if (value > 0.5)
					{
						chunk[ChunkLayer.Blocking, x, y] = _stoneId;
						chunk[ChunkLayer.Background, x, y] = _dirtId;
					}
				}
			}

			return chunk;
		}

		#endregion
	}
}
