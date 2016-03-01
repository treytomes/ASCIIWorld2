using ASCIIWorld.Data;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ASCIIWorld.IO
{
	public class BlockContentProvider : XmlBasedContentProvider<Block>
	{
		public override Block Load(ContentManager content, FileInfo contentPath)
		{
			var blockElem = LoadFile(contentPath);
			ExpectElementName(blockElem, "Block");
			var tileSet = LoadTileSet(content, blockElem.Attribute<string>("tileSet"));

			var framesElem = blockElem.Element("Frames");
			var framesPerSecond = framesElem.Attribute<int>("framesPerSecond");

			var frames = new List<TileStack>();
			foreach (var frameElem in framesElem.Elements("Frame"))
			{
				var tiles = new List<Tile>();
				foreach (var tileElem in frameElem.Elements("Tile"))
				{
					if (tileElem.Attribute("tileIndex") != null)
					{
						tiles.Add(new Tile(tileSet, ParseColor(tileElem.Attribute<string>("color")), tileElem.Attribute<int>("tileIndex")));
					}
					else if (tileElem.Attribute("name") != null)
					{
						tiles.Add(new Tile(tileSet, ParseColor(tileElem.Attribute<string>("color")), tileElem.Attribute<string>("name")));
					}
					else
					{
						throw new InvalidOperationException("Expected either 'tileIndex' or 'name'.");
					}
				}
				frames.Add(new TileStack(tiles));
			}

			var tile = new Block(framesPerSecond, frames);

			var propertiesElem = blockElem.Element("Properties");
			if (propertiesElem != null)
			{
				var properties = propertiesElem.Elements("Property").Select(propertyElem => new KeyValuePair<string, string>(
					propertyElem.Attribute<string>("name"),
					propertyElem.Attribute<string>("value")
				));

				foreach (var property in properties)
				{
					tile.SetProperty(property.Key, property.Value);
				}
			}

			return tile;
		}

		private TileSet LoadTileSet(ContentManager content, string name)
		{
			var elem = content.Load<XElement>(name);
			if (elem.Name == "TileSet")
			{
				return content.Load<TileSet>(name);
			}
			else if (elem.Name == "AtlasTileSet")
			{
				return content.Load<AtlasTileSet>(name);
			}
			else
			{
				throw new InvalidOperationException($"{name} is not implemented.");
			}
		}

		/// <summary>
		/// Create a color from an HTML-style color string.
		/// </summary>
		private Color ParseColor(string argb)
		{
			return ColorTranslator.FromHtml(argb);
		}
	}
}
