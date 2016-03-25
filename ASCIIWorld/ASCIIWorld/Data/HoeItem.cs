using GameCore.IO;
using GameCore.Rendering;
using ASCIIWorld.Rendering;

namespace ASCIIWorld.Data
{
	public class HoeItem : Item
	{
		#region Fields

		private int _dirtId;
		private int _grassId;
		private int _tilledSoil;
		
		#endregion

		#region Constructors

		public HoeItem(ContentManager content)
			: base(new Tile(content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml"), "Hoe"))
		{
			_dirtId = BlockRegistry.Instance.GetId("Dirt");
			_grassId = BlockRegistry.Instance.GetId("Grass");
			_tilledSoil = BlockRegistry.Instance.GetId("Tilled Soil");
		}

		#endregion

		#region Methods

		// TODO: I can make farmland; now I need something to plant.

		/// <summary>
		/// You can only till dirt or grass.
		/// </summary>
		public override void Use(IChunkAccess chunk, ChunkLayer layer, int blockX, int blockY)
		{
			base.Use(chunk, layer, blockX, blockY);
			layer = chunk.GetHighestVisibleLayer(blockX, blockY);
			var blockId = chunk[layer, blockX, blockY];
			if ((blockId == _dirtId) || (blockId == _grassId))
			{
				chunk[layer, blockX, blockY] = _tilledSoil;
			}
		}

		#endregion
	}
}
