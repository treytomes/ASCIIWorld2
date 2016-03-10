using System;

namespace ASCIIWorld.Data.Generation
{
	public interface IChunkGenerator
	{
		Chunk Generate(IProgress<string> progress);
	}
}
