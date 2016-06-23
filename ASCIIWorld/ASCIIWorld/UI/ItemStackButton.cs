using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using GameCore;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace ASCIIWorld.UI
{
	// TODO: Make an ItemStack button.  Show the stack size in yellow in the top-right corner (or nothing if the stack size = 1).
	public class ItemStackButton : IconButton
	{
		public ItemStackButton(IGameWindow window, Camera<OrthographicProjection> camera, Vector2 position, ItemStack itemStack, Key? hotkey = null)
			: base(window, camera, position, null, hotkey)
		{
			ItemStack = itemStack;
		}

		public override IRenderable Renderable
		{
			get
			{
				if (ItemStack == null)
				{
					return null;
				}
				else
				{
					return ItemRenderManager.Instance.GetRendererFor(ItemRegistry.Instance.GetById(ItemStack.ItemId));
				}
			}
			protected set
			{
				base.Renderable = value;
			}
		}

		public virtual ItemStack ItemStack { get; private set; }

		protected override void RenderContent(ITessellator tessellator)
		{
			base.RenderContent(tessellator);

			if ((ItemStack != null) && (ItemStack.StackSize > 1))
			{
				tessellator.BindColor(Color.Yellow);
				tessellator.Translate(-6, 16, -1);
				ASCII.RenderText(tessellator, ItemStack.StackSize.ToString());
				tessellator.Translate(6, -16, 1);
			}
		}
	}
}