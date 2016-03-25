using System.Drawing;

namespace GameCore.Rendering
{
	public class TileSetTileContentSource : ITileContentSource
	{
		private BitmapTileSet _tileSet;
		private int _tileIndex;
		private Color _tint;

		public TileSetTileContentSource(BitmapTileSet tileSet, int tileIndex, Color tint)
		{
			_tileSet = tileSet;
			_tileIndex = tileIndex;
			_tint = tint;
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
			_tileSet.Render(graphics, _tileIndex, x, y, _tint);
		}
	}
}