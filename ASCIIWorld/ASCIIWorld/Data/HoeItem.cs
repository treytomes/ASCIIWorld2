using GameCore.IO;
using GameCore.Rendering;
using System.Drawing;
using ASCIIWorld.Rendering;
using OpenTK;

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
			_dirtId = BlockRegistry.Instance.GetId("Dirt");
			_grassId = BlockRegistry.Instance.GetId("Grass");
			_tilledSoil = BlockRegistry.Instance.GetId("Tilled Soil");
		}

		#endregion

		#region Methods

		public static IRenderable GenerateRenderable(ContentManager content)
		{
			var ascii = content.Load<TileSet>("TileSets/UI-ASCII.xml");

			var rod = new Tile(ascii, Color.Brown, 179);
			var head = new Tile(ascii, Color.Gray, (int)'`');
			head.Transform = Transformer.New().SetTranslation(23, -4).SetRotation(90).SetMirrorY(true).Build();

			return new TileStack(new[] { rod, head });
		}

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

		public override void Render(ITessellator tessellator)
		{
			base.Render(tessellator);
		}

		#endregion
	}
}
