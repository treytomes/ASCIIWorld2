using GameCore.IO;
using GameCore.Rendering;
using ASCIIWorld.Rendering;
using OpenTK;

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

		public override void Use(Level level, ChunkLayer layer, int blockX, int blockY)
		{
			base.Use(level, layer, blockX, blockY);

			var blockId = level[layer, blockX, blockY];
			if (blockId > 0)
			{
				var blockEntity = new BlockEntity(blockId);
				blockEntity.MoveTo(level, new Vector2(blockX, blockY));
				level.AddEntity(blockEntity);

				level[layer, blockX, blockY] = 0;
			}
		}

		#endregion
	}
}
