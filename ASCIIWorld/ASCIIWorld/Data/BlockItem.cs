namespace ASCIIWorld.Data
{
	public class BlockItem : Item
	{
		public BlockItem(int blockId)
			: base(BlockRegistry.Instance.GetById(blockId).Name)
		{
			BlockId = blockId;
		}

		public int BlockId { get; private set; }

		public override void Use(Level level, ChunkLayer layer, int blockX, int blockY, out bool isConsumed)
		{
			base.Use(level, layer, blockX, blockY, out isConsumed);

			// TODO: I'm not really happy with the organization of this function.

			if ((layer == ChunkLayer.Background) && (level[layer, blockX, blockY] == 0))
			{
				// The background is the highest layer, and it's empty, so place the block there to fill the hole.
				level[layer, blockX, blockY] = BlockId;
				isConsumed = true;
			}
			else
			{
				if (layer.HasLayerAbove()) // you cannot place blocks above the ceiling
				{
					var useLayer = layer.GetLayerAbove();
					if (level[useLayer, blockX, blockY] == 0)
					{
						level[useLayer, blockX, blockY] = BlockId;
						isConsumed = true;
					}
				}
			}
		}
	}
}
