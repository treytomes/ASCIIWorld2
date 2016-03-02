using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASCIIWorld.Data;
using GameCore.Rendering;

namespace ASCIIWorld.Rendering
{
	public class RegionBoundedTileStack : TileStack
	{
		#region Fields

		private AtlasTileSet _connectedWallTiles;

		#endregion

		#region Constructors

		public RegionBoundedTileStack(IEnumerable<IRenderable> layers, AtlasTileSet connectedWallTileSet, Color outlineColor)
			: base(layers)
		{
			OutlineColor = outlineColor;
			_connectedWallTiles = connectedWallTileSet;
		}

		#endregion

		#region Properties

		public Color OutlineColor { get; private set; }

		#endregion

		#region Methosd

		public override void Render(ITessellator tessellator, Chunk chunk, ChunkLayer layer, int x, int y)
		{
			base.Render(tessellator, chunk, layer, x, y);

			tessellator.BindColor(Color.DimGray);
			if (chunk[layer, x - 1, y] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_W"));
			}
			if (chunk[layer, x + 1, y] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_E"));
			}
			if (chunk[layer, x, y - 1] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_N"));
			}
			if (chunk[layer, x, y + 1] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName("ConnectedWall_S"));
			}
		}

		#endregion
	}
}
