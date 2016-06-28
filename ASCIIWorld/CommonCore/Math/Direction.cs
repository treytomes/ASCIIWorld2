using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonCore.Math
{
	[Serializable]
	public struct Direction
	{
		#region Fields

		private static Random _random;
		private readonly Vector2I _vector;

		#endregion

		#region Constructors

		static Direction()
		{
			_random = new Random();
			North = new Direction(new Vector2I(0, -1));
			South = new Direction(new Vector2I(0, 1));
			East = new Direction(new Vector2I(1, 0));
			West = new Direction(new Vector2I(-1, 0));
		}

		private Direction(Vector2I v)
		{
			_vector = v.Normalized;
		}

		#endregion

		#region Properties

		public static Direction North { get; private set; }

		public static Direction South { get; private set; }

		public static Direction East { get; private set; }

		public static Direction West { get; private set; }

		public static IEnumerable<Direction> All
		{
			get
			{
				yield return North;
				yield return South;
				yield return East;
				yield return West;
				yield break;
			}
		}

		public static Direction Random
		{
			get
			{
				switch (_random.Next(All.Count()))
				{
					default:
					case 0:
						return North;
					case 1:
						return South;
					case 2:
						return East;
					case 3:
						return West;
				}
			}
		}

		public Direction Opposite
		{
			get
			{
				return new Direction(-_vector);
			}
		}

		#endregion

		#region Methods

		public static bool operator ==(Direction left, Direction right)
		{
			return left._vector == right._vector;
		}

		public static bool operator !=(Direction left, Direction right)
		{
			return left._vector != right._vector;
		}

		public static Direction FromVector(Vector2I v)
		{
			v = v.Normalized;
			if (v.Equals(-Vector2I.UnitY))
			{
				return North;
			}
			if (v.Equals(Vector2I.UnitY))
			{
				return South;
			}
			if (v.Equals(Vector2I.UnitX))
			{
				return East;
			}
			if (v.Equals(-Vector2I.UnitX))
			{
				return West;
			}

			return North;
		}

		public Vector2I ToVector2I()
		{
			return _vector;
		}

		public override bool Equals(object obj)
		{
			return (Direction)obj == this;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion
	}
}
