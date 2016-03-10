using CommonCore.Math;
using System;
using System.Linq;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class RoomGenerator
	{
		#region Fields

		private Random _random;

		#endregion

		#region Constructors

		public RoomGenerator(Random random)
		{
			_random = random;
		}

		#endregion

		#region Properties

		public int NumRooms { get; set; }

		public int MinRoomRows { get; set; }

		public int MaxRoomRows { get; set; }

		public int MinRoomColumns { get; set; }

		public int MaxRoomColumns { get; set; }

		#endregion

		#region Methods

		public void PlaceRooms(LabyrinthDungeon dungeon)
		{
			if ((NumRooms <= 0) || (MinRoomRows <= 0) || (MaxRoomRows <= 0) || (MinRoomColumns <= 0) || (MaxRoomColumns <= 0))
			{
				throw new InvalidOperationException("Invalid object state; all properties must have positive values.");
			}

			// Loop for the number of rooms to place:
			for (var roomCounter = 0; roomCounter < NumRooms; roomCounter++)
			{
				var room = CreateRoom();
				var bestRoomPlacementScore = int.MaxValue;
				Vector2I? bestRoomPlacementLocation = null;

				foreach (var currentRoomPlacementLocation in dungeon.CorridorCellLocations)
				{
					var currentRoomPlacementScore = CalculateRoomPlacementScore(currentRoomPlacementLocation, dungeon, room);

					if (currentRoomPlacementScore < bestRoomPlacementScore)
					{
						bestRoomPlacementScore = currentRoomPlacementScore;
						bestRoomPlacementLocation = currentRoomPlacementLocation;
					}
				}

				// Create room at best room placement cell.
				if (bestRoomPlacementLocation.HasValue)
				{
					PlaceRoom(bestRoomPlacementLocation.Value, dungeon, room);
				}
			}

			PlaceDoors(dungeon);
		}

		private void PlaceDoors(LabyrinthDungeon dungeon)
		{
			foreach (var room in dungeon.Rooms)
			{
				var hasNorthDoor = false;
				var hasSouthDoor = false;
				var hasWestDoor = false;
				var hasEastDoor = false;

				// TODO: Convert this into 4 loops on the sides of the room.
				for (var row = 0; row < room.Rows; row++)
				{
					for (var column = 0; column < room.Columns; column++)
					{
						var cellLocation = new Vector2I(column, row);

						// Translate the room cell location to its location in the dungeon:
						var dungeonLocation = new Vector2I(room.Bounds.X, room.Bounds.Y) + cellLocation;

						// Check if we are on the west boundary of our roomand if there is a corridor to the west:
						if (!hasWestDoor && (cellLocation.X == 0) &&
							dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.West))
						{
							dungeon.CreateDoor(dungeonLocation, Direction.West);
							hasWestDoor = true;
						}

						// Check if we are on the east boundary of our room and if there is a corridor to the east
						if (!hasEastDoor && (cellLocation.X == room.Columns - 1) &&
							dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.East))
						{
							dungeon.CreateDoor(dungeonLocation, Direction.East);
							hasEastDoor = true;
						}

						// Check if we are on the north boundary of our room and if there is a corridor to the north
						if (!hasNorthDoor && (cellLocation.Y == 0) &&
							dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.North))
						{
							dungeon.CreateDoor(dungeonLocation, Direction.North);
							hasNorthDoor = true;
						}

						// Check if we are on the south boundary of our room and if there is a corridor to the south
						if (!hasSouthDoor && (cellLocation.Y == room.Rows - 1) &&
							dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.South))
						{
							dungeon.CreateDoor(dungeonLocation, Direction.South);
							hasSouthDoor = true;
						}
					}
				}
			}
		}

		private void PlaceRoom(Vector2I location, LabyrinthDungeon dungeon, Room room)
		{
			// Offset the room origin to the new location.
			room.SetLocation(location);

			// Loop for each cell in the room
			for (var row = 0; row < room.Rows; row++)
			{
				for (var column = 0; column < room.Columns; column++)
				{
					// Translate the room cell location to its location in the dungeon.
					var dungeonLocation = location + new Vector2I(column, row);
					dungeon[dungeonLocation].NorthSide = room[row, column].NorthSide;
					dungeon[dungeonLocation].SouthSide = room[row, column].SouthSide;
					dungeon[dungeonLocation].WestSide = room[row, column].WestSide;
					dungeon[dungeonLocation].EastSide = room[row, column].EastSide;

					// TODO: This part may be unnecessary.
					// Create room walls on map (either side of the wall)
					if ((column == 0) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, Direction.West)))
					{
						dungeon.CreateWall(dungeonLocation, Direction.West);
					}
					if ((column == room.Columns - 1) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, Direction.East)))
					{
						dungeon.CreateWall(dungeonLocation, Direction.East);
					}
					if ((row == 0) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, Direction.North)))
					{
						dungeon.CreateWall(dungeonLocation, Direction.North);
					}
					if ((row == room.Rows - 1) && (dungeon.HasAdjacentCellInDirection(dungeonLocation, Direction.South)))
					{
						dungeon.CreateWall(dungeonLocation, Direction.South);
					}
				}
			}

			dungeon.Rooms.Add(room);
		}

		private Room CreateRoom()
		{
			var room = new Room(_random.Next(MinRoomRows, MaxRoomRows + 1), _random.Next(MinRoomColumns, MaxRoomColumns + 1));
			room.InitializeRoomCells();
			return room;
		}

		private bool Contains(RectI bounds, Vector2I roomLocation, Room room)
		{
			return
				bounds.Contains(roomLocation.X, roomLocation.Y) &&
				bounds.Contains(roomLocation.X + room.Columns - 1, roomLocation.Y) &&
				bounds.Contains(roomLocation.X + room.Columns - 1, roomLocation.Y + room.Rows - 1) &&
				bounds.Contains(roomLocation.X, roomLocation.Y + room.Rows - 1);
		}

		private int CalculateRoomPlacementScore(Vector2I location, LabyrinthDungeon dungeon, Room room)
		{
			// Check if the room at the given location will fit inside the bounds of the map.
			if (Contains(dungeon.Bounds, location, room))
			{
				var roomPlacementScore = 0;

				// Loop for each cell in the room.
				for (var column = 0; column < room.Columns; column++)
				{
					for (var row = 0; row < room.Rows; row++)
					{
						// Translate the room cell location to its location in the dungeon.
						var dungeonLocation = location + new Vector2I(column, row);

						// Add 1 point for each adjacent corridor to the cell.
						if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.North))
						{
							roomPlacementScore++;
						}
						if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.South))
						{
							roomPlacementScore++;
						}
						if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.West))
						{
							roomPlacementScore++;
						}
						if (dungeon.AdjacentCellInDirectionIsCorridor(dungeonLocation, Direction.East))
						{
							roomPlacementScore++;
						}

						// Add 3 points if the cell overlaps an existing corridor.
						if (dungeon[dungeonLocation].IsCorridor)
						{
							roomPlacementScore += 3;
						}

						// Add 100 points if the cell overlaps any existing room cells.
						foreach (var dungeonRoom in dungeon.Rooms)
						{
							if (dungeonRoom.Bounds.Contains(dungeonLocation))
							{
								roomPlacementScore += 100;
							}
						}
					}
				}

				return roomPlacementScore;
			}
			else
			{
				return int.MaxValue;
			}
		}

		#endregion
	}
}