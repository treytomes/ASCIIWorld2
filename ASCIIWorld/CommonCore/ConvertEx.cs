using System;
using System.Drawing;

namespace CommonCore
{
	public static class ConvertEx
	{
		public static T ChangeType<T>(object value)
		{
			if (typeof(T) == typeof(Color))
			{
				value = ParseColor(value.ToString());
			}
			return (T)Convert.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Create a color from an HTML-style color string.
		/// </summary>
		private static Color ParseColor(string argb)
		{
			if (!argb.StartsWith("#"))
			{
				throw new ArgumentException($"Expected text to start with '#': {argb}", argb);
			}

			return ColorTranslator.FromHtml(argb);
		}
	}
}
