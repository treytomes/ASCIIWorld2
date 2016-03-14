using ASCIIWorld.Data;
using GameCore;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

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

		public void Render(Camera<OrthographicProjection> camera, IChunkAccess chunk)
		{
			var topLeft = Vector3.Transform(new Vector3(camera.Projection.Left, camera.Projection.Top, 0), camera.ModelViewMatrix.Inverted());
			var bottomRight = Vector3.Transform(new Vector3(camera.Projection.Right, camera.Projection.Bottom, 0), camera.ModelViewMatrix.Inverted());

			var minX = (int)Math.Floor(topLeft.X);
			var maxX = (int)Math.Ceiling(bottomRight.X);
			var minY = (int)Math.Floor(topLeft.Y);
			var maxY = (int)Math.Ceiling(bottomRight.Y);

			_tessellator.LoadIdentity();
			_tessellator.Begin(PrimitiveType.Quads);
			
			RenderLayer(_tessellator, chunk, ChunkLayer.Background, minX, maxX, minY, maxY);
			RenderLayer(_tessellator, chunk, ChunkLayer.Floor, minX, maxX, minY, maxY);

			// TODO: Render entities here.

			RenderLayer(_tessellator, chunk, ChunkLayer.Blocking, minX, maxX, minY, maxY);
			RenderLayer(_tessellator, chunk, ChunkLayer.Ceiling, minX, maxX, minY, maxY);

			_tessellator.End();
		}

		private void RenderLayer(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int minX, int maxX, int minY, int maxY)
		{
			for (var y = minY; y < maxY; y++)
			{
				for (var x = minX; x < maxX; x++)
				{
					if ((chunk[layer, x, y] > 0) && chunk.CanSeeSky(_blocks, layer, x, y))
					{
						//var position = tessellator.WorldToScreenPoint(new Vector3(column, row, (int)layer));
						var position = new Vector3(x, y, -1 * (int)layer);

						tessellator.Translate(position);
						var id = _blocks.GetById(chunk[layer, x, y]);
						_blocks.GetById(chunk[layer, x, y]).Renderer.Render(tessellator, chunk, layer, x, y);

						tessellator.Translate(-position);
					}
				}
			}
		}

		#endregion
	}
}
