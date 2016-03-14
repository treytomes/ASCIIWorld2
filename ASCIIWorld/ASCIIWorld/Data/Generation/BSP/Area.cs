using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation.BSP
{
	public abstract class Area
	{
		#region Constructors

		public Area(RoomGenerator roomGenerator, Random random, int x, int y, int width, int height)
		{
			Random = random;

			Bounds = new RectI(x, y, width, height);
			SubArea1 = null;
			SubArea2 = null;

			Split(roomGenerator);

			if (!HasChildren)
			{
				Room = roomGenerator.Generate(random, this);
			}
		}

		#endregion

		#region Properties

		public Area SubArea1 { get; set; }

		public Area SubArea2 { get; set; }

		/// <summary>
		/// This will only be set if HasChildren is false.
		/// </summary>
		public Room Room { get; set; }

		public RectI Bounds { get; private set; }

		public bool HasChildren
		{
			get
			{
				return (SubArea1 != null) || (SubArea2 != null);
			}
		}

		protected Random Random { get; private set; }

		#endregion

		#region Methods

		public Room GetConnectableRoom()
		{
			if (Room != null)
			{
				return Room;
			}
			else
			{
				var room1 = (SubArea1 != null) ? SubArea1.GetConnectableRoom() : null;
				var room2 = (SubArea2 != null) ? SubArea2.GetConnectableRoom() : null;

				if ((room1 == null) && (room2 == null))
				{
					return null;
				}
				else if (room1 == null)
				{
					return room2;
				}
				else if (room2 == null)
				{
					return room1;
				}
				else if (Random.NextDouble() > 0.5)
				{
					return room1;
				}
				else
				{
					return room2;
				}
			}
		}

		protected abstract void Split(RoomGenerator roomGenerator);

		#endregion
	}
}