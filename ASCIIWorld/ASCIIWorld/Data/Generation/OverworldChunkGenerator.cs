using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation
{
	[Serializable]
	public class OverworldChunkGenerator : BaseChunkGenerator
	{
		#region Fields

		private int _dirtId;
		private int _waterId;
		private int _sandId;
		private int _grassId;
		private int _stoneId;
		private int _bushId;

		#endregion

		#region Constructors

		public OverworldChunkGenerator(int width, int height, string seed)
			: base(width, height, seed)
		{
			_dirtId = BlockRegistry.Instance.GetId("Dirt");
			_waterId = BlockRegistry.Instance.GetId("Water");
			_sandId = BlockRegistry.Instance.GetId("Sand");
			_grassId = BlockRegistry.Instance.GetId("Grass");
			_stoneId = BlockRegistry.Instance.GetId("Stone");
			_bushId = BlockRegistry.Instance.GetId("Bush");
		}

		#endregion

		#region Properties

		public override float AmbientLightLevel
		{
			get
			{
				return 32.0f;
			}
		}

		#endregion

		#region Methods

		public override Chunk Generate(IProgress<string> progress, int chunkX, int chunkY)
		{
			Reseed(chunkX, chunkY);

			var chunk = new Chunk(Width, Height);
			Fill(chunk, ChunkLayer.Background, _dirtId);
			Fill(chunk, ChunkLayer.Floor, _grassId);

			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					var value = SimplexNoise.Generate((chunkX * Width + x) / 256.0f, (chunkY * Height + y) / 256.0f);
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

			chunk = GenerateBushes(progress, chunk);

			return chunk;
		}

		private Chunk GenerateBushes(IProgress<string> progress, Chunk chunk)
		{
			// This will try to plant 16 bushes on grass areas of the chunk.
			for (var bushIndex = 0; bushIndex < 16; bushIndex++)
			{
				progress.Report($"Planting bush (x{bushIndex + 1})...");

				// Try to find a spawn point.
				Vector2I? spawnPoint = null;
				for (var spawnCheck = 0; spawnCheck < 16; spawnCheck++)
				{
					spawnPoint = chunk.FindSpawnPoint();
					if (!spawnPoint.HasValue)
					{
						break;
					}
					else
					{
						if (chunk[ChunkLayer.Floor, spawnPoint.Value.X, spawnPoint.Value.Y] == _grassId)
						{
							break;
						}
						else
						{
							spawnPoint = null;
						}
					}
				}

				// If a spawn point was found, plan a bush.
				if (spawnPoint.HasValue)
				{
					chunk[ChunkLayer.Blocking, spawnPoint.Value.X, spawnPoint.Value.Y] = _bushId;
				}
				else
				{
					break;
				}
			}

			return chunk;
		}

		#endregion
	}
}
