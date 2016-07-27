using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	public interface IChunkAccess
	{
		float AmbientLightLevel { get; }

		int this[ChunkLayer layer, int blockX, int blockY] { get; set; }

		IEnumerable<Entity> Entities { get; }

		void SetMetadata(ChunkLayer layer, int x, int y, int metadata);
		int GetMetadata(ChunkLayer layer, int x, int y);

		bool CanSeeSky(ChunkLayer layer, int blockX, int blockY);
		bool IsBlockedAt(int blockX, int blockY);

		ChunkLayer GetHighestVisibleLayer(int blockX, int blockY);

		void AddEntity(Entity entity);
		void RemoveEntity(Entity entity);
	}
}