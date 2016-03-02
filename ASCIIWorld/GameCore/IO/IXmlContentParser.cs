using System.Xml.Linq;

namespace GameCore.IO
{
	public interface IXmlContentParser<T>
	{
		T Parse(ContentManager content, XElement elem);
	}
}
