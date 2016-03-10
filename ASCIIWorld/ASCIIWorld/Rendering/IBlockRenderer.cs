using ASCIIWorld.Data;
using GameCore.Rendering;

namespace ASCIIWorld.Rendering
{
	public interface IBlockRenderer : IRenderable, IUpdateable
	{
		/// <summary>
		/// Render the block, taking into account it's position in the chunk.
		/// </summary>
		void Render(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int x, int y);
	}
}
