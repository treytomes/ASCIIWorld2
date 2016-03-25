using GameCore.IO;
using GameCore.Rendering;
using System.Drawing;
using ASCIIWorld.Rendering;

namespace ASCIIWorld.Data
{
	public class PickaxeItem : Item
	{
		#region Constructors

		public PickaxeItem(ContentManager content)
			: base(GenerateRenderable(content))
		{

		}

		#endregion

		#region Methods

		public static IRenderable GenerateRenderable(ContentManager content)
		{
			var ascii = content.Load<TileSet>("TileSets/UI-ASCII.xml");
			var rod = new Tile(ascii, Color.Brown, 196);
			var head = new Tile(ascii, Color.Gray, (int)'(');
			head.Transform = Transformer.New().SetTranslation(-2, 1).Build();
			return new TileStack(new[] { rod, head });
		}

		public override void Use(IChunkAccess chunk, ChunkLayer layer, int blockX, int blockY)
		{
			base.Use(chunk, layer, blockX, blockY);
			chunk[layer, blockX, blockY] = 0;
		}

		#endregion
	}
}
