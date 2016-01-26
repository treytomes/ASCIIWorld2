using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	class GlyphEnumerator : IEnumerator<Glyph>
	{
		#region Fields

		string _text;
		Font _font;

		IEnumerator<char> _implementation;

		#endregion

		#region Constructors

		public GlyphEnumerator(string text, Font font)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			_text = text;
			_font = font;

			_implementation = text.GetEnumerator();
		}

		#endregion

		#region Properties

		public Glyph Current
		{
			get
			{
				return new Glyph(_implementation.Current, _font);
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return new Glyph(_implementation.Current, _font);
			}
		}

		#endregion

		#region Methods

		public void Dispose()
		{
			_implementation.Dispose();
		}

		public bool MoveNext()
		{
			bool status;
			do
			{
				status = _implementation.MoveNext();
			} while (status && ((_implementation.Current == '\n') || (_implementation.Current == '\r')));

			return status;
		}

		public void Reset()
		{
			_implementation.Reset();
		}

		#endregion
	}
}
