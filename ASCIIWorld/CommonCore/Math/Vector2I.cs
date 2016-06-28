using System;

namespace CommonCore.Math
{
	/// <summary>
	/// Represent an integer-based 2-dimentional vector.
	/// </summary>
	[Serializable]
	public struct Vector2I
	{
		#region Fields

		public readonly int X;

		public readonly int Y;

		#endregion

		#region Constructors

		public Vector2I(int x, int y)
		{
			X = x;
			Y = y;
		}

		#endregion

		#region Properties

		public static Vector2I Zero
		{
			get
			{
				return new Vector2I(0, 0);
			}
		}

		public static Vector2I UnitX
		{
			get
			{
				return new Vector2I(1, 0);
			}
		}

		public static Vector2I UnitY
		{
			get
			{
				return new Vector2I(0, 1);
			}
		}

		public static Vector2I One
		{
			get
			{
				return new Vector2I(1, 1);
			}
		}

		public int Row
		{
			get
			{
				return Y;
			}
		}

		public int Column
		{
			get
			{
				return X;
			}
		}

		public double Length
		{
			get
			{
				return System.Math.Sqrt(X * X + Y * Y);
			}
		}

		/// <summary>
		/// Returns a normalized clone of this vector instance.
		/// </summary>
		public Vector2I Normalized
		{
			get
			{
				return this / Length;
			}
		}

		#endregion

		#region Methods

		public static bool operator ==(Vector2I left, Vector2I right)
		{
			return (left.X == right.X) && (left.Y == right.Y);
		}

		public static bool operator !=(Vector2I left, Vector2I right)
		{
			return (left.X != right.X) || (left.Y != right.Y);
		}

		public static Vector2I operator +(Vector2I left, Vector2I right)
		{
			return new Vector2I(left.X + right.X, left.Y + right.Y);
		}

		public static Vector2I operator -(Vector2I left, Vector2I right)
		{
			return new Vector2I(left.X - right.X, left.Y - right.Y);
		}

		public static Vector2I operator -(Vector2I v)
		{
			return new Vector2I(-v.X, -v.Y);
		}

		public static Vector2I operator /(Vector2I v, double n)
		{
			return new Vector2I((int)(v.X / n), (int)(v.Y / n));
		}

		public override bool Equals(object obj)
		{
			return (Vector2I)obj == this;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

		#endregion
	}
}
