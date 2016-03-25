using GameCore.IO;
using GameCore.Rendering;
using ASCIIWorld.Rendering;

namespace ASCIIWorld.Data
{
	public class PickaxeItem : Item
	{
		#region Constructors

		public PickaxeItem(ContentManager content)
			: base(new Tile(content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml"), "Pickaxe"))
		{
		}

		#endregion

		#region Methods

		public override void Use(IChunkAccess chunk, ChunkLayer layer, int blockX, int blockY)
		{
			base.Use(chunk, layer, blockX, blockY);
			chunk[layer, blockX, blockY] = 0;
		}

		#endregion
	}
}
