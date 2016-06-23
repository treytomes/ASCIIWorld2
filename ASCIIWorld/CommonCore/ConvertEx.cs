using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;

namespace CommonCore
{
	public static class ConvertEx
	{
		private struct Conversion
		{
			public readonly Type ConvertFrom;
			public readonly Type ConvertTo;

			public Conversion(Type from, Type to)
			{
				ConvertFrom = from;
				ConvertTo = to;
			}
		}

		private struct Converter
		{
			public readonly MethodInfo ConvertWith;

			public Converter(MethodInfo with)
			{
				ConvertWith = with;
			}

			public object Convert(object value)
			{
				return ConvertWith.Invoke(null, new[] { value });
			}
		}

		private static Dictionary<Conversion, Converter> _converters;

		static ConvertEx()
		{
			_converters = new Dictionary<Conversion, Converter>();

			AddConversion<string, Color>(ParseColor);
			AddConversion<string, int>(ParseInteger);
		}

		public static void AddConversion<TFrom, TTo>(Func<TFrom, TTo> converter)
		{
			_converters.Add(new Conversion(typeof(TFrom), typeof(TTo)), new Converter(converter.Method));
		}

		public static T ChangeType<T>(object value)
		{
			return (T)ChangeType(value, typeof(T));
		}

		public static object ChangeType(object value, Type targetType)
		{
			var conversion = new Conversion(value.GetType(), targetType);

			if (_converters.ContainsKey(conversion))
			{
				value = _converters[conversion].Convert(value);
			}
			return Convert.ChangeType(value, targetType);
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

		/// <summary>
		/// Parses the integer, optionally using a character expression.
		/// </summary>
		private static int ParseInteger(string text)
		{
			if (text.StartsWith("'") && text.EndsWith("'") && (text.Length == 3))
			{
				return text[1];
			}
			else
			{
				return Convert.ToInt32(text);
			}
		}
	}
}
