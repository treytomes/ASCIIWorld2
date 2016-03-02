using System.IO;
using System.Xml.Linq;

namespace GameCore.IO
{
	public class XElementContentProvider : XmlBasedContentProvider<XElement>
	{
		public override XElement Parse(ContentManager content, XElement elem)
		{
			return elem;
		}
	}
}
