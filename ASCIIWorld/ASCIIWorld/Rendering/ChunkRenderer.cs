using ASCIIWorld.Data;
using GameCore;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Linq;

namespace ASCIIWorld.Rendering
{
	public class ChunkRenderer
	{
		#region Fields
		
		private ITessellator _tessellator;

		#endregion

		#region Constructors

		public ChunkRenderer(Viewport viewport)
		{
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

			_tessellator.BindColor(Color.White);

			_tessellator.LoadIdentity();
			_tessellator.Begin(PrimitiveType.Quads);
			
			RenderLayer(_tessellator, chunk, ChunkLayer.Background, minX, maxX, minY, maxY);
			RenderLayer(_tessellator, chunk, ChunkLayer.Floor, minX, maxX, minY, maxY);

			RenderEntities(_tessellator, chunk, minX, maxX, minY, maxY);

			RenderLayer(_tessellator, chunk, ChunkLayer.Blocking, minX, maxX, minY, maxY);
			RenderLayer(_tessellator, chunk, ChunkLayer.Ceiling, minX, maxX, minY, maxY);

			_tessellator.End();
		}

		private void RenderEntities(ITessellator tessellator, IChunkAccess chunk, int minX, int maxX, int minY, int maxY)
		{
			tessellator.Translate(0, 0, -1 * (int)ChunkLayer.Blocking);
			
			foreach (var entity in chunk.Entities)
			{
				if (CommonCore.Math.MathHelper.IsInRange(entity.Position.X, minX, maxX + 1) &&
					CommonCore.Math.MathHelper.IsInRange(entity.Position.Y, minY, maxY + 1))
				{
					EntityRendererFactory.Instance.Render(tessellator, entity);
				}
			}

			tessellator.Translate(0, 0, (int)ChunkLayer.Blocking);
		}

		private void RenderLayer(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int minX, int maxX, int minY, int maxY)
		{
			for (var y = minY; y < maxY; y++)
			{
				for (var x = minX; x < maxX; x++)
				{
					if ((chunk[layer, x, y] > 0) && chunk.CanSeeSky(layer, x, y))
					{
						//var position = tessellator.WorldToScreenPoint(new Vector3(column, row, (int)layer));
						var position = new Vector3(x, y, -1 * (int)layer);

						tessellator.Translate(position);
						var id = BlockRegistry.Instance.GetById(chunk[layer, x, y]);
						BlockRegistry.Instance.GetById(chunk[layer, x, y]).Renderer.Render(tessellator, chunk, layer, x, y);

						tessellator.Translate(-position);
					}
				}
			}
		}

		#endregion
	}
}
