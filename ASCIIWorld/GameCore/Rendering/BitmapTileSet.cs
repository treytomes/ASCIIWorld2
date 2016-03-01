using System;
using System.Drawing;

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
		private RectangleF[] _tiles;

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

		public void Render(Graphics graphics, int tileIndex, int x, int y)
		{
			graphics.DrawImage(_bitmap, x, y, _tiles[tileIndex], GraphicsUnit.Pixel);
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

			_tiles = new RectangleF[Count];
			for (var n = 0; n < Count; n++)
			{
				float x = (n % _columns) * Width;
				float y = (n / _columns) * Height;

				_tiles[n] = new RectangleF(x, y, Width, Height);
			}
		}

		#endregion
	}
}