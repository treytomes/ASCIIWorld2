using ASCIIWorld.Data;
using GameCore.IO;
using GameCore.Rendering;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ASCIIWorld.IO
{
	public class TileContentProvider : XmlBasedContentProvider<Tile>
	{
		private const int SOLID_TILE = 219;

		public override Tile Load(ContentManager content, FileInfo contentPath)
		{
			var tileElem = LoadFile(contentPath);
			ExpectElementName(tileElem, "Tile");
			var tileSet = content.Load<TileSet>(GetAttributeValue<string>(tileElem, "tileSet"));

			var framesElem = tileElem.Element("Frames");
			var framesPerSecond = GetAttributeValue<int>(framesElem, "framesPerSecond");

			var frames = framesElem.Elements("Frame").Select(frameElem => new TileFrame(
				new TileLayer(tileSet, ParseColor(GetAttributeValue<string>(frameElem, "backgroundColor")), SOLID_TILE),
				new TileLayer(tileSet, ParseColor(GetAttributeValue<string>(frameElem, "foregroundColor")), GetAttributeValue<int>(frameElem, "tileIndex"))
			));

			var tile = new Tile(framesPerSecond, frames);

			var propertiesElem = tileElem.Element("Properties");
			if (propertiesElem != null)
			{
				var properties = propertiesElem.Elements("Property").Select(propertyElem => new KeyValuePair<string, string>(
					GetAttributeValue<string>(propertyElem, "name"),
					GetAttributeValue<string>(propertyElem, "value")
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
