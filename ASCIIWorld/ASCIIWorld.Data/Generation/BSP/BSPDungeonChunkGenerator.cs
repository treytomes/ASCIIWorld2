using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation.BSP
{
	public class BSPDungeonChunkGenerator : BaseChunkGenerator
	{
		#region Fields

		private RoomGenerator _roomGenerator;

		private string _seed;
		private Random _random;

		private int _chestId;
		private int _doorId;
		private int _floorId;
		private int _wallId;

		private int _height;
		private int _width;

		#endregion

		#region Constructors

		public BSPDungeonChunkGenerator(Dictionary<int, string> blocks, int width, int height, string seed, RoomGenerator roomGenerator)
		{
			_seed = seed ?? DateTime.Now.GetHashCode().ToString();
			_random = new Random(_seed.GetHashCode());

			_width = width;
			_height = height;

			_chestId = blocks.Single(x => x.Value == "Chest").Key;
			_doorId = blocks.Single(x => x.Value == "WoodenDoor").Key;
			_floorId = blocks.Single(x => x.Value == "Stone").Key;
			_wallId = blocks.Single(x => x.Value == "Stone").Key;

			_roomGenerator = roomGenerator;
		}

		public BSPDungeonChunkGenerator(Dictionary<int, string> blocks, int width, int height, string seed)
			: this(blocks, width, height, seed, new RoomGenerator(5, 6, 12, 6, 12))
		{
		}

		#endregion

		#region Methods

		public override Chunk Generate(IProgress<string> progress)
		{
			var chunk = new Chunk(_width, _height);
			Fill(chunk, ChunkLayer.Floor, _floorId);
			Fill(chunk, ChunkLayer.Blocking, _wallId);

			_roomGenerator.Reset();

			var dungeonArea = new HorizontalArea(_roomGenerator, _random, 0, 0, _width, _height);
			RenderRooms(chunk, dungeonArea, progress);
			RenderCorridors(chunk, dungeonArea, progress);

			DecorateWithDoors(chunk, dungeonArea, progress);

			return chunk;
		}

		private void RenderRooms(Chunk chunk, Area area, IProgress<string> progress)
		{
			if (area.Room != null)
			{
				area.Room.Render(this, chunk, _floorId, _wallId);
			}

			if (area.SubArea1 != null)
			{
				RenderRooms(chunk, area.SubArea1, progress);
			}
			if (area.SubArea2 != null)
			{
				RenderRooms(chunk, area.SubArea2, progress);
			}
		}

		private void RenderCorridors(Chunk chunk, Area area, IProgress<string> progress)
		{
			if (area.SubArea1 != null)
			{
				RenderCorridors(chunk, area.SubArea1, progress);
			}
			if (area.SubArea2 != null)
			{
				RenderCorridors(chunk, area.SubArea2, progress);
			}

			if ((area.SubArea1 != null) && (area.SubArea2 != null))
			{
				var room1 = area.SubArea1.GetConnectableRoom();
				var room2 = area.SubArea2.GetConnectableRoom();
				ConnectRooms(chunk, room1, room2, progress);
			}
		}

		private void ConnectRooms(Chunk chunk, Room room1, Room room2, IProgress<string> progress)
		{
			if ((room1 != null) && (room2 != null))
			{
				var start = room1.RandomPoint;
				var end = room2.RandomPoint;

				var current = start;
				while (current != end)
				{
					chunk[ChunkLayer.Blocking, current.X, current.Y] = 0;

					if (current.X < end.X)
					{
						current += Vector2I.UnitX;
					}
					else if (current.X > end.X)
					{
						current -= Vector2I.UnitX;
					}
					else if (current.Y < end.Y)
					{
						current += Vector2I.UnitY;
					}
					else if (current.Y > end.Y)
					{
						current -= Vector2I.UnitY;
					}
				}
			}
		}

		private void DecorateWithDoors(Chunk chunk, Area area, IProgress<string> progress)
		{
			// TODO: This is causing rooms to be disconnected in some cases (when rooms are adjacent).

			foreach (var room in CollectRooms(area))
			{
				// Check the room bounds places to put doors, ignoring the corners.
				// This will also make sure that doors aren't placed directly next to eachother.

				for (var row = room.Bounds.Top + 1; row <= room.Bounds.Bottom - 1; row++)
				{
					if (!room.HasWestDoor)
					{
						if (chunk[ChunkLayer.Blocking, room.Bounds.Left - 1, row] == 0)
						{
							if ((chunk[ChunkLayer.Blocking, room.Bounds.Left, row - 1] == _wallId) &&
								(chunk[ChunkLayer.Blocking, room.Bounds.Left, row + 1] == _wallId))
							{
								chunk[ChunkLayer.Blocking, room.Bounds.Left, row] = _doorId;
								room.HasWestDoor = true;
							}
						}
					}
					if (!room.HasEastDoor)
					{
						if (chunk[ChunkLayer.Blocking, room.Bounds.Right + 1, row] == 0)
						{
							if ((chunk[ChunkLayer.Blocking, room.Bounds.Right, row - 1] == _wallId) &&
								(chunk[ChunkLayer.Blocking, room.Bounds.Right, row + 1] == _wallId))
							{
								chunk[ChunkLayer.Blocking, room.Bounds.Right, row] = _doorId;
								room.HasEastDoor = true;
							}
						}
					}
				}

				for (var column = room.Bounds.Left + 1; column <= room.Bounds.Right - 1; column++)
				{
					if (!room.HasNorthDoor)
					{
						if (chunk[ChunkLayer.Blocking, column, room.Bounds.Top - 1] == 0)
						{
							if ((chunk[ChunkLayer.Blocking, column - 1, room.Bounds.Top] == _wallId) &&
								(chunk[ChunkLayer.Blocking, column + 1, room.Bounds.Top] == _wallId))
							{
								chunk[ChunkLayer.Blocking, column, room.Bounds.Top] = _doorId;
								room.HasNorthDoor = true;
							}
						}
					}
					if (!room.HasSouthDoor)
					{
						if (chunk[ChunkLayer.Blocking, column, room.Bounds.Bottom + 1] == 0)
						{
							if ((chunk[ChunkLayer.Blocking, column - 1, room.Bounds.Bottom] == _wallId) &&
								(chunk[ChunkLayer.Blocking, column + 1, room.Bounds.Bottom] == _wallId))
							{
								chunk[ChunkLayer.Blocking, column, room.Bounds.Bottom] = _doorId;
								room.HasSouthDoor = true;
							}
						}
					}
				}
			}
		}

		private IEnumerable<Room> CollectRooms(Area area)
		{
			var rooms = new List<Room>();

			if (area.Room != null)
			{
				yield return area.Room;
			}

			if (area.SubArea1 != null)
			{
				foreach (var room in CollectRooms(area.SubArea1))
				{
					yield return room;
				}
			}
			if (area.SubArea2 != null)
			{
				foreach (var room in CollectRooms(area.SubArea2))
				{
					yield return room;
				}
			}

			yield break;
		}

		#endregion
	}
}
