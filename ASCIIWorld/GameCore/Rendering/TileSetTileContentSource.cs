using System.Drawing;

namespace GameCore.Rendering
{
	public class TileSetTileContentSource : ITileContentSource
	{
		private BitmapTileSet _tileSet;
		private int _tileIndex;

		public TileSetTileContentSource(BitmapTileSet tileSet, int tileIndex)
		{
			_tileSet = tileSet;
			_tileIndex = tileIndex;
		}

		public int Width
		{
			get
			{
				return _tileSet.Width;
			}
		}

		public int Height
		{
			get
			{
				return _tileSet.Height;
			}
		}

		public void Render(Graphics graphics, int x, int y)
		{
			_tileSet.Render(graphics, _tileIndex, x, y);
		}
	}
}