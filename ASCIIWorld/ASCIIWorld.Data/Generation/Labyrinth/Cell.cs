using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class Cell
	{
		#region Fields

		private Dictionary<Direction, SideType> _sides;

		#endregion

		#region Constructors

		public Cell()
		{
			Visited = false;

			_sides = new Dictionary<Direction, SideType>()
			{
				{ Direction.North, SideType.Wall },
				{ Direction.South, SideType.Wall },
				{ Direction.East, SideType.Wall },
				{ Direction.West, SideType.Wall }
			};
		}

		#endregion

		#region Properties

		public bool Visited { get; set; }

		public SideType NorthSide
		{
			get
			{
				return _sides[Direction.North];
			}
			set
			{
				_sides[Direction.North] = value;
			}
		}

		public SideType SouthSide
		{
			get
			{
				return _sides[Direction.South];
			}
			set
			{
				_sides[Direction.South] = value;
			}
		}

		public SideType EastSide
		{
			get
			{
				return _sides[Direction.East];
			}
			set
			{
				_sides[Direction.East] = value;
			}
		}

		public SideType WestSide
		{
			get
			{
				return _sides[Direction.West];
			}
			set
			{
				_sides[Direction.West] = value;
			}
		}

		public bool IsDeadEnd
		{
			get
			{
				return WallCount == 3;
			}
		}

		public bool IsCorridor
		{
			get
			{
				return WallCount < 4;
			}
		}

		public int WallCount
		{
			get
			{
				return _sides.Count(kv => kv.Value == SideType.Wall);
			}
		}

		#endregion

		#region Methods

		public Direction CalculateDeadEndCorridorDirection()
		{
			if (!IsDeadEnd)
			{
				throw new InvalidOperationException("This should not have happened.");
			}

			return _sides.First(kv => kv.Value == SideType.Empty).Key;
		}

		#endregion
	}
}
