using GameCore.Rendering;
using System;
using System.IO;
using System.Xml.Linq;

namespace GameCore.IO
{
	public class TileSetContentProvider : XmlBasedContentProvider<TileSet>
	{
		public override TileSet Load(ContentManager content, FileInfo contentPath)
		{
			var tileSetElem = LoadFile(contentPath);
			ExpectElementName(tileSetElem, "TileSet");

			var source = GetAttributeValue<string>(tileSetElem, "source");
			var rows = GetAttributeValue<int>(tileSetElem, "rows");
			var columns = GetAttributeValue<int>(tileSetElem, "columns");

			var texture = content.Load<Texture2D>(source);

			return new TileSet(texture, rows, columns);
		}
	}
}
