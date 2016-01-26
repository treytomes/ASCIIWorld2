using OpenTK;
using System;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	/// <summary>
	/// Provides methods to perform layout and print hardware accelerated text.
	/// </summary>
	public sealed class TextPrinter : ITextPrinter
	{
		#region Fields

		IGlyphRasterizer glyph_rasterizer;
		ITextOutputProvider text_output;
		TextQuality text_quality;

		bool _disposed;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new TextPrinter instance.
		/// </summary>
		public TextPrinter()
			: this(null, null, TextQuality.Default)
		{
		}

		/// <summary>
		/// Constructs a new TextPrinter instance with the specified TextQuality level.
		/// </summary>
		/// <param name="quality">The desired TextQuality of this TextPrinter.</param>
		public TextPrinter(TextQuality quality)
			: this(null, null, quality)
		{
		}

		TextPrinter(IGlyphRasterizer rasterizer, ITextOutputProvider output, TextQuality quality)
		{
			glyph_rasterizer = rasterizer;
			text_output = output;
			text_quality = quality;
		}

		#endregion

		#region ITextPrinter Members

		#region Print

		/// <summary>
		/// Prints text using the specified color and layout options.
		/// </summary>
		/// <param name="text">The System.String to print.</param>
		/// <param name="font">The System.Drawing.Font that will be used to print text.</param>
		/// <param name="color">The System.Drawing.Color that will be used to print text.</param>
		public void Render(string text, Font font, Color color)
		{
			Render(text, font, color, RectangleF.Empty, TextPrinterOptions.Default, TextAlignment.Near, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Prints text using the specified color and layout options.
		/// </summary>
		/// <param name="text">The System.String to print.</param>
		/// <param name="font">The System.Drawing.Font that will be used to print text.</param>
		/// <param name="color">The System.Drawing.Color that will be used to print text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		public void Render(string text, Font font, Color color, RectangleF rect)
		{
			Render(text, font, color, rect, TextPrinterOptions.Default, TextAlignment.Near, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Prints text using the specified color and layout options.
		/// </summary>
		/// <param name="text">The System.String to print.</param>
		/// <param name="font">The System.Drawing.Font that will be used to print text.</param>
		/// <param name="color">The System.Drawing.Color that will be used to print text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to print text.</param>
		public void Render(string text, Font font, Color color, RectangleF rect, TextPrinterOptions options)
		{
			Render(text, font, color, rect, options, TextAlignment.Near, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Prints text using the specified color and layout options.
		/// </summary>
		/// <param name="text">The System.String to print.</param>
		/// <param name="font">The System.Drawing.Font that will be used to print text.</param>
		/// <param name="color">The System.Drawing.Color that will be used to print text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to print text.</param>
		/// <param name="alignment">The OpenTK.Graphics.TextAlignment that will be used to print text.</param>
		public void Render(string text, Font font, Color color, RectangleF rect, TextPrinterOptions options, TextAlignment alignment)
		{
			Render(text, font, color, rect, options, alignment, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Prints text using the specified color and layout options.
		/// </summary>
		/// <param name="text">The System.String to print.</param>
		/// <param name="font">The System.Drawing.Font that will be used to print text.</param>
		/// <param name="color">The System.Drawing.Color that will be used to print text.</param>
		/// <param name="position">The OpenTK.Vector2 that defines the position for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to print text.</param>
		/// <param name="alignment">The OpenTK.Graphics.TextAlignment that will be used to print text.</param>
		/// <param name="direction">The OpenTK.Graphics.TextDirection that will be used to print text.</param>
		public void Render(string text, Font font, Color color, Vector2 position, TextPrinterOptions options, TextAlignment alignment, TextDirection direction)
		{
			Render(text, font, color, new RectangleF(position.X, position.Y, 0, 0), options, alignment, direction);
		}

		/// <summary>
		/// Prints text using the specified color and layout options.
		/// </summary>
		/// <param name="text">The System.String to print.</param>
		/// <param name="font">The System.Drawing.Font that will be used to print text.</param>
		/// <param name="color">The System.Drawing.Color that will be used to print text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to print text.</param>
		/// <param name="alignment">The OpenTK.Graphics.TextAlignment that will be used to print text.</param>
		/// <param name="direction">The OpenTK.Graphics.TextDirection that will be used to print text.</param>
		public void Render(string text, Font font, Color color, RectangleF rect, TextPrinterOptions options, TextAlignment alignment, TextDirection direction)
		{
			Contract.Requires(!_disposed, new ObjectDisposedException(GetType().ToString()).Message);
			Contract.Requires(!string.IsNullOrEmpty(text));
			Contract.Requires(font != null);
			Contract.Requires((rect.Width >= 0) && (rect.Height >= 0));

			var block = new TextBlock(text, font, rect, options, alignment, direction);
			TextOutput.Render(ref block, color, Rasterizer);
		}

		#endregion

		#region Measure

		/// <summary>
		/// Measures text using the specified layout options.
		/// </summary>
		/// <param name="text">The System.String to measure.</param>
		/// <param name="font">The System.Drawing.Font that will be used to measure text.</param>
		/// <returns>An OpenTK.Graphics.TextExtents instance that contains the results of the measurement.</returns>
		public TextExtents Measure(string text, Font font)
		{
			return Measure(text, font, RectangleF.Empty, TextPrinterOptions.Default, TextAlignment.Near, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Measures text using the specified layout options.
		/// </summary>
		/// <param name="text">The System.String to measure.</param>
		/// <param name="font">The System.Drawing.Font that will be used to measure text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <returns>An OpenTK.Graphics.TextExtents instance that contains the results of the measurement.</returns>
		public TextExtents Measure(string text, Font font, RectangleF rect)
		{
			return Measure(text, font, rect, TextPrinterOptions.Default, TextAlignment.Near, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Measures text using the specified layout options.
		/// </summary>
		/// <param name="text">The System.String to measure.</param>
		/// <param name="font">The System.Drawing.Font that will be used to measure text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to measure text.</param>
		/// <returns>An OpenTK.Graphics.TextExtents instance that contains the results of the measurement.</returns>
		public TextExtents Measure(string text, Font font, RectangleF rect, TextPrinterOptions options)
		{
			return Measure(text, font, rect, options, TextAlignment.Near, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Measures text using the specified layout options.
		/// </summary>
		/// <param name="text">The System.String to measure.</param>
		/// <param name="font">The System.Drawing.Font that will be used to measure text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to measure text.</param>
		/// <param name="alignment">The OpenTK.Graphics.TextAlignment that will be used to measure text.</param>
		/// <returns>An OpenTK.Graphics.TextExtents instance that contains the results of the measurement.</returns>
		public TextExtents Measure(string text, Font font, RectangleF rect, TextPrinterOptions options, TextAlignment alignment)
		{
			return Measure(text, font, rect, options, alignment, TextDirection.LeftToRight);
		}

		/// <summary>
		/// Measures text using the specified layout options.
		/// </summary>
		/// <param name="text">The System.String to measure.</param>
		/// <param name="font">The System.Drawing.Font that will be used to measure text.</param>
		/// <param name="rect">The System.Drawing.Rectangle that defines the bounds for text layout.</param>
		/// <param name="options">The OpenTK.Graphics.TextPrinterOptions that will be used to measure text.</param>
		/// <param name="alignment">The OpenTK.Graphics.TextAlignment that will be used to measure text.</param>
		/// <param name="direction">The OpenTK.Graphics.TextDirection that will be used to measure text.</param>
		/// <returns>An OpenTK.Graphics.TextExtents instance that contains the results of the measurement.</returns>
		public TextExtents Measure(string text, Font font, RectangleF rect, TextPrinterOptions options, TextAlignment alignment, TextDirection direction)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(this.GetType().ToString());
			}

			Contract.Requires(!string.IsNullOrEmpty(text));
			Contract.Requires(font != null);
			Contract.Requires((rect.Width >= 0) && (rect.Height >= 0));

			var block = new TextBlock(text, font, rect, options, alignment, direction);
			return Rasterizer.MeasureText(ref block);
		}

		#endregion

		public void Clear()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(this.GetType().ToString());
			}

			TextOutput.Clear();
			Rasterizer.Clear();
		}

		/// <summary>
		/// Sets up a resolution-dependent orthographic projection.
		/// </summary>
		public void Begin()
		{
			TextOutput.Begin();
		}

		/// <summary>
		/// Restores the projection and modelview matrices to their previous state.
		/// </summary>
		public void End()
		{
			TextOutput.End();
		}

		//////#region Obsolete

		//////[Obsolete("Use TextPrinter.Print instead")]
		//////public void Draw(TextHandle handle)
		//////{
		//////	Print(handle.Text, handle.GdiPFont, Color.White);
		//////}

		//////[Obsolete("Use TextPrinter.Print instead")]
		//////public void Draw(string text, TextureFont font)
		//////{
		//////	Print(text, font.font, Color.White);
		//////}

		//////[Obsolete("Use TextPrinter.Print instead")]
		//////public void Prepare(string text, TextureFont font, out TextHandle handle)
		//////{
		//////	handle = new TextHandle(text, font.font);
		//////}

		//////#endregion

		#endregion

		#region Private Members

		IGlyphRasterizer Rasterizer
		{
			get
			{
				if (glyph_rasterizer == null)
					glyph_rasterizer = new GdiPlusGlyphRasterizer();

				return glyph_rasterizer;
			}

		}

		ITextOutputProvider TextOutput
		{
			get
			{
				if (text_output == null)
					text_output = GL1TextOutputProvider.Create(text_quality);

				return text_output;
			}
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Frees the resources consumed by this TextPrinter object.
		/// </summary>
		public void Dispose()
		{
			if (!_disposed)
			{
				TextOutput.Dispose();
				_disposed = true;
			}
		}

		#endregion
	}
}
