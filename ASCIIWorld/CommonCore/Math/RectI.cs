namespace CommonCore.Math
{
	public class RectI
	{
		#region Fields

		private int _x;
		private int _y;
		private int _width;
		private int _height;

		#endregion

		#region Constructors

		public RectI(int x, int y, int width, int height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

		public RectI(RectI clone)
		{
			_x = clone.X;
			_y = clone.Y;
			_width = clone.Width;
			_height = clone.Height;
		}

		#endregion

		#region Properties

		public int X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public int Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		public int Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public int Left
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public int Right
		{
			get
			{
				return _x + _width - 1;
			}
			set
			{
				_width = value - _x + 1;
			}
		}

		public int Top
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		public int Bottom
		{
			get
			{
				return _y + _height - 1;
			}
			set
			{
				_height = value - _y + 1;
			}
		}

		public Vector2I TopLeft
		{
			get
			{
				return new Vector2I(Left, Top);
			}
		}

		public Vector2I TopRight
		{
			get
			{
				return new Vector2I(Right, Top);
			}
		}

		public Vector2I BottomLeft
		{
			get
			{
				return new Vector2I(Left, Bottom);
			}
		}

		public Vector2I BottomRight
		{
			get
			{
				return new Vector2I(Right, Bottom);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create a new rectangle, inflated by <paramref name="amount"/>.
		/// </summary>
		public RectI Inflate(int amount)
		{
			return Inflate(amount, amount);
		}

		/// <summary>
		/// Create a new rectangle, inflated by <paramref name="x"/> and <paramref name="y"/>.
		/// </summary>
		public RectI Inflate(int x, int y)
		{
			var newRect = new RectI(this);
			newRect.Left -= x;
			newRect.Right += x;
			newRect.Top -= y;
			newRect.Bottom += y;
			return newRect;
		}

		public bool Contains(Vector2I point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(int x, int y)
		{
			return MathHelper.IsInRange(x, Left, Right + 1) && MathHelper.IsInRange(y, Top, Bottom + 1);
		}

		public bool Contains(RectI rect)
		{
			return
				Contains(rect.TopLeft) &&
				Contains(rect.TopRight) &&
				Contains(rect.BottomLeft) &&
				Contains(rect.BottomRight);
		}

		#endregion
	}
}
