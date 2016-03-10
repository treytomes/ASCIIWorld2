using ASCIIWorld.Data;
using CommonCore.Math;
using System;

namespace ASCIIWorld.Generation
{
	public abstract class BaseChunkGenerator
	{
		#region Methods

		public abstract Chunk Generate(IProgress<string> progress);

		protected void Fill(Chunk chunk, ChunkLayer layer, int blockId)
		{
			Fill(chunk, layer, new RectI(0, 0, chunk.Width, chunk.Height), blockId);
		}

		protected void Fill(Chunk chunk, ChunkLayer layer, RectI bounds, int blockId)
		{
			for (var row = bounds.Top; row <= bounds.Bottom; row++)
			{
				for (var column = bounds.Left; column <= bounds.Right; column++)
				{
					chunk[layer, column, row] = blockId;
				}
			}
		}

		protected void DrawVerticalLine(Chunk chunk, ChunkLayer layer, int top, int bottom, int x, int blockId)
		{
			for (var row = top; row <= bottom; row++)
			{
				chunk[layer, x, row] = blockId;
			}
		}

		protected void DrawHorizontalLine(IChunkAccess chunk, ChunkLayer layer, int y, int left, int right, int blockId)
		{
			for (var column = left; column <= right; column++)
			{
				chunk[layer, column, y] = blockId;
			}
		}

		#endregion
	}
}
