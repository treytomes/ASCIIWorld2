using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using CommonCore;

namespace GameCore.IO
{
	public class AtlasTileSetContentProvider : XmlBasedContentProvider<AtlasTileSet>
	{
		public override AtlasTileSet Parse(ContentManager content, XElement tileSetElem)
		{
			tileSetElem.RequireElement("AtlasTileSet");

			var tileElems = tileSetElem.Elements("Tile");
			var tiles = new List<TileInfo>();
			foreach (var tileElem in tileElems)
			{
				tiles.Add(LoadTile(content, tileElem));
			}

			return new AtlasTileSet(tiles.ToArray());
		}

		private TileInfo LoadTile(ContentManager content, XElement tileElem)
		{
			var name = tileElem.Property<string>("name");

			ITileContentSource source = null;
			if (tileElem.Attribute("source") != null)
			{
				source = LoadSource(tileElem.Attribute("source").Value);
			}
			else
			{
				source = LoadSource(content, tileElem.Element("Tile.source"));
			}

			return new TileInfo(name, source);
		}

		private ITileContentSource LoadSource(string source)
		{
			throw new NotImplementedException();
		}

		private ITileContentSource LoadSource(ContentManager content, XElement source)
		{
			var sourceElem = source.Elements().Single();
			if (sourceElem.Name == "TileSetContentProvider")
			{
				return LoadTileSetSource(content, sourceElem);
			}
			else if (sourceElem.Name == "BitmapTileContentSource")
			{
				return LoadBitmapSource(content, sourceElem);
			}
			else
			{
				throw new InvalidOperationException($"{sourceElem.Name} has not been implemented.");
			}
		}

		private TileSetTileContentSource LoadTileSetSource(ContentManager content, XElement sourceElem)
		{
			var tileSetName = sourceElem.Attribute<string>("tileSet");
			var tileIndex = sourceElem.Attribute<int>("tileIndex");

			var tileSetElem = content.Load<XElement>(tileSetName, false);
			var bitmap = content.Load<Bitmap>(tileSetElem.Attribute<string>("source"));
			var rows = tileSetElem.Attribute<int>("rows");
			var columns = tileSetElem.Attribute<int>("columns");

			var tileSet = new BitmapTileSet(bitmap, rows, columns);

			return new TileSetTileContentSource(tileSet, tileIndex);
		}

		private BitmapTileContentSource LoadBitmapSource(ContentManager content, XElement sourceElem)
		{
			var bitmap = content.Load<Bitmap>(sourceElem.Attribute<string>("source"));
			var x = sourceElem.Attribute<int>("x");
			var y = sourceElem.Attribute<int>("y");
			var width = sourceElem.Attribute<int>("width");
			var height = sourceElem.Attribute<int>("height");

			return new BitmapTileContentSource(bitmap, new RectangleF(x, y, width, height));
		}
	}
}
