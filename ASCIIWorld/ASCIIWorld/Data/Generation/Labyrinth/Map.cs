using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class Map
	{
		#region Fields

		private Cell[,] _cells;

		#endregion

		#region Constructors

		protected Map(int rows, int columns)
		{
			_cells = new Cell[rows, columns];
			Bounds = new RectI(0, 0, columns, rows);
			Rows = rows;
			Columns = columns;
		}

		#endregion

		#region Properties

		public RectI Bounds { get; set; }

		public int Rows { get; private set; }

		public int Columns { get; private set; }

		public Cell this[int row, int column]
		{
			get
			{
				return _cells[row, column];
			}
			set
			{
				_cells[row, column] = value;
			}
		}

		public Cell this[Vector2I location]
		{
			get
			{
				return this[location.Row, location.Column];
			}
			set
			{
				this[location.Row, location.Column] = value;
			}
		}

		#endregion

		#region Methods

		public bool HasAdjacentCellInDirection(Vector2I location, Direction direction)
		{
			// Check that the location falls within the bounds of the map:
			if (!Bounds.Contains(location))
			{
				return false;
			}

			// Check if there is an adjacent cell in the direction:
			if (direction.Equals(Direction.North))
			{
				return location.Y > 0;
			}
			else if (direction.Equals(Direction.South))
			{
				return location.Y < (Rows - 1);
			}
			else if (direction.Equals(Direction.West))
			{
				return location.X > 0;
			}
			else if (direction.Equals(Direction.East))
			{
				return location.X < (Columns - 1);
			}
			else
			{
				return false;
			}
		}

		protected Vector2I GetTargetLocation(Vector2I location, Direction direction)
		{
			if (!HasAdjacentCellInDirection(location, direction))
			{
				throw new InvalidOperationException("No adjacent cell exists for the location and direction provided.");
			}

			if (direction.Equals(Direction.North))
			{
				return location - Vector2I.UnitY;
			}
			else if (direction.Equals(Direction.South))
			{
				return location + Vector2I.UnitY;
			}
			else if (direction.Equals(Direction.West))
			{
				return location - Vector2I.UnitX;
			}
			else if (direction.Equals(Direction.East))
			{
				return location + Vector2I.UnitX;
			}
			else
			{
				throw new InvalidOperationException("No adjacent cell exists for the location and direction provided.");
			}
		}

		#endregion
	}
}
