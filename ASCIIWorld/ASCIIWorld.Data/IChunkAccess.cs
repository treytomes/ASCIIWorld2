namespace ASCIIWorld.Data
{
	public interface IChunkAccess
	{
		int this[ChunkLayer layer, int blockX, int blockY] { get; set; }

		bool CanSeeSky(ChunkLayer layer, int blockX, int blockY);
	}
}