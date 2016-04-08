using ASCIIWorld.Data;
using CommonCore;
using GameCore.IO;
using System.Xml.Linq;

namespace ASCIIWorld.IO
{
	public class BlockRegistryContentProvider : XmlBasedContentProvider<BlockRegistry>
	{
		public override BlockRegistry Parse(ContentManager content, XElement elem)
		{
			elem.RequireElement("BlockRegistry");

			var registry = BlockRegistry.Instance;
			foreach (var blockElem in elem.Elements("Block"))
			{
				var id = blockElem.Attribute<int>("id");
				var source = blockElem.Attribute<string>("source");
				registry.Register(id, content.Load<Block>(source));
			}

			return registry;
		}
	}
}
