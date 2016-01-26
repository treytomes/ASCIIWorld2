using System;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	struct Glyph : IEquatable<Glyph>
	{
		#region Fields

		char _character;
		Font _font;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Glyph that represents the given character and Font.
		/// </summary>
		/// <param name="c">The character to represent.</param>
		/// <param name="font">The Font of the character.</param>
		public Glyph(char c, Font font)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}
			_character = c;
			_font = font;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the character represented by this Glyph.
		/// </summary>
		public char Character
		{
			get
			{
				return _character;
			}
			private set
			{
				_character = value;
			}
		}

		/// <summary>
		///  Gets the Font of this Glyph.
		/// </summary>
		public Font Font
		{
			get
			{
				return _font;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Font", "Glyph font cannot be null");
				}
				_font = value;
			}
		}

		public bool IsWhiteSpace
		{
			get
			{
				return char.IsWhiteSpace(Character);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks whether the given object is equal (memberwise) to the current Glyph.
		/// </summary>
		/// <param name="obj">The obj to check.</param>
		/// <returns>True, if the object is identical to the current Glyph.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Glyph)
			{
				return Equals((Glyph)obj);
			}
			return base.Equals(obj);
		}

		/// <summary>
		/// Describes this Glyph object.
		/// </summary>
		/// <returns>Returns a System.String describing this Glyph.</returns>
		public override string ToString()
		{
			return string.Format("'{0}', {1} {2}, {3} {4}", Character, Font.Name, _font.Style, _font.Size, _font.Unit);
		}

		/// <summary>
		/// Calculates the hashcode for this Glyph.
		/// </summary>
		/// <returns>A System.Int32 containing a hashcode that uniquely identifies this Glyph.</returns>
		public override int GetHashCode()
		{
			return _character.GetHashCode() ^ _font.GetHashCode();
		}

		public bool Equals(Glyph other)
		{
			return (Character == other.Character) && (Font == other.Font);
		}

		#endregion
	}
}