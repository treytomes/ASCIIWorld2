using GameCore.Rendering;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.IO;

namespace GameCore.IO
{
	public class Texture2DContentProvider : IContentProvider<Texture2D>
	{
		public Texture2D Load(ContentManager content, FileInfo contentPath)
		{
			var image = new Bitmap(contentPath.FullName);
			var texture = new Texture2D(image.Width, image.Height)
			{
				MinificationFilter = TextureMinFilter.Nearest,
				MagnificationFilter = TextureMagFilter.Nearest
			};
			texture.WriteRegion(image);
			return texture;
		}
	}
}