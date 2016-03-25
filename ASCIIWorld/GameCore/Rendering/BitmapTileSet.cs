using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GameCore.Rendering
{
	/// <summary>
	/// This is used to help create texture atlases.
	/// </summary>
	public class BitmapTileSet
	{
		#region Fields

		private Bitmap _bitmap;
		private int _rows;
		private int _columns;
		private Rectangle[] _tiles;

		#endregion

		#region Constructors

		public BitmapTileSet(Bitmap bitmap, int rows, int columns)
		{
			Initialize(bitmap, rows, columns);
		}

		#endregion

		#region Properties

		public int Count
		{
			get
			{
				return _rows * _columns;
			}
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

		#endregion

		#region Methods

		public void Render(Graphics graphics, int tileIndex, int x, int y, Color tint)
		{
			var imageAttrs = new ImageAttributes();
			var m = new ColorMatrix(new float[][] {
				new float[] {tint.R / 255.0f, 0, 0, 0, 0 },
				new float[] {0, tint.G / 255.0f, 0, 0, 0 },
				new float[] {0, 0, tint.B / 255.0f, 0, 0 },
				new float[] {0, 0, 0, tint.A / 255.0f, 0 },
				new float[] {0, 0, 0, 0, 1 }
			});
			imageAttrs.SetColorMatrix(m);

			graphics.DrawImage(_bitmap, new Rectangle(x, y, Width, Height), _tiles[tileIndex].X, _tiles[tileIndex].Y, _tiles[tileIndex].Width, _tiles[tileIndex].Height, GraphicsUnit.Pixel, imageAttrs);
			//graphics.DrawImage(_bitmap, x, y, _tiles[tileIndex], GraphicsUnit.Pixel);
		}

		protected void Initialize(Bitmap bitmap, int rows, int columns)
		{
			if (rows <= 0)
			{
				throw new ArgumentException("Value must be greater than 0.", "rows");
			}
			if (columns <= 0)
			{
				throw new ArgumentException("Value must be greater than 0.", "columns");
			}

			_bitmap = bitmap;
			_rows = rows;
			_columns = columns;

			Width = _bitmap.Width / columns;
			Height = _bitmap.Height / rows;

			_tiles = new Rectangle[Count];
			for (var n = 0; n < Count; n++)
			{
				int x = (n % _columns) * Width;
				int y = (n / _columns) * Height;

				_tiles[n] = new Rectangle(x, y, Width, Height);
			}
		}

		#endregion
	}
}