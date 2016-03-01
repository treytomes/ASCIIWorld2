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
				throw new Exception($"Unable to find attribute '{name}'");
			}
			return (TAttribute)Convert.ChangeType(attribute.Value, typeof(TAttribute));
		}

		public static bool HasAttribute(this XElement @this, XName name)
		{
			return @this.Attribute(name) != null;
		}

		/// <summary>
		/// Look for XAML-style properties.
		/// </summary>
		public static TProperty Property<TProperty>(this XElement @this, string name)
		{
			if (@this.Attribute(name) != null)
			{
				return @this.Attribute<TProperty>(name);
			}
			else
			{
				var propertyElementName = string.Format("{0}.{1}", @this.Name, name);
				var propertyElement = @this.Element(propertyElementName);
				if (propertyElement != null)
				{
					return (TProperty)Convert.ChangeType(propertyElement.Value, typeof(TProperty));
				}
				else
				{
					throw new Exception($"Unable to find property '{name}'.");
				}
			}
		}
	}
}
