using System;
using System.IO;
using System.Xml.Linq;

namespace GameCore.IO
{
	public abstract class XmlBasedContentProvider<T> : IContentProvider<T>, IXmlContentParser<T>
	{
		public abstract T Parse(ContentManager content, XElement elem);

		public T Load(ContentManager content, FileInfo contentPath)
		{
			return Parse(content, XElement.Load(contentPath.FullName));
		}
	}
}
