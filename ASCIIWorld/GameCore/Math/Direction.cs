using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameCore.Math
{
	public class Direction : IEquatable<Direction>
	{
		#region Fields

		private static Random _random;
		private static Direction[] _values;

		private Func<Direction> _opposite;

		#endregion

		#region Constructors

		static Direction()
		{
			_random = new Random();

			North = new Direction(() => South, -Vector2.UnitY);
			South = new Direction(() => North, Vector2.UnitY);
			East = new Direction(() => West, Vector2.UnitX);
			West = new Direction(() => East, -Vector2.UnitX);

			_values = new[]
			{
				North,
				South,
				East,
				West
			};
		}

		private Direction(Func<Direction> opposite, Vector2 vector)
		{
			_opposite = opposite;
			Vector = vector;
		}

		#endregion

		#region Properties

		public static Direction North { get; private set; }

		public static Direction South { get; private set; }

		public static Direction East { get; private set; }

		public static Direction West { get; private set; }

		public static IEnumerable<Direction> Values
		{
			get
			{
				foreach (var value in _values)
				{
					yield return value;
				}
				yield break;
			}
		}

		public static Direction Random
		{
			get
			{
				switch (_random.Next(4))
				{
					case 0:
						return North;
					case 1:
						return South;
					case 2:
						return East;
					case 3:
					default:
						return West;
				}
			}
		}

		public int Value
		{
			get
			{
				for (var index = 0; index < _values.Length; index++)
				{
					if (_values[index] == this)
					{
						return index;
					}
				}
				return -1;
			}
		}

		public Direction Opposite
		{
			get
			{
				return _opposite();
			}
		}

		public Vector2 Vector { get; private set; }

		public Point Point
		{
			get
			{
				return new Point((int)Vector.X, (int)Vector.Y);
			}
		}

		public Size Size
		{
			get
			{
				return new Size((int)Vector.X, (int)Vector.Y);
			}
		}

		#endregion

		#region Methods

		public bool Equals(Direction other)
		{
			return (other != null) && Vector.Equals(other.Vector);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Direction);
		}

		public override int GetHashCode()
		{
			return Vector.GetHashCode();
		}

		public static Direction FromValue(int value)
		{
			return Values.ElementAt(value);
		}

		#endregion
	}
}
