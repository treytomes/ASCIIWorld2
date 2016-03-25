using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameCore.Rendering
{
	public class TileStackContentSource : ITileContentSource
	{
		private ITileContentSource[] _tiles;

		public TileStackContentSource(IEnumerable<ITileContentSource> tiles)
		{
			_tiles = tiles.ToArray();
			foreach (var tile in tiles)
			{
				if ((tile.Width != Width) || (tile.Height != Height))
				{
					throw new Exception("All tiles is the stack must be the same size.");
				}
			}
		}

		public int Width
		{
			get
			{
				return _tiles[0].Width;
			}
		}

		public int Height
		{
			get
			{
				return _tiles[0].Height;
			}
		}

		public void Render(Graphics graphics, int x, int y)
		{
			foreach (var tile in _tiles)
			{
				tile.Render(graphics, x, y);
			}
		}
	}
}
