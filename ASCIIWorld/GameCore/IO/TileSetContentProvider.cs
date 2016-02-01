using GameCore.Rendering;
using System.IO;

namespace GameCore.IO
{
	public class TileSetContentProvider : XmlBasedContentProvider<TileSet>
	{
		public override TileSet Load(ContentManager content, FileInfo contentPath)
		{
			var tileSetElem = LoadFile(contentPath);
			ExpectElementName(tileSetElem, "TileSet");

			var source = tileSetElem.Attribute<string>("source");
			var rows = tileSetElem.Attribute<int>("rows");
			var columns = tileSetElem.Attribute<int>("columns");

			var texture = content.Load<Texture2D>(source);

			return new TileSet(texture, rows, columns);
		}
	}
}
