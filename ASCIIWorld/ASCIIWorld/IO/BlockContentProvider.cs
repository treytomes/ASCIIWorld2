using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using CommonCore;
using GameCore.IO;
using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace ASCIIWorld.IO
{
	public class BlockContentProvider : XmlBasedContentProvider<Block>
	{
		public override Block Parse(ContentManager content, XElement blockElem)
		{
			blockElem.RequireElement("Block");

			var name = blockElem.Attribute<string>("name");

			var isOpaque = blockElem.HasAttribute("isOpaque") ? blockElem.Attribute<bool>("isOpaque") : true;

			var tileSet = LoadTileSet(content, blockElem.Attribute<string>("tileSet"));
			
			var rendererElem = blockElem.Element("Block.renderer").Elements().Single();
			var renderer = LoadRenderer(tileSet, rendererElem);
			var tile = new Block(name, isOpaque, renderer);

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
			else if (rendererElem.Name == "RegionBoundedTileStack")
			{
				return LoadRegionBoundedTileStack(tileSet as AtlasTileSet, rendererElem);
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

		private RegionBoundedTileStack LoadRegionBoundedTileStack(TileSet tileSet, XElement tileStackElem)
		{
			var outlineColor = ConvertEx.ChangeType<Color>(tileStackElem.Attribute<string>("outlineColor"));
			var tiles = new List<Tile>();
			foreach (var tileElem in tileStackElem.Elements("Tile"))
			{
				tiles.Add(LoadTile(tileSet, tileElem));
			}

			var northWall = tileStackElem.Attribute<string>("northWall");
			var eastWall = tileStackElem.Attribute<string>("eastWall");
			var southWall = tileStackElem.Attribute<string>("southWall");
			var westWall = tileStackElem.Attribute<string>("westWall");

			return new RegionBoundedTileStack(tiles, tileSet, outlineColor, northWall, eastWall, southWall, westWall);
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
			var color = Color.White;
			if (tileElem.HasAttribute("color"))
			{
				color = tileElem.Attribute<Color>("color");
			}

			if (tileElem.HasAttribute("tileIndex"))
			{
				return new Tile(tileSet, color, tileElem.Attribute<int>("tileIndex"));
			}
			else if (tileElem.HasAttribute("name"))
			{
				return new Tile(tileSet, color, tileElem.Attribute<string>("name"));
			}
			else if (tileElem.HasAttribute("char"))
			{
				return new Tile(tileSet, color, tileElem.Attribute<char>("char"));
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
	}
}
