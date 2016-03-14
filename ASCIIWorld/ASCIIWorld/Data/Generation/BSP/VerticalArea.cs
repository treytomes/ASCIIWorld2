using System;

namespace ASCIIWorld.Data.Generation.BSP
{
	public class VerticalArea : Area
	{
		public VerticalArea(RoomGenerator roomGenerator, Random random, int x, int y, int width, int height)
			: base(roomGenerator, random, x, y, width, height)
		{
		}

		protected override void Split(RoomGenerator roomGenerator)
		{
			var minAreaHeight = roomGenerator.MinHeight + 2;
			var maxAreaHeight = roomGenerator.MaxHeight + 2;

			if (Bounds.Height < minAreaHeight)
			{
				return;
			}
			else if (Bounds.Height > maxAreaHeight)
			{
				var splitRow = Random.Next(minAreaHeight, Bounds.Height - minAreaHeight + 1);

				var height1 = splitRow;
				var height2 = Bounds.Height - splitRow + 1;

				if (height1 >= minAreaHeight)
				{
					SubArea1 = new HorizontalArea(roomGenerator, Random, Bounds.X, Bounds.Y, Bounds.Width, height1);
				}

				if (height2 >= minAreaHeight)
				{
					SubArea2 = new HorizontalArea(roomGenerator, Random, Bounds.X, Bounds.Y + splitRow - 1, Bounds.Width, height2);
				}
			}
		}
	}
}
