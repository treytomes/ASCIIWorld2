using GameCore.IO;
using GameCore.Rendering;
using System.Drawing;
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
			: base(GenerateRenderable(content))
		{
			// TODO: Find a better way to get id by name.
			_dirtId = BlockRegistry.Instance.GetByName("Dirt").Id;
			_grassId = BlockRegistry.Instance.GetByName("Grass").Id;
			_tilledSoil = BlockRegistry.Instance.GetByName("Tilled Soil").Id;
		}

		#endregion

		#region Methods

		public static IRenderable GenerateRenderable(ContentManager content)
		{
			var ascii = content.Load<TileSet>("TileSets/UI-ASCII.xml");
			return new TileStack(new[] {
				new Tile(ascii, Color.Brown,  196),
				new Tile(ascii, Color.Gray, (int)'`')
			});
		}

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
