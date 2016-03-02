using GameCore.Rendering;
using System.IO;
using System.Xml.Linq;

namespace GameCore.IO
{
	public class TileSetContentProvider : XmlBasedContentProvider<TileSet>
	{
		public override TileSet Parse(ContentManager content, XElement tileSetElem)
		{
			tileSetElem.RequireElement("TileSet");

			var source = tileSetElem.Attribute<string>("source");
			var rows = tileSetElem.Attribute<int>("rows");
			var columns = tileSetElem.Attribute<int>("columns");

			var texture = content.Load<Texture2D>(source);

			return new TileSet(texture, rows, columns);
		}
	}
}
