using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	/// <summary>
	/// Uniquely identifies a block of text. This structure can be used to identify text blocks for caching.
	/// </summary>
	struct TextBlock : IEquatable<TextBlock>, IEnumerable<Glyph>
	{
		#region Fields

		public readonly string Text;

		public readonly Font Font;

		public readonly RectangleF Bounds;

		public readonly TextPrinterOptions Options;

		public readonly TextAlignment Alignment;

		public readonly TextDirection Direction;

		public readonly int UsageCount;

		#endregion

		#region Constructors

		public TextBlock(string text, Font font, RectangleF bounds, TextPrinterOptions options, TextAlignment alignment, TextDirection direction)
		{
			Text = text;
			Font = font;
			Bounds = bounds;
			Options = options;
			Alignment = alignment;
			Direction = direction;
			UsageCount = 0;
		}

		#endregion

		#region Properties

		public Glyph this[int i]
		{
			get
			{
				return new Glyph(Text[i], Font);
			}
		}

		#endregion

		#region Methods

		public override int GetHashCode()
		{
			return Text.GetHashCode() ^ Font.GetHashCode() ^ Bounds.GetHashCode() ^ Options.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is TextBlock) && Equals((TextBlock)obj);
		}

		public bool Equals(TextBlock other)
		{
			return
				(Text == other.Text) &&
				(Font == other.Font) &&
				(Bounds == other.Bounds) &&
				(Options == other.Options);
		}

		public IEnumerator<Glyph> GetEnumerator()
		{
			return new GlyphEnumerator(Text, Font);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new GlyphEnumerator(Text, Font);
		}

		#endregion
	}
}
