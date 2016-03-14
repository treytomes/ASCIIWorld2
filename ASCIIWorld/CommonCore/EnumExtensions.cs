using System;
using System.ComponentModel;
using System.Linq;

namespace CommonCore
{
	public static class EnumExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this Enum @this)
		{
			var type = @this.GetType();
			var name = Enum.GetName(type, @this);
			return type.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
		}

		public static string GetDescription(this Enum @this)
		{
			var description = @this.GetAttribute<DescriptionAttribute>();
			return (description == null) ? @this.ToString() : description.Description;
		}
	}
}
