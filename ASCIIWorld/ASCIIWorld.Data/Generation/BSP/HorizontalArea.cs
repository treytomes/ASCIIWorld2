using System;

namespace ASCIIWorld.Data.Generation.BSP
{
	public class HorizontalArea : Area
	{
		public HorizontalArea(RoomGenerator roomGenerator, Random random, int x, int y, int width, int height)
			: base(roomGenerator, random, x, y, width, height)
		{
		}

		protected override void Split(RoomGenerator roomGenerator)
		{
			var minAreaWidth = roomGenerator.MinWidth + 2;
			var maxAreaWidth = roomGenerator.MaxWidth + 2;

			if (Bounds.Width < minAreaWidth)
			{
				return;
			}
			else if (Bounds.Width > maxAreaWidth)
			{
				var splitColumn = Random.Next(minAreaWidth, Bounds.Width - minAreaWidth + 1);

				var width1 = splitColumn;
				var width2 = Bounds.Width - splitColumn + 1;

				if (width1 >= minAreaWidth)
				{
					SubArea1 = new VerticalArea(roomGenerator, Random, Bounds.X, Bounds.Y, width1, Bounds.Height);
				}

				if (width2 >= minAreaWidth)
				{
					SubArea2 = new VerticalArea(roomGenerator, Random, Bounds.X + splitColumn - 1, Bounds.Y, width2, Bounds.Height);
				}
			}
		}
	}
}
