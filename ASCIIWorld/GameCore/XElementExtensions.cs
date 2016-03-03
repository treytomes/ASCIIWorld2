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
			return ConvertEx.ChangeType<TAttribute>(attribute.Value);
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
					return ConvertEx.ChangeType<TProperty>(propertyElement.Value);
				}
				else
				{
					throw new Exception($"Unable to find property '{name}'.");
				}
			}
		}

		/// <summary>
		/// Throw an exception if an element with the given name doesn't exist.
		/// </summary>
		public static void RequireElement(this XElement @this, string name)
		{
			if (string.Compare(@this.Name.LocalName, name, true) != 0)
			{
				throw new Exception(string.Format("Expected element named '{0}', found element named '{1}'.", name, @this.Name));
			}
		}
	}
}
