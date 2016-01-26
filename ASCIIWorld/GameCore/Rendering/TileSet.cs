using System;

namespace GameCore.Rendering
{
	public class TileSet
	{
		private struct Tile
		{
			public int Width;
			public int Height;

			public float MinU;
			public float MaxU;
			public float MinV;
			public float MaxV;
		}

		private Texture2D _texture;
		private int _rows;
		private int _columns;
		private Tile[] _tiles;

		public TileSet(Texture2D texture, int rows, int columns)
		{
			if (rows <= 0)
			{
				throw new ArgumentException("Value must be greater than 0.", "rows");
			}
			if (columns <= 0)
			{
				throw new ArgumentException("Value must be greater than 0.", "columns");
			}

			_texture = texture;
			_rows = rows;
			_columns = columns;

			Width = _texture.Width / columns;
			Height = _texture.Height / rows;

			_tiles = new Tile[Count];
			for (var n = 0; n < Count; n++)
			{
				float x = (n % _columns) * Width;
				float y = (n / _columns) * Height;

				float texLeft = x / _texture.Width;
				float texTop = y / _texture.Height;
				float texRight = (x + Width) / _texture.Width;
				float texBottom = (y + Height) / _texture.Height;

				_tiles[n] = new Tile()
				{
					Width = Width,
					Height = Height,
					MinU = texLeft,
					MaxU = texRight,
					MinV = texTop,
					MaxV = texBottom
				};
			}
		}

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

		public void Render(ITessellator tessellator, int tileIndex, bool mirrorX = false, bool mirrorY = false)
		{
			var tile = _tiles[tileIndex];

			var minU = mirrorX ? tile.MaxU : tile.MinU;
			var maxU = mirrorX ? tile.MinU : tile.MaxU;
			var minV = mirrorY ? tile.MaxV : tile.MinV;
			var maxV = mirrorY ? tile.MinV : tile.MaxV;

			tessellator.BindTexture(_texture);
			tessellator.AddPoint(0, 0, minU, minV);
			tessellator.AddPoint(0, tile.Height, minU, maxV);
			tessellator.AddPoint(tile.Width, tile.Height, maxU, maxV);
			tessellator.AddPoint(tile.Width, 0, maxU, minV);
		}

		#endregion
	}
}