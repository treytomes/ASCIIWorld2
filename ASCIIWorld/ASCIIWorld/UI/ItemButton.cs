using ASCIIWorld.Data;
using GameCore;
using GameCore.Rendering;
using OpenTK;

namespace ASCIIWorld.UI
{
	public class ItemButton : IconButton<Item>
	{
		public ItemButton(Camera<OrthographicProjection> camera, Vector2 position, Item renderable)
			: base(camera, position, renderable)
		{
		}
	}
}