using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Linq;

namespace GameCore.Rendering
{
	public class AtlasTileSet : TileSet
	{
		public AtlasTileSet(TileInfo[] tiles)
		{
			var tileResolution = tiles[0].Source.Width;
			ValidateResolution(tileResolution, tiles);

			var atlasSize = CalculateAtlasSize(tiles.Length);
			var textureSize = atlasSize * tileResolution;

			var atlasBitmap = new Bitmap(textureSize, textureSize);
			var x = 0;
			var y = 0;

			using (var graphics = Graphics.FromImage(atlasBitmap))
			{
				for (var n = 0; n < tiles.Length; n++)
				{
					tiles[n].Source.Render(graphics, x, y);

					x += tileResolution;
					if (x >= textureSize)
					{
						x = 0;
						y += tileResolution;
						if (y >= textureSize)
						{
							throw new Exception("The texture atlas is too small.");
						}
					}
				}
			}

			var atlasTexture = new Texture2D(atlasBitmap.Width, atlasBitmap.Height)
			{
				MinificationFilter = TextureMinFilter.Nearest,
				MagnificationFilter = TextureMagFilter.Nearest
			};
			atlasTexture.WriteRegion(atlasBitmap);

			Initialize(atlasTexture, atlasSize, atlasSize);

			for (var n = 0; n < tiles.Length; n++)
			{
				SetTileName(n, tiles[n].Name);
			}
		}

		private void ValidateResolution(int tileResolution, TileInfo[] tiles)
		{
			for (var n = 0; n < tiles.Length; n++)
			{
				if ((tiles[n].Source.Width != tileResolution) || (tiles[n].Source.Height != tileResolution))
				{
					throw new Exception("Each texture in the atlas must be square, and of identical size to one another.");
				}
			}
		}

		private int CalculateAtlasSize(int numTiles)
		{
			var atlasSize = 1;
			while (numTiles > Enumerable.Range(1, atlasSize).Sum())
			{
				atlasSize++;
			}
			return atlasSize;
		}
	}
}