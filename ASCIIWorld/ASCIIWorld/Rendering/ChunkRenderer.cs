using ASCIIWorld.Data;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace ASCIIWorld.Rendering
{
	public class ChunkRenderer
	{
		#region Fields

		private BlockRegistry _blocks;
		private ITessellator _tessellator;

		private AtlasTileSet _connectedWallTiles;

		#endregion

		#region Constructors

		public ChunkRenderer(Viewport viewport, BlockRegistry blocks)
		{
			_blocks = blocks;

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
		}

		#endregion

		#region Methods

		public void LoadContent(ContentManager content)
		{
			_connectedWallTiles = content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml");
		}

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
			var maxRow = chunk.Height; // (float)Math.Min(chunk.Rows, Math.Ceiling(bottomRight.Y));
			var minColumn = 0; // (float)Math.Max(0, Math.Floor(topLeft.X));
			var maxColumn = chunk.Width; // (float)Math.Min(chunk.Columns, Math.Ceiling(bottomRight.X));

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
			for (var y = minRow; y < maxRow; y++)
			{
				for (var x = minColumn; x < maxColumn; x++)
				{
					if (chunk[layer, (int)x, (int)y] > 0)
					{
						//var position = tessellator.WorldToScreenPoint(new Vector3(column, row, (int)layer));
						var position = new Vector3(x, y, -1 * (int)layer);

						tessellator.Translate(position);
						var id = _blocks.GetById(chunk[layer, (int)x, (int)y]);
						_blocks.GetById(chunk[layer, (int)x, (int)y]).Render(tessellator);

						tessellator.BindColor(Color.DimGray);
						if (chunk[layer, (int)x - 1, (int)y] != chunk[layer, (int)x, (int)y])
						{
							_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_W"));
						}
						if (chunk[layer, (int)x + 1, (int)y] != chunk[layer, (int)x, (int)y])
						{
							_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_E"));
						}
						if (chunk[layer, (int)x, (int)y - 1] != chunk[layer, (int)x, (int)y])
						{
							_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_N"));
						}
						if (chunk[layer, (int)x, (int)y + 1] != chunk[layer, (int)x, (int)y])
						{
							_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_S"));
						}

						tessellator.Translate(-position);
					}
				}
			}
		}

		#endregion
	}
}
