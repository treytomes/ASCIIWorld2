using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation.BSP
{
	public class RoomGenerator
	{
		#region Fields

		/// <summary>
		/// The current number of generated rooms.
		/// </summary>
		/// <remarks>
		/// When this number equals MaxRooms, no more rooms will be generated.
		/// </remarks>
		private int _roomCount;

		#endregion

		#region Constructors

		public RoomGenerator(int maxRooms, int minWidth, int maxWidth, int minHeight, int maxHeight)
		{
			_roomCount = 0;

			MaxRooms = maxRooms;
			MinWidth = minWidth;
			MaxWidth = maxWidth;
			MinHeight = minHeight;
			MaxHeight = maxHeight;
		}

		#endregion

		#region Properties

		public int MaxRooms { get; set; }

		public int MinWidth { get; set; }

		public int MaxWidth { get; set; }

		public int MinHeight { get; set; }

		public int MaxHeight { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Reset the room count to 0.
		/// </summary>
		/// <remarks>
		/// This should be called before regenerating a dungeon.
		/// </remarks>
		public void Reset()
		{
			_roomCount = 0;
		}

		public Room Generate(Random random, Area area)
		{
			if (_roomCount >= MaxRooms)
			{
				return null;
			}

			// Don't place the room on the area's edge.
			// This will keep rooms from being adjacent to each other.
			var placeableBounds = area.Bounds.Inflate(-1);

			var width = random.Next(MinWidth, placeableBounds.Width + 1);
			var height = random.Next(MinHeight, placeableBounds.Height + 1);

			// Find a place in the area to put the room.
			while (true)
			{
				var x = random.Next(placeableBounds.Left, placeableBounds.Right - width + 1 + 1);
				var y = random.Next(placeableBounds.Top, placeableBounds.Bottom - height + 1 + 1);
				var roomBounds = new RectI(x, y, width, height);
				if (placeableBounds.Contains(roomBounds))
				{
					_roomCount++;
					return new Room(random, roomBounds);
				}
			}
		}

		#endregion
	}
}