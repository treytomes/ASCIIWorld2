using System.Drawing;
using System.Drawing.Imaging;

namespace GameCore.Rendering
{
	public class BitmapTileContentSource : ITileContentSource
	{
		private Bitmap _bitmap;
		private RectangleF _sourceRectangle;
		private Color _tint;

		public BitmapTileContentSource(Bitmap bitmap, Color tint, RectangleF? sourceRectangle = null)
		{
			_bitmap = bitmap;
			_tint = tint;
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
			var imageAttrs = new ImageAttributes();
			var m = new ColorMatrix(new float[][] {
				new float[] {_tint.R / 255.0f, 0, 0, 0, 0 },
				new float[] {0, _tint.G / 255.0f, 0, 0, 0 },
				new float[] {0, 0, _tint.B / 255.0f, 0, 0 },
				new float[] {0, 0, 0, _tint.A / 255.0f, 0 },
				new float[] {0, 0, 0, 0, 1 }
			});
			imageAttrs.SetColorMatrix(m);

			graphics.DrawImage(_bitmap, new Rectangle(x, y, Width, Height), _sourceRectangle.X, _sourceRectangle.Y, _sourceRectangle.Width, _sourceRectangle.Height, GraphicsUnit.Pixel, imageAttrs);
			//graphics.DrawImage(_bitmap, x, y, _sourceRectangle, GraphicsUnit.Pixel);
		}
	}
}