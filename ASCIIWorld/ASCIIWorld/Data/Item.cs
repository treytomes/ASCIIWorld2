using ASCIIWorld.Rendering;
using GameCore.Rendering;

namespace ASCIIWorld.Data
{
	public class Item : IRenderable
	{
		#region Fields

		private IRenderable _renderable;

		#endregion

		#region Constructors

		public Item(IRenderable renderable)
		{
			_renderable = renderable;
		}

		#endregion

		#region Properties

		#endregion

		#region Methods

		/// <summary>
		/// Use this item on the selected location.
		/// </summary>
		public virtual void Use(Level level, ChunkLayer layer, int blockX, int blockY)
		{
		}

		/// <summary>
		/// Use this item on the specified entity.
		/// </summary>
		public virtual void Use(Entity entity)
		{
		}

		public virtual void Render(ITessellator tessellator)
		{
			_renderable.Render(tessellator);
		}

		#endregion
	}
}
