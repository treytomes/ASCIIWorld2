using ASCIIWorld.Data;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using System;
using System.Drawing;

namespace ASCIIWorld.Rendering
{
	public class EntityRenderManager
	{
		#region Fields

		private AtlasTileSet _tiles;
		private Tile _playerTile;

		#endregion

		#region Constructors

		static EntityRenderManager()
		{
			Instance = new EntityRenderManager();
		}

		private EntityRenderManager()
		{
		}

		#endregion

		#region Properties

		public static EntityRenderManager Instance { get; private set; }

		#endregion

		#region Methods

		public void LoadContent(ContentManager content)
		{
			_tiles = content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml");
			_playerTile = new Tile(_tiles, "Player");
		}

		public void Render(ITessellator tessellator, Entity entity)
		{
			if (entity is BlockEntity)
			{
				Render(tessellator, entity as BlockEntity);
			}
			else if (entity is PlayerEntity)
			{
				Render(tessellator, entity as PlayerEntity);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void Render(ITessellator tessellator, BlockEntity entity)
		{
			var color = tessellator.CurrentColor;
			tessellator.BindColor(Color.FromArgb(196, entity.IsSelected ? Color.Red : Color.White));
			tessellator.PushTransform();

			var origin = tessellator.Transform(Vector3.Zero);

			tessellator.LoadIdentity();
			tessellator.Scale(0.5f, 0.5f);
			tessellator.Translate(-0.25f, -0.25f); // center the rotation
			tessellator.Rotate(entity.Rotation, 0, 0, 1);
			tessellator.Translate(origin);
			tessellator.Translate(entity.Size, entity.Size); // center on the current tile position
			tessellator.Translate(entity.Position); // move to the entity's position

			BlockRegistry.Instance.GetById(entity.BlockID).Renderer.Render(tessellator);

			tessellator.PopTransform();
			tessellator.BindColor(color); // TODO: Is this still needed?
		}

		private void Render(ITessellator tessellator, PlayerEntity entity)
		{
			tessellator.PushTransform();

			var origin = tessellator.Transform(Vector3.Zero);

			tessellator.LoadIdentity();
			tessellator.Scale(entity.Size, entity.Size);
			tessellator.Translate(origin);
			tessellator.Translate(0.1f, 0.1f); // center on the current tile position

			tessellator.Translate(entity.Position); // move to the entity's position

			_playerTile.Render(tessellator);
			tessellator.PopTransform();
		}

		#endregion
	}
}
