using ASCIIWorld.Data;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ASCIIWorld.Rendering
{
	public class ChunkRenderer
	{
		#region Fields

		private BlockRegistry _blocks;
		private ITessellator _tessellator;

		#endregion

		#region Constructors

		public ChunkRenderer(Viewport viewport, BlockRegistry blocks)
		{
			_blocks = blocks;

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
		}

		#endregion

		#region Methods

		public void Render(Viewport viewport, Chunk chunk)
		{
			_tessellator.LoadIdentity();

			//var zero = _tessellator.WorldToScreenPoint(Vector2.Zero);
			//var scale = (_tessellator.WorldToScreenPoint(Vector2.One) - zero);

			//var blocksPerColumn = (viewport.Width / scale.X) + 1;
			//var blocksPerRow = (viewport.Height / scale.Y) + 1;

			//var topLeft = _tessellator.ScreenToWorldPoint(Vector2.Zero) * 2;
			//var bottomRight = topLeft + new Vector2(blocksPerColumn, blocksPerRow);
			
			// This doesn't work.  I don't know why.  It really should work.
			//var bottomRight = _tessellator.ScreenToWorldPoint(new Vector2(projection.Viewport.Width, projection.Viewport.Height));

			_tessellator.Begin(PrimitiveType.Quads);

			var minRow = 0; // (float)Math.Max(0, Math.Floor(topLeft.Y));
			var maxRow = chunk.Rows; // (float)Math.Min(chunk.Rows, Math.Ceiling(bottomRight.Y));
			var minColumn = 0; // (float)Math.Max(0, Math.Floor(topLeft.X));
			var maxColumn = chunk.Columns; // (float)Math.Min(chunk.Columns, Math.Ceiling(bottomRight.X));

			//Console.WriteLine($"Viewport: {projection.Viewport.Left} --> {projection.Viewport.Right}");
			//Console.WriteLine($"H: {topLeft.X*24}-->{bottomRight.X * 24} ({bottomRight.X * 24 - topLeft.X * 24}), V: {topLeft.Y}-->{bottomRight.Y} ({bottomRight.Y - topLeft.Y})");
			//Console.WriteLine($"X: {minColumn}-->{maxColumn} ({maxColumn - minColumn}), Y: {minRow}-->{maxRow} ({maxRow - minRow})");
			
			RenderLayer(_tessellator, chunk, ChunkLayer.Background, minRow, maxRow, minColumn, maxColumn);
			RenderLayer(_tessellator, chunk, ChunkLayer.Floor, minRow, maxRow, minColumn, maxColumn);

			// TODO: Render entities here.

			RenderLayer(_tessellator, chunk, ChunkLayer.Blocking, minRow, maxRow, minColumn, maxColumn);
			RenderLayer(_tessellator, chunk, ChunkLayer.Ceiling, minRow, maxRow, minColumn, maxColumn);

			_tessellator.End();
		}

		private void RenderLayer(ITessellator tessellator, Chunk chunk, ChunkLayer layer, float minRow, float maxRow, float minColumn, float maxColumn)
		{
			for (var row = minRow; row < maxRow; row++)
			{
				for (var column = minColumn; column < maxColumn; column++)
				{
					if (chunk[layer, (int)row, (int)column] > 0)
					{
						//var position = tessellator.WorldToScreenPoint(new Vector3(column, row, (int)layer));
						var position = new Vector3(column, row, -1 * (int)layer);

						tessellator.Translate(position);
						var id = _blocks.GetById(chunk[layer, (int)row, (int)column]);
						_blocks.GetById(chunk[layer, (int)row, (int)column]).Render(tessellator);
						tessellator.Translate(-position);
					}
				}
			}
		}

		#endregion
	}
}
