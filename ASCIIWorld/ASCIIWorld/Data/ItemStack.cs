namespace ASCIIWorld.Data
{
	public class ItemStack
	{
		public ItemStack(int itemId, int stackSize = 1, int metadata = 0)
		{
			ItemId = itemId;
			StackSize = stackSize;
			Metadata = metadata;
		}

		public int ItemId { get; private set; }

		public int StackSize { get; private set; }

		public int Metadata { get; private set; }

		// TODO: Might need to pass in the owner entity, so that if a potion is used up, the player will get an empty bottle.
		public void Use(Level level, ChunkLayer layer, int blockX, int blockY)
		{
			var isConsumed = false;
			GetItem().Use(level, layer, blockX, blockY, out isConsumed);
			if (isConsumed)
			{
				StackSize--;
			}
		}

		public void Use(Entity target)
		{
			var isConsumed = false;
			ItemRegistry.Instance.GetById(ItemId).Use(target, out isConsumed);
			if (isConsumed)
			{
				StackSize--;
			}
		}

		public Item GetItem()
		{
			return ItemRegistry.Instance.GetById(ItemId);
		}

		public void AddToStack(int amount)
		{
			StackSize += amount;
		}

		public bool IsCompatibleWith(ItemStack other)
		{
			return (ItemId == other.ItemId) && (Metadata == other.Metadata);
		}
	}
}
