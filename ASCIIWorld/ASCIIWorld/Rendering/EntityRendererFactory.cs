using ASCIIWorld.Data;
using GameCore.Rendering;
using OpenTK;
using System.Drawing;

namespace ASCIIWorld.Rendering
{
	public class EntityRendererFactory
	{
		#region Constructors

		static EntityRendererFactory()
		{
			Instance = new EntityRendererFactory();
		}

		private EntityRendererFactory()
		{
		}

		#endregion

		#region Properties

		public static EntityRendererFactory Instance { get; private set; }

		#endregion

		#region Methods

		public void Render(ITessellator tessellator, Entity entity)
		{
			if (entity is BlockEntity)
			{
				Render(tessellator, entity as BlockEntity);
			}
		}

		private void Render(ITessellator tessellator, BlockEntity entity)
		{
			tessellator.BindColor(Color.FromArgb(196, entity.IsSelected ? Color.Red : Color.White));
			tessellator.PushTransform();

			var origin = tessellator.Transform(Vector3.Zero);

			tessellator.LoadIdentity();
			tessellator.Scale(0.5f, 0.5f);
			tessellator.Translate(-0.25f, -0.25f); // center the rotation
			tessellator.Rotate(entity.Rotation, 0, 0, 1);
			tessellator.Translate(origin);
			tessellator.Translate(0.5f, 0.5f); // center on the current tile position
			tessellator.Translate(entity.Position); // move to the entity's position

			BlockRegistry.Instance.GetById(entity.BlockID).Renderer.Render(tessellator);

			tessellator.PopTransform();
		}

		#endregion
	}
}
