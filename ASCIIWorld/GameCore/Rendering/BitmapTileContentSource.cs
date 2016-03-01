using System.Drawing;

namespace GameCore.Rendering
{
	public class BitmapTileContentSource : ITileContentSource
	{
		private Bitmap _bitmap;
		private RectangleF _sourceRectangle;

		public BitmapTileContentSource(Bitmap bitmap, RectangleF? sourceRectangle = null)
		{
			_bitmap = bitmap;
			_sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new RectangleF(0, 0, _bitmap.Width, _bitmap.Height);
		}

		public int Width
		{
			get
			{
				return _bitmap.Width;
			}
		}

		public int Height
		{
			get
			{
				return _bitmap.Height;
			}
		}

		public void Render(Graphics graphics, int x, int y)
		{
			graphics.DrawImage(_bitmap, x, y, _sourceRectangle, GraphicsUnit.Pixel);
		}
	}
}