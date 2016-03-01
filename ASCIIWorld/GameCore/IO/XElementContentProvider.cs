using System.IO;
using System.Xml.Linq;

namespace GameCore.IO
{
	public class XElementContentProvider : XmlBasedContentProvider<XElement>
	{
		public override XElement Load(ContentManager content, FileInfo contentPath)
		{
			return LoadFile(contentPath);
		}
	}
}
