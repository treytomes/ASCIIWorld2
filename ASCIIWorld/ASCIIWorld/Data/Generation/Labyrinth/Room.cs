using CommonCore.Math;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class Room : Map
	{
		#region Constructors

		public Room(int rows, int columns)
			: base(rows, columns)
		{
			InitializeRoomCells();
		}

		#endregion

		#region Methods

		public void InitializeRoomCells()
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var column = 0; column < Columns; column++)
				{
					var cell = new Cell();

					cell.NorthSide = (row == 0) ? SideType.Wall : SideType.Empty;
					cell.SouthSide = (row == Rows - 1) ? SideType.Wall : SideType.Empty;
					cell.WestSide = (column == 0) ? SideType.Wall : SideType.Empty;
					cell.EastSide = (column == Columns - 1) ? SideType.Wall : SideType.Empty;

					this[row, column] = cell;
				}
			}
		}

		public void SetLocation(Vector2I location)
		{
			var bounds = Bounds;
			bounds.X = location.X;
			bounds.Y = location.Y;
			Bounds = bounds;
		}

		#endregion
	}
}