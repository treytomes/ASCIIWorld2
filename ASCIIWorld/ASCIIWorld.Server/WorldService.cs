using ASCIIWorld.Data;
using ASCIIWorld.Generation;
using CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Server
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WorldService" in both code and config file together.
	public class WorldService : IWorldService
	{
		private Dictionary<int, Chunk> _chunks;
		private int _nextId;

		public WorldService()
		{
			_nextId = 0;
			_chunks = new Dictionary<int, Chunk>();
		}

		public Chunk GetChunk(int chunkId)
		{
			return _chunks[chunkId];
		}

		public Chunk GenerateChunk(Dictionary<int, string> blocks, string seed)
		{
			var progress = new Progress<string>(message => Console.WriteLine(message));
			var chunk = new CavernChunkGenerator(blocks, seed).Generate(progress);
			SpawnBushes(blocks, chunk, progress);

			chunk.Id = ++_nextId;
			_chunks.Add(chunk.Id, chunk);

			return chunk;
		}

		private void SpawnBushes(Dictionary<int, string> blocks, Chunk chunk, IProgress<string> progress)
		{
			for (var n = 0; n < 10; n++)
			{
				Console.WriteLine($"Planting bush (x{n + 1})...");
				var spawnPoint = chunk.FindSpawnPoint();
				chunk[ChunkLayer.Blocking, spawnPoint.X, spawnPoint.Y] = blocks.Single(x => x.Value == "Bush").Key;
			}
		}
	}
}
