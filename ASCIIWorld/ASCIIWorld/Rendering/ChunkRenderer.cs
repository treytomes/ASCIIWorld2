using ASCIIWorld.Data;
using CommonCore.Math;
using GameCore;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

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

			RenderLightMap(_tessellator, chunk, minX, maxX, minY, maxY);

			_tessellator.End();
		}

		/// <summary>
		/// Renders a light map around each entity.
		/// </summary>
		/// <remarks>
		/// Also takes line-of-sight into account.
		/// </remarks>
		private void RenderLightMap(ITessellator tessellator, IChunkAccess chunk, int minX, int maxX, int minY, int maxY)
		{
			tessellator.PushTransform();
			tessellator.Translate(0, 0, -1 * (int)ChunkLayer.Lights);
			tessellator.BindTexture(null);

			int[,] lightMap = new int[maxY - minY + 1, maxX - minX + 1];
			foreach (var entity in chunk.Entities)
			{
				var originX = (int)(entity.Position.X + 0.5f);
				var originY = (int)(entity.Position.Y + 0.5f);

				var considered = new List<Vector2I>();
				//for (var angle = 0.0f; angle < 360.0f; angle += (9.0f - distance)) // hit more angles as you move further out
				for (var angle = 0.0f; angle < 360.0f; angle += 1.0f)
				{
					for (var distance = 0.0f; distance < 8.0f; distance++)
					{
						var x = (int)(originX + distance * Math.Cos(OpenTK.MathHelper.DegreesToRadians(angle)));
						var y = (int)(originY + distance * Math.Sin(OpenTK.MathHelper.DegreesToRadians(angle)));
						var vector = new Vector2I(y - minY, x - minX);
						if (!considered.Contains(vector))
						{
							considered.Add(vector);
							
							//var alpha = (8.0f - distance) / 8.0f;
							var alpha = 1.0f / Math.Pow((distance + 1) / 4.0f, 2); // The 4.0 represents the light intensity.

							lightMap[y - minY, x - minX] = OpenTK.MathHelper.Clamp((int)(lightMap[y - minY, x - minX] + alpha * 255.0f), 0, 255);
						}

						if (chunk[ChunkLayer.Blocking, x, y] != 0)
						{
							break;
						}
					}
				}
			}

			for (var y = 0; y < lightMap.GetLength(0); y++)
			{
				for (var x = 0; x < lightMap.GetLength(1); x++)
				{
					tessellator.BindColor(Color.FromArgb(255 - lightMap[y, x], 0, 0, 0));
					tessellator.Translate(minX + x, minY + y);
					tessellator.AddPoint(0, 0);
					tessellator.AddPoint(0, 1);
					tessellator.AddPoint(1, 1);
					tessellator.AddPoint(1, 0);
					tessellator.Translate(-(minX + x), -(minY + y));
				}
			}

			tessellator.PopTransform();
		}

		private void RenderEntities(ITessellator tessellator, IChunkAccess chunk, int minX, int maxX, int minY, int maxY)
		{
			tessellator.Translate(0, 0, -1 * (int)ChunkLayer.Blocking);

			foreach (var entity in chunk.Entities)
			{
				if (CommonCore.Math.MathHelper.IsInRange(entity.Position.X, minX, maxX + 1) &&
					CommonCore.Math.MathHelper.IsInRange(entity.Position.Y, minY, maxY + 1))
				{
					tessellator.BindColor(Color.White);
					EntityRenderManager.Instance.Render(tessellator, entity);
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
