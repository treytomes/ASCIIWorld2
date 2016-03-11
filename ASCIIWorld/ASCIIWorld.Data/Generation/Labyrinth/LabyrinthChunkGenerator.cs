using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class LabyrinthChunkGenerator : BaseChunkGenerator
	{
		#region Fields

		private int _height;
		private int _width;

		private string _seed;
		private Random _random;

		private int _doorId;
		private int _floorId;
		private int _wallId;

		#endregion

		#region Constructors

		public LabyrinthChunkGenerator(Dictionary<int, string> blocks, int width, int height, string seed)
		{
			_seed = seed ?? DateTime.Now.GetHashCode().ToString();
			_random = new Random(_seed.GetHashCode());

			_height = height;
			_width = width;

			_doorId = blocks.Single(x => x.Value == "WoodenDoor").Key;
			_floorId = blocks.Single(x => x.Value == "Stone").Key;
			_wallId = blocks.Single(x => x.Value == "Stone").Key;
		}

		#endregion

		#region Methods

		public override Chunk Generate(IProgress<string> progress)
		{
			var tileMap = new Chunk(_width, _height);

			progress.Report("Generating dungeon...");
			var generator = CreateDungeonGenerator(tileMap);
			var dungeon = generator.Generate();

			tileMap = ConvertToTileMap(tileMap, dungeon, progress);

			return tileMap;
		}

		private Chunk ConvertToTileMap(Chunk chunk, LabyrinthDungeon dungeon, IProgress<string> progress)
		{
			progress.Report("Converting dungeon into chunk...");

			progress.Report("Generating a rocky chunk...");
			chunk = GenerateRockyChunk(chunk);

			progress.Report("Excavating rooms...");
			chunk = ExcavateRooms(chunk, dungeon, progress);

			progress.Report("Excavating corridors...");
			chunk = ExcavateCorridors(chunk, dungeon);

			return chunk;
		}

		private Chunk GenerateRockyChunk(Chunk chunk)
		{
			// Initialize the tile array to rock.
			for (var row = 0; row < chunk.Height; row++)
			{
				for (var column = 0; column < chunk.Width; column++)
				{
					chunk[ChunkLayer.Floor, column, row] = _floorId;
					chunk[ChunkLayer.Blocking, column, row] = _wallId;
				}
			}

			return chunk;
		}

		private Chunk ExcavateRooms(Chunk chunk, LabyrinthDungeon dungeon, IProgress<string> progress)
		{
			progress.Report("Excavating room...");
			progress.Report($"Room count: {dungeon.Rooms.Count}");
			// Fill tiles with corridor values for each room in dungeon.
			foreach (var room in dungeon.Rooms)
			{
				// Get the room min and max location in tile coordinates.
				var right = room.Bounds.X + room.Bounds.Width - 1;
				var bottom = room.Bounds.Y + room.Bounds.Height - 1;
				var minPoint = new Vector2I(room.Bounds.X * 2 + 1, room.Bounds.Y * 2 + 1);
				var maxPoint = new Vector2I(right * 2, bottom * 2);

				// Fill the room in tile space with an empty value.
				for (var row = minPoint.Y; row <= maxPoint.Y; row++)
				{
					for (var column = minPoint.X; column <= maxPoint.X; column++)
					{
						ExcavateChunkPoint(chunk, new Vector2I(column, row));
					}
				}
			}
			progress.Report("Room complete!");

			return chunk;
		}

		private Chunk ExcavateCorridors(Chunk chunk, LabyrinthDungeon dungeon)
		{
			// Loop for each corridor cell and expand it.
			foreach (var cellLocation in dungeon.CorridorCellLocations)
			{
				var tileLocation = new Vector2I(cellLocation.X * 2 + 1, cellLocation.Y * 2 + 1);
				ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y));

				if (dungeon[cellLocation].NorthSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y - 1));
				}
				else if (dungeon[cellLocation].NorthSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y - 1));
					chunk[ChunkLayer.Blocking, tileLocation.X, tileLocation.Y - 1] = _doorId;
				}

				if (dungeon[cellLocation].SouthSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y + 1));
				}
				else if (dungeon[cellLocation].SouthSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X, tileLocation.Y + 1));
					chunk[ChunkLayer.Blocking, tileLocation.X, tileLocation.Y + 1] = _doorId;
				}

				if (dungeon[cellLocation].WestSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X - 1, tileLocation.Y));
				}
				else if (dungeon[cellLocation].WestSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X - 1, tileLocation.Y));
					chunk[ChunkLayer.Blocking, tileLocation.X - 1, tileLocation.Y] = _doorId;
				}

				if (dungeon[cellLocation].EastSide == SideType.Empty)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X + 1, tileLocation.Y));
				}
				else if (dungeon[cellLocation].EastSide == SideType.Door)
				{
					ExcavateChunkPoint(chunk, new Vector2I(tileLocation.X + 1, tileLocation.Y));
					chunk[ChunkLayer.Blocking, tileLocation.X + 1, tileLocation.Y] = _doorId;
				}
			}

			return chunk;
		}

		private void ExcavateChunkPoint(Chunk chunk, Vector2I chunkPoint)
		{
			chunk[ChunkLayer.Blocking, chunkPoint.X, chunkPoint.Y] = 0;
		}

		private bool IsDoorAdjacent(Chunk chunk, Vector2I chunkPoint)
		{
			var north = chunk[ChunkLayer.Blocking, chunkPoint.X, chunkPoint.Y - 1];
			var south = chunk[ChunkLayer.Blocking, chunkPoint.X, chunkPoint.Y + 1];
			var east = chunk[ChunkLayer.Blocking, chunkPoint.X + 1, chunkPoint.Y];
			var west = chunk[ChunkLayer.Blocking, chunkPoint.X - 1, chunkPoint.Y];

			return (north == _doorId) || (south == _doorId) || (east == _doorId) || (west == _doorId);
		}

		private LabyrinthGenerator CreateDungeonGenerator(Chunk chunk)
		{
			return new LabyrinthGenerator(_random, CreateRoomGenerator())
			{
				Rows = (chunk.Height - 1) / 2,
				Columns = (chunk.Width - 1) / 2,
				ChangeDirectionModifier = _random.NextDouble(),
				SparsenessFactor = _random.NextDouble(),
				DeadEndRemovalModifier = _random.NextDouble()
			};
		}

		private RoomGenerator CreateRoomGenerator()
		{
			return new RoomGenerator(_random)
			{
				NumRooms = 5,
				MinRoomRows = 2,
				MaxRoomRows = 5,
				MinRoomColumns = 2,
				MaxRoomColumns = 5
			};
		}

		#endregion
	}
}
