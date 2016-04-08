using OpenTK;

namespace ASCIIWorld.Data
{
	public class PickaxeItem : Item
	{
		#region Constructors

		public PickaxeItem()
			: base("Pickaxe")
		{
		}

		#endregion

		#region Methods

		// TODO: When farmland is broken, it should drop a dirt block.

		public override void Use(Level level, ChunkLayer layer, int blockX, int blockY, out bool isConsumed)
		{
			base.Use(level, layer, blockX, blockY, out isConsumed);

			var blockId = level[layer, blockX, blockY];
			if (blockId > 0)
			{
				level[layer, blockX, blockY] = 0;

				var blockEntity = new BlockEntity(blockId);
				blockEntity.MoveTo(level, new Vector2(blockX, blockY));
				level.AddEntity(blockEntity);
			}

			// TODO: If durability <= 0, isConsumed = true.
			isConsumed = false;
		}

		#endregion
	}
}
