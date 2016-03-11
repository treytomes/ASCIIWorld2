using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Data.Generation.BSP
{
	public class Room
	{
		#region Fields

		private Random _random;

		#endregion

		#region Constructors

		public Room(Random random, RectI bounds)
		{
			_random = random;

			HasNorthDoor = false;
			HasSouthDoor = false;
			HasEastDoor = false;
			HasWestDoor = false;
			Bounds = bounds;
		}

		#endregion

		#region Properties

		public bool HasNorthDoor { get; set; }

		public bool HasSouthDoor { get; set; }

		public bool HasEastDoor { get; set; }

		public bool HasWestDoor { get; set; }

		public RectI Bounds { get; private set; }

		/// <summary>
		/// Choose a random point within this room's bounds, excluding the walls.
		/// </summary>
		public Vector2I RandomPoint
		{
			get
			{
				return new Vector2I(_random.Next(Bounds.Left + 1, Bounds.Right), _random.Next(Bounds.Top + 1, Bounds.Bottom));
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Is this given point on this room's boundary?
		/// </summary>
		/// <remarks>
		/// The room will generate walls on it's boundary,
		/// so if this returns true it may be a good place to put a door.
		/// </remarks>
		public bool IsPointOnBounds(Vector2I point)
		{
			return
				(point.X == Bounds.Left) ||
				(point.X == Bounds.Right) ||
				(point.Y == Bounds.Top) ||
				(point.Y == Bounds.Bottom);
		}

		/// <summary>
		/// Is this given point in one of the room's corners?
		/// </summary>
		/// <remarks>
		/// A corner cannot be walked to, so it is a bad place to put a door.
		/// </remarks>
		public bool IsPointInCorner(Vector2I point)
		{
			return
				(point == Bounds.TopLeft) ||
				(point == Bounds.TopRight) ||
				(point == Bounds.BottomLeft) ||
				(point == Bounds.BottomRight);
		}

		public void Render(IChunkGenerator generator, Chunk chunk, int floorId, int wallId)
		{
			generator.Fill(chunk, ChunkLayer.Floor, Bounds.Inflate(-1), floorId);
			generator.Fill(chunk, ChunkLayer.Blocking, Bounds.Inflate(-1), 0);

			generator.DrawVerticalLine(chunk, ChunkLayer.Blocking, Bounds.Top, Bounds.Bottom, Bounds.Left, wallId);
			generator.DrawVerticalLine(chunk, ChunkLayer.Blocking, Bounds.Top, Bounds.Bottom, Bounds.Right, wallId);

			generator.DrawHorizontalLine(chunk, ChunkLayer.Blocking, Bounds.Top, Bounds.Left, Bounds.Right, wallId);
			generator.DrawHorizontalLine(chunk, ChunkLayer.Blocking, Bounds.Bottom, Bounds.Left, Bounds.Right, wallId);
		}

		#endregion
	}
}