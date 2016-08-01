using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Data
{
	public class WheatSeedItem : Item
	{
		#region Fields

		private int _tilledSoil;
		private int _wheatPlant;

		#endregion

		#region Constructors

		public WheatSeedItem()
			: base("Wheat Seed")
		{
			_tilledSoil = BlockRegistry.Instance.GetId("Tilled Soil");
			_wheatPlant = BlockRegistry.Instance.GetId("Wheat Plant");
		}

		#endregion

		#region Methods

		// TODO: I can make farmland; now I need something to plant.

		/// <summary>
		/// You can only till dirt or grass.
		/// </summary>
		public override void Use(Level level, ChunkLayer layer, int blockX, int blockY, out bool isConsumed)
		{
			base.Use(level, layer, blockX, blockY, out isConsumed);

			layer = level.GetHighestVisibleLayer(blockX, blockY);
			var blockId = level[layer, blockX, blockY];
			if ((blockId == _tilledSoil))
			{
				level[layer, blockX, blockY] = _wheatPlant;
				isConsumed = true;
			}

			// TODO: If durability <= 0, isConsumed = true.
		}

		#endregion
	}
}
