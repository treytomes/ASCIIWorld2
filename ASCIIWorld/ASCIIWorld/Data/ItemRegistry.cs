using CommonCore;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	public class ItemRegistry : ObjectRegistry<Item>
	{
		private Dictionary<int, int> _blockToItemId;

		static ItemRegistry()
		{
			Instance = new ItemRegistry();
		}

		private ItemRegistry()
		{
			_blockToItemId = new Dictionary<int, int>();
		}

		public static ItemRegistry Instance { get; private set; }

		public void Initialize()
		{
			Register(new PickaxeItem());
			Register(new HoeItem());
			Register(new WheatSeedItem());

			foreach (var block in BlockRegistry.Instance)
			{
				var item = new BlockItem(block.Id);
				Register(item);
				_blockToItemId.Add(block.Id, item.Id);
			}
		}

		public Item GetByBlockId(int blockId)
		{
			return GetById(_blockToItemId[blockId]);
		}
	}
}
