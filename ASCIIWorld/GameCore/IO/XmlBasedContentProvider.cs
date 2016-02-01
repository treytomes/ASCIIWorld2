using System;
using System.IO;
using System.Xml.Linq;

namespace GameCore.IO
{
	public abstract class XmlBasedContentProvider<T> : IContentProvider<T>
	{
		public abstract T Load(ContentManager content, FileInfo contentPath);

		protected XElement LoadFile(FileInfo contentPath)
		{
			return XElement.Load(contentPath.FullName);
		}

		protected void ExpectElementName(XElement element, string name)
		{
			if (string.Compare(element.Name.LocalName, name, true) != 0)
			{
				throw new Exception(string.Format("Expected element named '{0}', found element named '{1}'.", name, element.Name));
			}
		}
	}
}
