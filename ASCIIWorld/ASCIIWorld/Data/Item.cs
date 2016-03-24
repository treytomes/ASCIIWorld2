using ASCIIWorld.Rendering;
using GameCore.Rendering;
using System.Drawing;

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
		public virtual void Use(IChunkAccess chunk, ChunkLayer layer, int blockX, int blockY)
		{
		}

		// TODO: public virtual void Use(Entity entity) { }

		public void Render(ITessellator tessellator)
		{
			_renderable.Render(tessellator);
		}

		#endregion
	}
}
