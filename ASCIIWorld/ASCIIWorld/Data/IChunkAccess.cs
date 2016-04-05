using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	public interface IChunkAccess
	{
		int this[ChunkLayer layer, int blockX, int blockY] { get; set; }

		IEnumerable<Entity> Entities { get; }

		bool CanSeeSky(ChunkLayer layer, int blockX, int blockY);

		ChunkLayer GetHighestVisibleLayer(int blockX, int blockY);

		void AddEntity(Entity entity);
		void RemoveEntity(Entity entity);
	}
}