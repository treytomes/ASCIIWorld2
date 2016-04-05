using CommonCore.Math;
using OpenTK;
using System.Drawing;
using System.Drawing.Imaging;

namespace GameCore.Rendering
{
	public class TileSetTileContentSource : ITileContentSource
	{
		private BitmapTileSet _tileSet;
		private int _tileIndex;
		private Color _tint;
		private Vector2I _translate;
		private float _rotate;

		public TileSetTileContentSource(BitmapTileSet tileSet, int tileIndex, Color tint, float rotate, Vector2I translate)
		{
			_tileSet = tileSet;
			_tileIndex = tileIndex;
			_tint = tint;
			_translate = translate;
			_rotate = rotate;
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
			_tileSet.Render(graphics, _tileIndex, x + _translate.X, y + _translate.Y, _tint, _rotate);
		}
	}
}