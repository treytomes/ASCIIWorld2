using OpenTK;
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

			IsNormalized = true;
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

		/// <summary>
		/// Should the tiles be drawn with a width and height of 1?  Or of their pixel height?
		/// </summary>
		/// <remarks>
		/// Defaults to True.
		/// </remarks>
		public bool IsNormalized { get; set; }

		#endregion

		#region Methods

		public void Render(ITessellator tessellator, int tileIndex, bool mirrorX = false, bool mirrorY = false)
		{
			var tile = _tiles[tileIndex];

			var minU = mirrorX ? tile.MaxU : tile.MinU;
			var maxU = mirrorX ? tile.MinU : tile.MaxU;
			var minV = mirrorY ? tile.MaxV : tile.MinV;
			var maxV = mirrorY ? tile.MinV : tile.MaxV;

			var width = IsNormalized ? 1 : tile.Width;
			var height = IsNormalized ? 1 : tile.Height;

			tessellator.BindTexture(_texture);
			tessellator.AddPoint(0, 0, minU, minV);
			tessellator.AddPoint(0, height, minU, maxV);
			tessellator.AddPoint(width, height, maxU, maxV);
			tessellator.AddPoint(width, 0, maxU, minV);
		}

		public void RenderText(ITessellator tessellator, string format, params object[] args)
		{
			var unitX = tessellator.Transform(Vector2.UnitX) - tessellator.Transform(Vector2.Zero);
			//tessellator.Translate(x, y);

			format = string.Format(format, args);
			for (var index = 0; index < format.Length; index++)
			{
				Render(tessellator, (int)format[index]);
				tessellator.Translate(unitX);
			}

			tessellator.Translate(-unitX * format.Length);
			//tessellator.Translate(-x, -y);
		}

		#endregion
	}
}