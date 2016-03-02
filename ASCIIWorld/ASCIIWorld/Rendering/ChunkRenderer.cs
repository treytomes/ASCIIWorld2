using ASCIIWorld.Data;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
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

		public void Render(Camera<OrthographicProjection> camera, Chunk chunk)
		{
			_tessellator.LoadIdentity();

			var topLeft = Vector3.Transform(new Vector3(camera.Projection.Left, camera.Projection.Top, 0), camera.ModelViewMatrix.Inverted());
			var bottomRight = Vector3.Transform(new Vector3(camera.Projection.Right, camera.Projection.Bottom, 0), camera.ModelViewMatrix.Inverted());

			_tessellator.Begin(PrimitiveType.Quads);

			var minRow = (float)Math.Floor(topLeft.Y);
			var maxRow = (float)Math.Ceiling(bottomRight.Y);
			var minColumn = (float)Math.Floor(topLeft.X);
			var maxColumn = (float)Math.Ceiling(bottomRight.X);
			
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
						_blocks.GetById(chunk[layer, (int)x, (int)y]).Renderer.Render(tessellator);

						// TODO: Borders should not be drawn around every type of block.
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
