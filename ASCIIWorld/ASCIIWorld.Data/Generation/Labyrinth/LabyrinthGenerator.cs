using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class LabyrinthGenerator
	{
		#region Fields

		private Random _random;
		private RoomGenerator _roomGenerator;

		#endregion

		#region Constructors

		public LabyrinthGenerator(Random random, RoomGenerator roomGenerator)
		{
			_random = random;
			_roomGenerator = roomGenerator;
		}

		#endregion

		#region Properties

		public int Rows { get; set; }

		public int Columns { get; set; }

		/// <summary>
		/// Value from 0.0 to 1.0.  Percentage chance of changing direction.
		/// </summary>
		public double ChangeDirectionModifier { get; set; }

		/// <summary>
		/// Percentage of the map (0.0 to 1.0) turned to walls.
		/// </summary>
		public double SparsenessFactor { get; set; }

		/// <summary>
		/// Percentage value (0.0 - 1.0) of dead ends to convert into loops.
		/// </summary>
		public double DeadEndRemovalModifier { get; set; }

		#endregion

		#region Methods

		public LabyrinthDungeon Generate()
		{
			var dungeon = CreateDenseMaze(Rows, Columns, ChangeDirectionModifier);

			SparsifyMaze(dungeon, SparsenessFactor);

			RemoveDeadEnds(dungeon, DeadEndRemovalModifier);

			_roomGenerator.PlaceRooms(dungeon);

			return dungeon;
		}

		private LabyrinthDungeon CreateDenseMaze(int rows, int columns, double changeDirectionModifier)
		{
			var map = new LabyrinthDungeon(rows, columns);
			map.MarkCellsUnvisited();
			var currentLocation = map.PickRandomCellAndMarkItVisited(_random);
			var previousDirection = Direction.North;

			while (!map.AllCellsVisited)
			{
				var directionPicker = new DirectionPicker(_random, previousDirection, changeDirectionModifier);
				var direction = directionPicker.GetNextDirection();

				while (!map.HasAdjacentCellInDirection(currentLocation, direction) || map.AdjacentCellInDirectionIsVisited(currentLocation, direction))
				{
					if (directionPicker.HasNextDirection)
					{
						direction = directionPicker.GetNextDirection();
					}
					else
					{
						currentLocation = map.GetRandomVisitedCell(currentLocation, _random); // get a new previously visited location
						directionPicker = new DirectionPicker(_random, previousDirection, changeDirectionModifier); // reset the direction picker
						direction = directionPicker.GetNextDirection(); // get a new direction.
					}
				}

				currentLocation = map.CreateCorridor(currentLocation, direction);

				map.FlagCellAsVisited(currentLocation);
				previousDirection = direction;
			}

			return map;
		}

		/// <param name="sparsenessFactor">Percentage of the map (0.0 to 1.0) turned to walls.</param>
		private void SparsifyMaze(LabyrinthDungeon map, double sparsenessFactor)
		{
			// Calculate the number of cells to remove as a percentage of the total number of cells in the map:
			var noOfDeadEndCellsToRemove = (int)System.Math.Ceiling(sparsenessFactor * map.Rows * map.Columns);
			var points = map.DeadEndCellLocations;

			for (var i = 0; i < noOfDeadEndCellsToRemove; i++)
			{
				if (points.Count == 0)
				{
					// check if there is another item in our enumerator
					points = map.DeadEndCellLocations; // get a new list
					if (points.Count == 0)
					{
						break; // no new items exist so break out of loop
					}
				}

				var index = _random.Next(0, points.Count);
				var point = points[index];
				points.RemoveAt(index);
				if (map[point].IsDeadEnd)
				{
					// make sure the status of the cell hasn't change
					map.CreateWall(point, map[point].CalculateDeadEndCorridorDirection());
				}
			}
		}

		/// <param name="deadEndRemovalModifier">Percentage value (0.0 - 1.0) of dead ends to convert into loops.</param>
		private void RemoveDeadEnds(LabyrinthDungeon map, double deadEndRemovalModifier)
		{
			var noOfDeadEndCellsToRemove = (int)System.Math.Ceiling(deadEndRemovalModifier * map.Rows * map.Columns);
			var deadEndLocations = map.DeadEndCellLocations;

			for (var i = 0; i < noOfDeadEndCellsToRemove; i++)
			{
				if (deadEndLocations.Count == 0)
				{
					break;
				}

				var index = _random.Next(0, deadEndLocations.Count);
				var deadEndLocation = deadEndLocations[index];
				deadEndLocations.RemoveAt(index);
				if (map[deadEndLocation].IsDeadEnd)
				{
					var currentLocation = deadEndLocation;

					do
					{
						// Initialize the direction picker not to select the dead-end corridor direction.
						var directionPicker = new DirectionPicker(_random, map[currentLocation].CalculateDeadEndCorridorDirection(), 1);
						var direction = directionPicker.GetNextDirection();

						while (!map.HasAdjacentCellInDirection(currentLocation, direction))
						{
							if (directionPicker.HasNextDirection)
							{
								direction = directionPicker.GetNextDirection();
							}
							else
							{
								throw new InvalidOperationException("This should not happen.");
							}
						}

						// Create a corridor in the selected direction:
						currentLocation = map.CreateCorridor(currentLocation, direction);
					}
					while (map[currentLocation].IsDeadEnd); // stop when you intersect an existing corridor
				}
			}
		}

		#endregion
	}
}
