using System;
using System.Xml.Linq;

namespace GameCore
{
	public static class XElementExtensions
	{
		public static TAttribute Attribute<TAttribute>(this XElement @this, XName name)
		{
			var attribute = @this.Attribute(name);
			if (attribute == null)
			{
				throw new Exception(string.Format("Unable to find attribute '{0}'", name));
			}
			return (TAttribute)Convert.ChangeType(attribute.Value, typeof(TAttribute));
		}

		public static bool HasAttribute(this XElement @this, XName name)
		{
			return @this.Attribute(name) != null;
		}
	}
}
