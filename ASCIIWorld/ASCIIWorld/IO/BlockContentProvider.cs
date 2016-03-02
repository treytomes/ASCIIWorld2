using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
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
		public override Block Parse(ContentManager content, XElement blockElem)
		{
			blockElem.RequireElement("Block");
			var tileSet = LoadTileSet(content, blockElem.Attribute<string>("tileSet"));

			var rendererElem = blockElem.Element("Block.renderer").Elements().Single();
			var renderer = LoadRenderer(tileSet, rendererElem);
			var tile = new Block(renderer);

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

		private IBlockRenderer LoadRenderer(TileSet tileSet, XElement rendererElem)
		{
			if (rendererElem.Name == "Animation")
			{
				return LoadAnimation(tileSet, rendererElem);
			}
			else if (rendererElem.Name == "TileStack")
			{
				return LoadTileStack(tileSet, rendererElem);
			}
			else if (rendererElem.Name == "Tile")
			{
				return LoadTile(tileSet, rendererElem);
			}
			else
			{
				throw new InvalidOperationException($"Unknown renderer: {rendererElem.Name}");
			}
		}

		private Animation LoadAnimation(TileSet tileSet, XElement animationElem)
		{
			var framesPerSecond = animationElem.Attribute<int>("framesPerSecond");
			var tileStacks = new List<TileStack>();
			foreach (var tileStackElem in animationElem.Elements("TileStack"))
			{
				tileStacks.Add(LoadTileStack(tileSet, tileStackElem));
			}

			return new Animation(framesPerSecond, tileStacks);
		}

		private TileStack LoadTileStack(TileSet tileSet, XElement tileStackElem)
		{
			var tiles = new List<Tile>();
			foreach (var tileElem in tileStackElem.Elements("Tile"))
			{
				tiles.Add(LoadTile(tileSet, tileElem));
			}
			return new TileStack(tiles);
		}

		private Tile LoadTile(TileSet tileSet, XElement tileElem)
		{
			if (tileElem.HasAttribute("tileIndex"))
			{
				return new Tile(tileSet, ParseColor(tileElem.Attribute<string>("color")), tileElem.Attribute<int>("tileIndex"));
			}
			else if (tileElem.HasAttribute("name"))
			{
				return new Tile(tileSet, ParseColor(tileElem.Attribute<string>("color")), tileElem.Attribute<string>("name"));
			}
			else
			{
				throw new InvalidOperationException("Expected either 'tileIndex' or 'name'.");
			}
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
