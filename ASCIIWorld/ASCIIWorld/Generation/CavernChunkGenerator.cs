﻿using ASCIIWorld.Data;
using GameCore.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ASCIIWorld.Generation
{
	public class CavernChunkGenerator : IGenerator<Chunk>
	{
		#region Constants

		private const int SMOOTH_ITERATIONS = 5;
		private const int THRESHOLD_SIZE = 50;

		#endregion

		#region Fields

		private SampleBlockRegistry _blocks;

		private int _randomFillPercent;
		private string _seed;
		private Random _random;

		private int _passageCount;
		private int _connectingRoomsCount;

		#endregion

		#region Constructors

		public CavernChunkGenerator(SampleBlockRegistry blocks, int randomFillPercent, string seed = null)
		{
			if (blocks == null)
			{
				throw new ArgumentNullException("blocks");
			}
			_blocks = blocks;

			_randomFillPercent = randomFillPercent;
			_seed = seed ?? DateTime.Now.GetHashCode().ToString();
			_random = new Random(_seed.GetHashCode());

			_passageCount = 0;
			_connectingRoomsCount = 0;
		}

		#endregion

		#region Methods

		public Chunk Generate(IProgress<string> progress)
		{
			progress.Report("Generating chunk.");

			var chunk = new Chunk();
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

			EnsureMapBorder(chunk);
		}

		private void RandomFillMap(Chunk chunk)
		{
			for (var x = 0; x < chunk.Columns; x++)
			{
				for (var y = 0; y < chunk.Rows; y++)
				{
					if (_random.Next(0, 100) < _randomFillPercent)
					{
						chunk[ChunkLayer.Blocking, y, x] = _blocks.Stone.Id;
					}
					chunk[ChunkLayer.Floor, y, x] = _blocks.Stone.Id;
				}
			}
		}

		private void SmoothMap(Chunk chunk)
		{
			for (var x = 0; x < chunk.Columns; x++)
			{
				for (var y = 0; y < chunk.Rows; y++)
				{
					var neighbourWallTiles = GetSurroundingWallCount(chunk, x, y);

					if (neighbourWallTiles > 4)
					{
						chunk[ChunkLayer.Blocking, y, x] = _blocks.Stone.Id;
					}
					else if (neighbourWallTiles < 4)
					{
						chunk[ChunkLayer.Blocking, y, x] = 0;
					}
				}
			}
		}

		private int GetSurroundingWallCount(Chunk level, int gridX, int gridY)
		{
			var wallCount = 0;
			for (var neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
			{
				for (var neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
				{
					if (MathHelper.IsInRange(neighbourX, 0, level.Columns) && MathHelper.IsInRange(neighbourY, 0, level.Rows))
					{
						if ((neighbourX != gridX) || (neighbourY != gridY))
						{
							if (level[ChunkLayer.Blocking, neighbourY, neighbourX] == _blocks.Stone.Id)
							{
								wallCount++;
							}
						}
					}
					else
					{
						wallCount++;
					}
				}
			}

			return wallCount;
		}

		private void EnsureMapBorder(Chunk chunk)
		{
			for (var x = 0; x < chunk.Columns; x++)
			{
				chunk[ChunkLayer.Blocking, 0, x] = _blocks.Stone.Id;
				chunk[ChunkLayer.Blocking, chunk.Columns - 1, x] = _blocks.Stone.Id;
			}
			for (var y = 0; y < chunk.Rows; y++)
			{
				chunk[ChunkLayer.Blocking, y, 0] = _blocks.Stone.Id;
				chunk[ChunkLayer.Blocking, y, chunk.Rows - 1] = _blocks.Stone.Id;
			}
		}

		#endregion

		#region Remove small regions.

		private void RemoveSmallRegions(Chunk chunk)
		{
			// We do this in two rounds, because the first round will probably remove some regions.
			EnsureMinimumSize(chunk, ChunkLayer.Blocking, GetRegions(chunk, ChunkLayer.Blocking, _blocks.Stone.Id), THRESHOLD_SIZE);

			var floors = GetRegions(chunk, ChunkLayer.Floor, _blocks.Stone.Id);
			EnsureMinimumSize(chunk, ChunkLayer.Floor, GetRegions(chunk, ChunkLayer.Floor, _blocks.Stone.Id), THRESHOLD_SIZE);
			floors = GetRegions(chunk, ChunkLayer.Floor, _blocks.Stone.Id);
		}

		/// <summary>
		/// If a region size is &lt;= thresholdSize, change all of it's tiles to revertTileId.
		/// </summary>
		/// <remarks>
		/// Rooms that are too small will be removed from the regions list.
		/// </remarks>
		/// <param name="thresholdSize">If a region is &lt;= this size, delete it.</param>
		/// <param name="revertPrototype">The tile prototype to use when deleting small regions.</param>
		private void EnsureMinimumSize(Chunk chunk, ChunkLayer layer, List<List<Point>> regions, int thresholdSize)
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
							chunk[layer, point.Y, point.X] = 0;
						}
						else
						{
							chunk[ChunkLayer.Blocking, point.Y, point.X] = _blocks.Stone.Id;
						}
					}
					regions.Remove(region);
					index--;
				}
			}
		}

		#endregion

		#region Locate regions.

		private List<List<Point>> GetRegions(Chunk chunk, ChunkLayer layer, int tileId)
		{
			// This is the collection of regions we have found..
			var regions = new List<List<Point>>();

			// Track whether a position has been checked.
			var mapFlags = new bool[chunk.Rows, chunk.Columns];

			for (var x = 0; x < chunk.Columns; x++)
			{
				for (var y = 0; y < chunk.Rows; y++)
				{
					if (!mapFlags[y, x])
					{
						if (((layer == ChunkLayer.Floor) && (chunk[ChunkLayer.Floor, y, x] == tileId) && (chunk[ChunkLayer.Blocking, y, x] == 0)) ||
							((layer == ChunkLayer.Blocking) && (chunk[ChunkLayer.Floor, y, x] == tileId)))
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

		private static List<Point> GetRegionPoints(Chunk chunk, ChunkLayer layer, int startX, int startY)
		{
			// The list of tiles in this region.
			var points = new List<Point>();

			// Track whether a position has been checked.
			var mapFlags = new bool[chunk.Rows, chunk.Columns];

			// This is the tile id we will match the region on.
			var tileId = chunk[layer, startY, startX];

			// The list of tiles to check.
			var queue = new Queue<Point>();

			queue.Enqueue(new Point(startX, startY));
			mapFlags[startY, startX] = true;

			while (queue.Count > 0)
			{
				var chunkPos = queue.Dequeue();
				points.Add(chunkPos);

				for (var x = chunkPos.X - 1; x <= chunkPos.X + 1; x++)
				{
					for (var y = chunkPos.Y - 1; y <= chunkPos.Y + 1; y++)
					{
						if (MathHelper.IsInRange(x, 0, chunk.Columns) && MathHelper.IsInRange(y, 0, chunk.Rows) && ((y == chunkPos.Y) || (x == chunkPos.X)))
						{
							if (!mapFlags[y, x])
							{
								if (((layer == ChunkLayer.Floor) && (chunk[ChunkLayer.Floor, y, x] == tileId) && (chunk[ChunkLayer.Blocking, y, x] == 0)) ||
									((layer == ChunkLayer.Blocking) && (chunk[ChunkLayer.Floor, y, x] == tileId)))
								{
									mapFlags[y, x] = true;
									queue.Enqueue(new Point(x, y));
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
			var floorRegions = GetRegions(chunk, ChunkLayer.Floor, _blocks.Stone.Id).Select(x => new LevelRegion(x, chunk)).ToList();
			floorRegions.Sort();
			floorRegions[0].IsMainRegion = true;
			floorRegions[0].IsAccessibleFromMainRegion = true;

			ConnectClosestRooms(progress, floorRegions, chunk);
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
			var bestTileA = new Point();
			var bestTileB = new Point();
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

		private void CreatePassage(Chunk chunk, LevelRegion regionA, LevelRegion regionB, Point pointA, Point pointB)
		{
			LevelRegion.ConnectRegions(regionA, regionB);

			var line = GetLine(pointA, pointB);
			foreach (var c in line)
			{
				DrawCircle(chunk, c, 1);
			}
		}

		private void DrawCircle(Chunk chunk, Point c, int passageSize)
		{
			for (var x = -passageSize; x <= passageSize; x++)
			{
				for (var y = -passageSize; y <= passageSize; y++)
				{
					if (x * x + y * y <= passageSize * passageSize)
					{
						var drawX = c.X + x;
						var drawY = c.Y + y;
						if (MathHelper.IsInRange(drawX, 0, chunk.Columns) && MathHelper.IsInRange(drawY, 0, chunk.Rows))
						{
							chunk[ChunkLayer.Blocking, drawY, drawX] = 0;
						}
					}
				}
			}
		}

		private List<Point> GetLine(Point from, Point to)
		{
			var line = new List<Point>();

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
				line.Add(new Point(x, y));

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
			public List<Point> Points;
			public List<Point> EdgeTiles;
			public List<LevelRegion> ConnectedRegions;

			public LevelRegion(List<Point> points, Chunk chunk)
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
				var regionTileId = chunk[ChunkLayer.Floor, Points[0].Y, Points[0].X];

				// TODO: Make this configurable?
				Func<int, int, bool> isOnEdge = (x, y) => chunk[ChunkLayer.Blocking, y, x] != 0;

				EdgeTiles = new List<Point>();
				foreach (var tile in Points)
				{
					for (var x = tile.X - 1; x <= tile.X + 1; x++)
					{
						for (var y = tile.Y - 1; y <= tile.Y + 1; y++)
						{
							if (MathHelper.IsInRange(x, 0, chunk.Columns) && MathHelper.IsInRange(y, 0, chunk.Rows) && ((x == tile.X) || (y == tile.Y)))
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