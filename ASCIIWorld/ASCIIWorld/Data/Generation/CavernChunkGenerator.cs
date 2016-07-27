using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation
{
	[Serializable]
	public class CavernChunkGenerator : BaseChunkGenerator
	{
		#region Constants

		private const int SMOOTH_ITERATIONS = 5;
		private const int THRESHOLD_SIZE = 50;

		#endregion

		#region Fields

		private int _randomFillPercent;

		private int _passageCount;
		private int _connectingRoomsCount;

		private int _stoneId;

		#endregion

		#region Constructors

		public CavernChunkGenerator(int width, int height, string seed)
			: base(width, height, seed)
		{
			_stoneId = BlockRegistry.Instance.GetId("Stone");

			_passageCount = 0;
			_connectingRoomsCount = 0;

			_randomFillPercent = 50; // _random.Next(0, 100); // 50 is a good number
		}

		#endregion

		#region Properties

		public override float AmbientLightLevel
		{
			get
			{
				return 8.0f;
			}
		}

		#endregion


		#region Methods

		public override Chunk Generate(IProgress<string> progress, int chunkX, int chunkY)
		{
			Reseed(chunkX, chunkY);

			progress.Report("Generating chunk.");

			var chunk = new Chunk(Width, Height);
			
			GenerateCavern(chunk);

			progress.Report("Removing small regions...");
			RemoveSmallRegions(chunk);

			progress.Report("Connecting isolated regions...");
			ConnectRegions(progress, chunk);

			progress.Report("Done generating chunk.");
			return chunk;
		}

		#region Generate the cavern.

		private void GenerateCavern(Chunk chunk)
		{
			RandomFillMap(chunk);

			for (var iteration = 0; iteration < SMOOTH_ITERATIONS; iteration++)
			{
				SmoothMap(chunk);
			}

			// This is not needed in infinite maps.
			//EnsureMapBorder(chunk);
		}

		private void RandomFillMap(Chunk chunk)
		{
			for (var x = 0; x < chunk.Width; x++)
			{
				for (var y = 0; y < chunk.Height; y++)
				{
					//var value = SimplexNoise.Generate((_chunkX * Width + x) / 256.0f, (_chunkY * Height + y) / 256.0f) * 100;
					//if (value < _randomFillPercent)
					if (Random.Next(0, 100) < _randomFillPercent)
					{
						chunk[ChunkLayer.Blocking, x, y] = _stoneId;
					}
					chunk[ChunkLayer.Floor, x, y] = _stoneId;
				}
			}
		}

		private void SmoothMap(Chunk chunk)
		{
			for (var x = 0; x < chunk.Width; x++)
			{
				for (var y = 0; y < chunk.Height; y++)
				{
					var neighbourWallTiles = GetSurroundingWallCount(chunk, x, y);

					if (neighbourWallTiles > 4)
					{
						chunk[ChunkLayer.Blocking, x, y] = _stoneId;
					}
					else if (neighbourWallTiles < 4)
					{
						chunk[ChunkLayer.Blocking, x, y] = 0;
					}
				}
			}
		}

		private int GetSurroundingWallCount(Chunk chunk, int x, int y)
		{
			var wallCount = 0;
			for (var neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
			{
				for (var neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
				{
					if ((neighbourX != x) || (neighbourY != y))
					{
						if (chunk[ChunkLayer.Blocking, (int)MathHelper.Modulo(neighbourX, chunk.Width), (int)MathHelper.Modulo(neighbourY, chunk.Height)] == _stoneId)
						{
							wallCount++;
						}
					}
				}
			}

			return wallCount;
		}

		private void EnsureMapBorder(Chunk chunk)
		{
			for (var x = 0; x < chunk.Width; x++)
			{
				chunk[ChunkLayer.Blocking, x, 0] = _stoneId;
				chunk[ChunkLayer.Blocking, x, chunk.Height - 1] = _stoneId;
			}
			for (var y = 0; y < chunk.Height; y++)
			{
				chunk[ChunkLayer.Blocking, 0, y] = _stoneId;
				chunk[ChunkLayer.Blocking, chunk.Width - 1, y] = _stoneId;
			}
		}

		#endregion

		#region Remove small regions.

		private void RemoveSmallRegions(Chunk chunk)
		{
			// We do this in two rounds, because the first round will probably remove some regions.
			EnsureMinimumSize(chunk, ChunkLayer.Blocking, GetRegions(chunk, ChunkLayer.Blocking, _stoneId), THRESHOLD_SIZE);

			var floors = GetRegions(chunk, ChunkLayer.Floor, _stoneId);
			EnsureMinimumSize(chunk, ChunkLayer.Floor, GetRegions(chunk, ChunkLayer.Floor, _stoneId), THRESHOLD_SIZE);
			floors = GetRegions(chunk, ChunkLayer.Floor, _stoneId);
		}

		/// <summary>
		/// If a region size is &lt;= thresholdSize, change all of it's tiles to revertTileId.
		/// </summary>
		/// <remarks>
		/// Rooms that are too small will be removed from the regions list.
		/// </remarks>
		/// <param name="thresholdSize">If a region is &lt;= this size, delete it.</param>
		/// <param name="revertPrototype">The tile prototype to use when deleting small regions.</param>
		private void EnsureMinimumSize(Chunk chunk, ChunkLayer layer, List<List<Vector2I>> regions, int thresholdSize)
		{
			for (var index = 0; index < regions.Count; index++)
			{
				var region = regions[index];
				if (region.Count < thresholdSize)
				{
					foreach (var point in region)
					{
						if (layer == ChunkLayer.Blocking)
						{
							chunk[layer, point.X, point.Y] = 0;
						}
						else
						{
							chunk[ChunkLayer.Blocking, point.X, point.Y] = _stoneId;
						}
					}
					regions.Remove(region);
					index--;
				}
			}
		}

		#endregion

		#region Locate regions.

		private List<List<Vector2I>> GetRegions(Chunk chunk, ChunkLayer layer, int tileId)
		{
			// This is the collection of regions we have found..
			var regions = new List<List<Vector2I>>();

			// Track whether a position has been checked.
			var mapFlags = new bool[chunk.Height, chunk.Width];

			for (var x = 0; x < chunk.Width; x++)
			{
				for (var y = 0; y < chunk.Height; y++)
				{
					if (!mapFlags[y, x])
					{
						if (((layer == ChunkLayer.Floor) && (chunk[ChunkLayer.Floor, x, y] == tileId) && (chunk[ChunkLayer.Blocking, x, y] == 0)) ||
							((layer == ChunkLayer.Blocking) && (chunk[ChunkLayer.Floor, x, y] == tileId)))
						{
							var newRegion = GetRegionPoints(chunk, layer, x, y);
							regions.Add(newRegion);

							// Mark each position in the newly-found region as checked, so we don't try putting a second region here.
							foreach (var point in newRegion)
							{
								mapFlags[point.Y, point.X] = true;
							}
						}
					}
				}
			}

			return regions;
		}

		private static List<Vector2I> GetRegionPoints(Chunk chunk, ChunkLayer layer, int startX, int startY)
		{
			// The list of tiles in this region.
			var points = new List<Vector2I>();

			// Track whether a position has been checked.
			var mapFlags = new bool[chunk.Height, chunk.Width];

			// This is the tile id we will match the region on.
			var tileId = chunk[layer, startX, startY];

			// The list of tiles to check.
			var queue = new Queue<Vector2I>();

			queue.Enqueue(new Vector2I(startX, startY));
			mapFlags[startY, startX] = true;

			while (queue.Count > 0)
			{
				var chunkPos = queue.Dequeue();
				points.Add(chunkPos);

				for (var x = chunkPos.X - 1; x <= chunkPos.X + 1; x++)
				{
					for (var y = chunkPos.Y - 1; y <= chunkPos.Y + 1; y++)
					{
						if (MathHelper.IsInRange(x, 0, chunk.Width) && MathHelper.IsInRange(y, 0, chunk.Height) && ((y == chunkPos.Y) || (x == chunkPos.X)))
						{
							if (!mapFlags[y, x])
							{
								if (((layer == ChunkLayer.Floor) && (chunk[ChunkLayer.Floor, x, y] == tileId) && (chunk[ChunkLayer.Blocking, x, y] == 0)) ||
									((layer == ChunkLayer.Blocking) && (chunk[ChunkLayer.Floor, x, y] == tileId)))
								{
									mapFlags[y, x] = true;
									queue.Enqueue(new Vector2I(x, y));
								}
							}
						}
					}
				}
			}

			return points;
		}

		#endregion

		#region Connect isolated regions.

		private void ConnectRegions(IProgress<string> progress, Chunk chunk)
		{
			progress.Report("Collecting regions...");
			var floorRegions = GetRegions(chunk, ChunkLayer.Floor, _stoneId).Select(x => new LevelRegion(x, chunk)).ToList();
			floorRegions.Sort();
			if (floorRegions.Count > 0)
			{
				floorRegions[0].IsMainRegion = true;
				floorRegions[0].IsAccessibleFromMainRegion = true;

				ConnectClosestRooms(progress, floorRegions, chunk);
			}
		}

		private void ConnectClosestRooms(IProgress<string> progress, List<LevelRegion> floorRegions, Chunk chunk, bool forceAccessibilityFromMainRegion = false)
		{
			progress.Report($"Connecting rooms (x{++_connectingRoomsCount})...");

			var roomListA = new List<LevelRegion>(); // is not accessible from main region
			var roomListB = new List<LevelRegion>(); // is accessible from main region

			if (forceAccessibilityFromMainRegion)
			{
				foreach (var room in floorRegions)
				{
					if (room.IsAccessibleFromMainRegion)
					{
						roomListB.Add(room);
					}
					else
					{
						roomListA.Add(room);
					}
				}
			}
			else
			{
				roomListA = floorRegions;
				roomListB = floorRegions;
			}

			var bestDistance = 0;
			var bestTileA = new Vector2I();
			var bestTileB = new Vector2I();
			LevelRegion bestRegionA = null;
			LevelRegion bestRegionB = null;
			var possibleConnectionFound = false;

			foreach (var regionA in roomListA)
			{
				if (!forceAccessibilityFromMainRegion)
				{
					possibleConnectionFound = false;
					if (regionA.ConnectedRegions.Count > 0)
					{
						continue;
					}
				}

				foreach (var regionB in roomListB)
				{
					if ((regionA == regionB) || (regionA.IsConnected(regionB)))
					{
						continue;
					}

					// Find the closest points between the two rooms.
					for (var tileIndexA = 0; tileIndexA < regionA.EdgeTiles.Count; tileIndexA++)
					{
						for (var tileIndexB = 0; tileIndexB < regionB.EdgeTiles.Count; tileIndexB++)
						{
							var tileA = regionA.EdgeTiles[tileIndexA];
							var tileB = regionB.EdgeTiles[tileIndexB];
							var distanceBetweenRegions = (int)(Math.Pow(tileA.X - tileB.X, 2) + Math.Pow(tileA.Y - tileB.Y, 2));

							if ((distanceBetweenRegions < bestDistance) || !possibleConnectionFound)
							{
								bestDistance = distanceBetweenRegions;
								possibleConnectionFound = true;
								bestTileA = tileA;
								bestTileB = tileB;
								bestRegionA = regionA;
								bestRegionB = regionB;
							}
						}
					}
				}

				if (possibleConnectionFound && !forceAccessibilityFromMainRegion)
				{
					progress.Report($"Creating a passage (x{++_passageCount})...");
					CreatePassage(chunk, bestRegionA, bestRegionB, bestTileA, bestTileB);
				}
			}

			if (possibleConnectionFound && forceAccessibilityFromMainRegion)
			{
				CreatePassage(chunk, bestRegionA, bestRegionB, bestTileA, bestTileB);
				ConnectClosestRooms(progress, floorRegions, chunk, true);
			}

			if (!forceAccessibilityFromMainRegion)
			{
				ConnectClosestRooms(progress, floorRegions, chunk, true);
			}
		}

		private void CreatePassage(Chunk chunk, LevelRegion regionA, LevelRegion regionB, Vector2I pointA, Vector2I pointB)
		{
			LevelRegion.ConnectRegions(regionA, regionB);

			var line = GetLine(pointA, pointB);
			foreach (var c in line)
			{
				DrawCircle(chunk, c, 1);
			}
		}

		private void DrawCircle(Chunk chunk, Vector2I c, int passageSize)
		{
			for (var x = -passageSize; x <= passageSize; x++)
			{
				for (var y = -passageSize; y <= passageSize; y++)
				{
					if (x * x + y * y <= passageSize * passageSize)
					{
						var drawX = c.X + x;
						var drawY = c.Y + y;
						if (MathHelper.IsInRange(drawX, 0, chunk.Width) && MathHelper.IsInRange(drawY, 0, chunk.Height))
						{
							chunk[ChunkLayer.Blocking, drawX, drawY] = 0;
						}
					}
				}
			}
		}

		private List<Vector2I> GetLine(Vector2I from, Vector2I to)
		{
			var line = new List<Vector2I>();

			var x = from.X;
			var y = from.Y;

			var dx = to.X - from.X;
			var dy = to.Y - from.Y;

			var inverted = false;
			var step = Math.Sign(dx);
			var gradientStep = Math.Sign(dy);

			var longest = Math.Abs(dx);
			var shortest = Math.Abs(dy);

			if (longest < shortest)
			{
				inverted = true;
				longest = Math.Abs(dy);
				shortest = Math.Abs(dx);

				step = Math.Sign(dy);
				gradientStep = Math.Sign(dx);
			}

			var gradientAccumulation = longest / 2;
			for (var i = 0; i < longest; i++)
			{
				line.Add(new Vector2I(x, y));

				if (inverted)
				{
					y += step;
				}
				else
				{
					x += step;
				}

				gradientAccumulation += shortest;
				if (gradientAccumulation >= longest)
				{
					if (inverted)
					{
						x += gradientStep;
					}
					else
					{
						y += gradientStep;
					}
					gradientAccumulation -= longest;
				}
			}

			return line;
		}

		#endregion

		#endregion

		/// <summary>
		/// A level can be composed of many regions.
		/// </summary>
		private class LevelRegion : IComparable<LevelRegion>
		{
			public List<Vector2I> Points;
			public List<Vector2I> EdgeTiles;
			public List<LevelRegion> ConnectedRegions;

			public LevelRegion(List<Vector2I> points, Chunk chunk)
			{
				Points = points;
				ConnectedRegions = new List<LevelRegion>();
				IsMainRegion = false;
				IsAccessibleFromMainRegion = false;

				LocateEdgeTiles(chunk);
			}

			public bool IsMainRegion { get; set; }

			public bool IsAccessibleFromMainRegion { get; set; }

			public int Size
			{
				get
				{
					return Points.Count;
				}
			}

			public static void ConnectRegions(LevelRegion regionA, LevelRegion regionB)
			{
				if (regionA.IsAccessibleFromMainRegion)
				{
					regionB.SetAccessibleFromMainRegion();
				}
				else if (regionB.IsAccessibleFromMainRegion)
				{
					regionA.SetAccessibleFromMainRegion();
				}

				regionA.ConnectedRegions.Add(regionB);
				regionB.ConnectedRegions.Add(regionA);
			}

			public bool IsConnected(LevelRegion otherRegion)
			{
				return ConnectedRegions.Contains(otherRegion);
			}

			public int CompareTo(LevelRegion other)
			{
				return Size.CompareTo(other.Size);
			}

			public void SetAccessibleFromMainRegion()
			{
				if (!IsAccessibleFromMainRegion)
				{
					IsAccessibleFromMainRegion = true;
					foreach (var region in ConnectedRegions)
					{
						region.SetAccessibleFromMainRegion();
					}
				}
			}

			private void LocateEdgeTiles(Chunk chunk)
			{
				var regionTileId = chunk[ChunkLayer.Floor, Points[0].X, Points[0].Y];

				// TODO: Make this configurable?
				Func<int, int, bool> isOnEdge = (x, y) => chunk[ChunkLayer.Blocking, x, y] != 0;

				EdgeTiles = new List<Vector2I>();
				foreach (var tile in Points)
				{
					for (var x = tile.X - 1; x <= tile.X + 1; x++)
					{
						for (var y = tile.Y - 1; y <= tile.Y + 1; y++)
						{
							if (MathHelper.IsInRange(x, 0, chunk.Width) && MathHelper.IsInRange(y, 0, chunk.Height) && ((x == tile.X) || (y == tile.Y)))
							{
								if (isOnEdge(x, y))
								{
									EdgeTiles.Add(tile);
								}
							}
						}
					}
				}
			}
		}
	}
}
