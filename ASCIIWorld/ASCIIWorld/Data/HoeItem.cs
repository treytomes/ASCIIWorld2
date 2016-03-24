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

		public HoeItem(ContentManager content, BlockRegistry blocks)
			: base(GenerateRenderable(content))
		{

			_dirtId = blocks.GetByName("Dirt").Id;
			_grassId = blocks.GetByName("Grass").Id;
			_tilledSoil = blocks.GetByName("Tilled Soil").Id;
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
