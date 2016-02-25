using ASCIIWorld.Data;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace ASCIIWorld.Rendering
{
	public class ChunkRenderer
	{
		#region Constants

		private const float BLOCK_SCALE = 24;

		#endregion

		#region Fields

		private BlockRegistry _blocks;
		private ITessellator _tessellator;

		#endregion

		#region Constructors

		public ChunkRenderer(BlockRegistry blocks)
		{
			_blocks = blocks;
			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
		}

		#endregion

		#region Methods

		public void Render(Chunk chunk, OrthographicProjection projection)
		{
			projection.Apply();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Scale(BLOCK_SCALE, BLOCK_SCALE);

			var minRow = (float)Math.Floor(projection.Top / BLOCK_SCALE);
			var maxRow = (float)Math.Ceiling(projection.Bottom / BLOCK_SCALE);
			var minColumn = (float)Math.Floor(projection.Left / BLOCK_SCALE);
			var maxColumn = (float)Math.Ceiling(projection.Right / BLOCK_SCALE);

			RenderLayer(chunk, ChunkLayer.Background, minRow, maxRow, minColumn, maxColumn);
			RenderLayer(chunk, ChunkLayer.Floor, minRow, maxRow, minColumn, maxColumn);

			// TODO: Render entities here.

			RenderLayer(chunk, ChunkLayer.Blocking, minRow, maxRow, minColumn, maxColumn);
			RenderLayer(chunk, ChunkLayer.Ceiling, minRow, maxRow, minColumn, maxColumn);

			_tessellator.End();
		}

		private void RenderLayer(Chunk chunk, ChunkLayer layer, float minRow, float maxRow, float minColumn, float maxColumn)
		{
			for (var row = minRow; row < maxRow; row++)
			{
				for (var column = minColumn; column < maxColumn; column++)
				{
					if (chunk[layer, (int)row, (int)column] > 0)
					{
						var position = _tessellator.Transform(new Vector3(column, row, 0));
						position.Z = (int)layer;

						_tessellator.Translate(position);
						_blocks.GetById(chunk[layer, (int)row, (int)column]).Render(_tessellator);
						_tessellator.Translate(-position);
					}
				}
			}
		}

		#endregion
	}
}
