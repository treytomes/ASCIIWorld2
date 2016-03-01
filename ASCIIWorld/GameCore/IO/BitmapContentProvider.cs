using System.Drawing;
using System.IO;

namespace GameCore.IO
{
	public class BitmapContentProvider : IContentProvider<Bitmap>
	{
		public Bitmap Load(ContentManager content, FileInfo contentPath)
		{
			return new Bitmap(Image.FromFile(contentPath.FullName));
		}
	}
}