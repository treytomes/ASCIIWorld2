namespace ASCIIWorld.Data
{
	public interface IChunkAccess
	{
		int this[ChunkLayer layer, int blockX, int blockY] { get; set; }

		bool CanSeeSky(BlockRegistry blocks, ChunkLayer layer, int blockX, int blockY);

		ChunkLayer GetHighestVisibleLayer(int blockX, int blockY);
	}
}