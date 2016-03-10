using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class LabyrinthDungeon : Map
	{
		#region Fields

		private List<Vector2I> _visitedCells;

		#endregion

		#region Constructors

		public LabyrinthDungeon(int rows, int columns)
			: base(rows, columns)
		{
			_visitedCells = new List<Vector2I>();
			Rooms = new List<Room>();

			MarkCellsUnvisited();
		}

		#endregion

		#region Properties

		public bool AllCellsVisited
		{
			get
			{
				return _visitedCells.Count == (Rows * Columns);
			}
		}

		public List<Vector2I> DeadEndCellLocations
		{
			get
			{
				var points = new List<Vector2I>();
				for (var row = 0; row < Rows; row++)
				{
					for (var column = 0; column < Columns; column++)
					{
						if (this[row, column].IsDeadEnd)
						{
							points.Add(new Vector2I(column, row));
						}
					}
				}
				return points;
			}
		}

		public IEnumerable<Vector2I> CorridorCellLocations
		{
			get
			{
				for (var row = 0; row < Rows; row++)
				{
					for (var column = 0; column < Columns; column++)
					{
						if (this[row, column].IsCorridor)
						{
							yield return new Vector2I(column, row);
						}
					}
				}
				yield break;
			}
		}

		public List<Room> Rooms { get; private set; }

		#endregion

		#region Methods

		public void MarkCellsUnvisited()
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var column = 0; column < Columns; column++)
				{
					this[row, column] = new Cell();
				}
			}
			_visitedCells.Clear();
		}

		public Vector2I PickRandomCellAndMarkItVisited(Random random)
		{
			var pnt = new Vector2I(random.Next(Columns), random.Next(Rows));
			FlagCellAsVisited(pnt);
			return pnt;
		}

		public bool AdjacentCellInDirectionIsVisited(Vector2I location, Direction direction)
		{
			if (!HasAdjacentCellInDirection(location, direction))
			{
				throw new InvalidOperationException("No adjacent cell exists for the location and direction provided.");
			}
			return this[location + direction.ToVector()].Visited;
		}

		public bool AdjacentCellInDirectionIsCorridor(Vector2I location, Direction direction)
		{
			if (!HasAdjacentCellInDirection(location, direction))
			{
				return false;
			}
			var target = GetTargetLocation(location, direction);
			return this[target].IsCorridor;
		}

		public void FlagCellAsVisited(Vector2I location)
		{
			if (LocationIsOutsideBounds(location))
			{
				throw new Exception("Location is outside of map bounds.");
			}
			if (this[location].Visited)
			{
				throw new Exception("Location is already visited.");
			}
			else
			{
				this[location].Visited = true;
				_visitedCells.Add(location);
			}
		}

		private bool LocationIsOutsideBounds(Vector2I location)
		{
			return ((location.X < 0) || (location.X >= Columns) || (location.Y < 0) || (location.Y >= Rows));
		}

		public Vector2I GetRandomVisitedCell(Vector2I location, Random random)
		{
			if (!_visitedCells.Any())
			{
				throw new InvalidOperationException("There are no visited cells to return.");
			}

			var index = random.Next(_visitedCells.Count);

			// Loop while the current cell is the visited cell:
			while (_visitedCells[index] == location)
			{
				index = random.Next(_visitedCells.Count);
			}

			return _visitedCells[index];
		}

		public Vector2I CreateCorridor(Vector2I location, Direction direction)
		{
			return CreateSide(location, direction, SideType.Empty);
		}

		public Vector2I CreateWall(Vector2I location, Direction direction)
		{
			return CreateSide(location, direction, SideType.Wall);
		}

		public Vector2I CreateDoor(Vector2I location, Direction direction)
		{
			return CreateSide(location, direction, SideType.Door);
		}

		private Vector2I CreateSide(Vector2I location, Direction direction, SideType sideType)
		{
			var target = GetTargetLocation(location, direction);

			if (direction.Equals(Direction.North))
			{
				this[location].NorthSide = sideType;
				this[target].SouthSide = sideType;
			}
			else if (direction.Equals(Direction.South))
			{
				this[location].SouthSide = sideType;
				this[target].NorthSide = sideType;
			}
			else if (direction.Equals(Direction.West))
			{
				this[location].WestSide = sideType;
				this[target].EastSide = sideType;
			}
			else if (direction.Equals(Direction.East))
			{
				this[location].EastSide = sideType;
				this[target].WestSide = sideType;
			}

			return target;
		}

		#endregion
	}
}