﻿using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using CommonCore;
using GameCore.IO;
using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ASCIIWorld.IO
{
	public class BlockContentProvider : XmlBasedContentProvider<Block>
	{
		private const string DEFAULT_DESCRIPTION = "// TODO: Describe me!";
			
		public override Block Parse(ContentManager content, XElement blockElem)
		{
			blockElem.RequireElement("Block");

			var name = blockElem.Attribute<string>("name");

			var description = DEFAULT_DESCRIPTION;
			if (blockElem.HasAttribute("description"))
			{
				description = blockElem.Attribute<string>("description");
			}

			var isOpaque = blockElem.HasAttribute("isOpaque") ? blockElem.Attribute<bool>("isOpaque") : true;

			var tileSet = LoadTileSet(content, blockElem.Attribute<string>("tileSet"));
			
			var rendererElem = blockElem.Element("Block.renderer").Elements().Single();
			var renderer = LoadRenderer(tileSet, rendererElem);
			var behaviors = LoadBehaviors(blockElem);
			var block = new Block(name, isOpaque, renderer, description, behaviors);

			var propertiesElem = blockElem.Element("Properties");
			if (propertiesElem != null)
			{
				var properties = propertiesElem.Elements("Property").Select(propertyElem => new KeyValuePair<string, string>(
					propertyElem.Attribute<string>("name"),
					propertyElem.Attribute<string>("value")
				));

				foreach (var property in properties)
				{
					block.SetProperty(property.Key, property.Value);
				}
			}

			return block;
		}

		private IEnumerable<BlockBehavior> LoadBehaviors(XElement blockElem)
		{
			var behaviors = new List<BlockBehavior>();
			var behaviorsElem = blockElem.Element("Behaviors");
			if (behaviorsElem != null)
			{
				foreach (var behaviorElem in behaviorsElem.Elements("Behavior"))
				{
					if (behaviorElem.HasAttribute("type"))
					{
						behaviors.Add(LoadBehavior(behaviorElem.Attribute<string>("type")));
					}
				}
			}
			return behaviors;
		}

		private BlockBehavior LoadBehavior(string type)
		{
			return (BlockBehavior)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(type));
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
			else if (rendererElem.Name == "MetadataTileSet")
			{
				return LoadMetadataTileSet(tileSet, rendererElem);
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
			var tileStacks = new List<IRenderable>();
			foreach (var elem in animationElem.Elements())
			{
				tileStacks.Add(LoadRenderer(tileSet, elem));
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

		private MetadataTileSet LoadMetadataTileSet(TileSet tileSet, XElement tileSetElem)
		{
			var mask = tileSetElem.Attribute<int>("mask");
			var tiles = new Dictionary<int, IRenderable>();
			foreach (var tileElem in tileSetElem.Elements())
			{
				tiles.Add(tileElem.Attribute<int>("data"), LoadRenderer(tileSet, tileElem));
			}
			return new MetadataTileSet(mask, tiles);
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
				return new Tile(tileSet, tileElem.Attribute<int>("tileIndex"));
			}
			else if (tileElem.HasAttribute("name"))
			{
				return new Tile(tileSet, tileElem.Attribute<string>("name"));
			}
			else if (tileElem.HasAttribute("char"))
			{
				return new Tile(tileSet, tileElem.Attribute<char>("char"));
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
