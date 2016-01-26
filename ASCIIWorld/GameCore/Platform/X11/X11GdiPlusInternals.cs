﻿using System;
using System.Drawing;
using System.Reflection;

namespace GameCore.Platform.X11
{
	/// <summary>
	/// Note: This class is Mono-specific, not X11-specific!
	/// It works on all platforms (windows, linux, macos) as long as we are running on Mono.
	/// </summary>
	class X11GdiPlusInternals : IGdiPlusInternals
	{
		static readonly PropertyInfo native_graphics_property, native_font_property, native_string_format_property;

		static X11GdiPlusInternals()
		{
			native_graphics_property =
				typeof(System.Drawing.Graphics).GetProperty("NativeObject", BindingFlags.Instance | BindingFlags.NonPublic);

			native_font_property =
				typeof(System.Drawing.Font).GetProperty("NativeObject", BindingFlags.Instance | BindingFlags.NonPublic);

			native_string_format_property =
				typeof(System.Drawing.StringFormat).GetProperty("NativeObject", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		#region --- IGdiPlusInternals Members ---

		public IntPtr GetNativeGraphics(System.Drawing.Graphics graphics)
		{
			return (IntPtr)native_graphics_property.GetValue(graphics, null);
		}

		public IntPtr GetNativeFont(Font font)
		{
			return (IntPtr)native_font_property.GetValue(font, null);
		}

		public IntPtr GetNativeStringFormat(StringFormat format)
		{
			return (IntPtr)native_string_format_property.GetValue(format, null);
		}

		#endregion
	}
}
