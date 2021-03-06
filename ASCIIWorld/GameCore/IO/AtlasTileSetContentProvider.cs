﻿using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using CommonCore;
using OpenTK;
using CommonCore.Math;

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
				source = LoadSource(content, tileElem.Element("Tile.source").Elements().Single());
			}

			return new TileInfo(name, source);
		}

		private ITileContentSource LoadSource(string source)
		{
			throw new NotImplementedException();
		}

		private ITileContentSource LoadSource(ContentManager content, XElement sourceElem)
		{
			if (sourceElem.Name == "TileSetContentSource")
			{
				return LoadTileSetSource(content, sourceElem);
			}
			else if (sourceElem.Name == "BitmapTileContentSource")
			{
				return LoadBitmapSource(content, sourceElem);
			}
			else if (sourceElem.Name == "TileStackContentSource")
			{
				return LoadTileStackSource(content, sourceElem);
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

			var color = Color.White;
			if (sourceElem.HasAttribute("color"))
			{
				color = sourceElem.Attribute<Color>("color");
			}

			var translateX = 0;
			if (sourceElem.HasAttribute("translateX"))
			{
				translateX = sourceElem.Attribute<int>("translateX");
			}

			var translateY = 0;
			if (sourceElem.HasAttribute("translateY"))
			{
				translateY = sourceElem.Attribute<int>("translateY");
			}

			var rotate = 0.0f;
			if (sourceElem.HasAttribute("rotate"))
			{
				rotate = sourceElem.Attribute<float>("rotate");
			}

			var tileSetElem = content.Load<XElement>(tileSetName, false);
			var bitmap = content.Load<Bitmap>(tileSetElem.Attribute<string>("source"));
			var rows = tileSetElem.Attribute<int>("rows");
			var columns = tileSetElem.Attribute<int>("columns");

			var tileSet = new BitmapTileSet(bitmap, rows, columns);

			return new TileSetTileContentSource(tileSet, tileIndex, color, rotate, new Vector2I(translateX, translateY));
		}

		private BitmapTileContentSource LoadBitmapSource(ContentManager content, XElement sourceElem)
		{
			var color = Color.White;
			if (sourceElem.HasAttribute("color"))
			{
				color = sourceElem.Attribute<Color>("color");
			}

			var bitmap = content.Load<Bitmap>(sourceElem.Attribute<string>("source"));
			var x = sourceElem.Attribute<int>("x");
			var y = sourceElem.Attribute<int>("y");
			var width = sourceElem.Attribute<int>("width");
			var height = sourceElem.Attribute<int>("height");

			return new BitmapTileContentSource(bitmap, color, new RectangleF(x, y, width, height));
		}

		private TileStackContentSource LoadTileStackSource(ContentManager content, XElement sourceElem)
		{
			var tiles = new List<ITileContentSource>();
			foreach (var elem in sourceElem.Elements())
			{
				tiles.Add(LoadSource(content, elem));
			}
			return new TileStackContentSource(tiles);
		}
	}
}
