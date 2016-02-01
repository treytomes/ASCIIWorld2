using System;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	public class BlockRegistry
	{
		private Dictionary<int, Block> _blocks;

		public BlockRegistry()
		{
			_blocks = new Dictionary<int, Block>();
		}

		public void RegisterBlock(int id, Block block)
		{
			if (_blocks.ContainsKey(id))
			{
				throw new Exception("The block id has already been defined.");
			}
			block.Id = id;
			_blocks[id] = block;
		}

		public Block GetById(int blockId)
		{
			return _blocks[blockId];
		}

		public bool IsDefined(int blockId)
		{
			return _blocks.ContainsKey(blockId);
		}

		public void Update(TimeSpan elapsed)
		{
			foreach (var block in _blocks.Values)
			{
				block.Update(elapsed);
			}
		}
	}
}
