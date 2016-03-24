using GameCore.IO;
using GameCore.Rendering;
using System.Drawing;
using ASCIIWorld.Rendering;

namespace ASCIIWorld.Data
{
	public class PickaxeItem : Item
	{
		#region Constructors

		public PickaxeItem(ContentManager content)
			: base(GenerateRenderable(content))
		{

		}

		#endregion

		#region Methods

		public static IRenderable GenerateRenderable(ContentManager content)
		{
			var ascii = content.Load<TileSet>("TileSets/UI-ASCII.xml");
			return new TileStack(new[] {
				new Tile(ascii, Color.Brown,  196),
				new Tile(ascii, Color.Gray, (int)'(')
			});
		}

		public override void Use(IChunkAccess chunk, ChunkLayer layer, int blockX, int blockY)
		{
			base.Use(chunk, layer, blockX, blockY);
			chunk[layer, blockX, blockY] = 0;
		}

		#endregion
	}
}
