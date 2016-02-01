using ASCIIWorld.Data;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ASCIIWorld.IO
{
	public class BlockContentProvider : XmlBasedContentProvider<Block>
	{
		private const int SOLID_TILE = 219;

		public override Block Load(ContentManager content, FileInfo contentPath)
		{
			var tileElem = LoadFile(contentPath);
			ExpectElementName(tileElem, "Block");
			var tileSet = content.Load<TileSet>(tileElem.Attribute<string>("tileSet"));

			var framesElem = tileElem.Element("Frames");
			var framesPerSecond = framesElem.Attribute<int>("framesPerSecond");

			var frames = new List<TileStack>();
			foreach (var frameElem in framesElem.Elements("Frame"))
			{
				var tiles = new List<Tile>();
				if (frameElem.HasAttribute("backgroundColor")) // background color is optional
				{
					tiles.Add(new Tile(tileSet, ParseColor(frameElem.Attribute<string>("backgroundColor")), SOLID_TILE));
				}
				tiles.Add(new Tile(tileSet, ParseColor(frameElem.Attribute<string>("foregroundColor")), frameElem.Attribute<int>("tileIndex")));
				frames.Add(new TileStack(tiles));
			}

			var tile = new Block(framesPerSecond, frames);

			var propertiesElem = tileElem.Element("Properties");
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

		/// <summary>
		/// Create a color from an HTML-style color string.
		/// </summary>
		private Color ParseColor(string argb)
		{
			return ColorTranslator.FromHtml(argb);
		}
	}
}
