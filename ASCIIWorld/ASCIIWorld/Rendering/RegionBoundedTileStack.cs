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

		private TileSet _connectedWallTiles;
		private string _northWall;
		private string _eastWall;
		private string _southWall;
		private string _westWall;

		#endregion

		#region Constructors

		public RegionBoundedTileStack(IEnumerable<IRenderable> layers, TileSet connectedWallTileSet, Color outlineColor, string northWall, string eastWall, string southWall, string westWall)
			: base(layers)
		{
			OutlineColor = outlineColor;
			_connectedWallTiles = connectedWallTileSet;
			_northWall = northWall;
			_eastWall = eastWall;
			_southWall = southWall;
			_westWall = westWall;
		}

		#endregion

		#region Properties

		public Color OutlineColor { get; private set; }

		#endregion

		#region Methosd

		public override void Render(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int x, int y)
		{
			base.Render(tessellator, chunk, layer, x, y);

			tessellator.BindColor(Color.DimGray);
			if (chunk[layer, x - 1, y] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName(_westWall));
			}
			if (chunk[layer, x + 1, y] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName(_eastWall));
			}
			if (chunk[layer, x, y - 1] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName(_northWall));
			}
			if (chunk[layer, x, y + 1] != chunk[layer, x, y])
			{
				_connectedWallTiles.Render(tessellator, _connectedWallTiles.GetTileIndexFromName(_southWall));
			}
		}

		#endregion
	}
}
