using ASCIIWorld.Data;
using GameCore.IO;
using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Rendering
{
	public class ItemRenderManager
	{
		#region Fields

		private IRenderable _pickaxe;
		private IRenderable _hoe;
		private IRenderable _wheatSeed;

		#endregion

		#region Constructors

		static ItemRenderManager()
		{
			Instance = new ItemRenderManager();
		}

		private ItemRenderManager()
		{
		}

		#endregion

		#region Properties

		public static ItemRenderManager Instance { get; private set; }

		#endregion

		#region Methods

		public void LoadContent(ContentManager content)
		{
			_pickaxe = new Tile(content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml"), "Pickaxe");
			_hoe = new Tile(content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml"), "Hoe");
			_wheatSeed = new Tile(content.Load<AtlasTileSet>("TileSets/SampleBlocks.xml"), "WheatSeed");
		}

		public void Render(ITessellator tessellator, Item item)
		{
			GetRendererFor(item)?.Render(tessellator);
		}

		public IRenderable GetRendererFor(Item item)
		{
			if (item == null)
			{
				return null;
			}
			else if (item is PickaxeItem)
			{
				return _pickaxe;
			}
			else if (item is HoeItem)
			{
				return _hoe;
			}
			else if (item is WheatSeedItem)
			{
				return _wheatSeed;
			}
			else if (item is BlockItem)
			{
				return BlockRegistry.Instance.GetById((item as BlockItem).BlockId).Renderer;
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
