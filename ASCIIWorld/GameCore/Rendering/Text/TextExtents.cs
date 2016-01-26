using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	/// <summary>
	/// Holds the results of a text measurement.
	/// </summary>
	public class TextExtents : IDisposable
	{
		#region Fields

		public static readonly TextExtents Empty;

		protected RectangleF _textExtents;
		protected List<RectangleF> _glyphExtents;

		#endregion

		#region Constructors

		static TextExtents()
		{
			Empty = new TextExtents();
		}

		internal TextExtents()
		{
			_glyphExtents = new List<RectangleF>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the bounding box of the measured text.
		/// </summary>
		public RectangleF BoundingBox
		{
			get
			{
				return _textExtents;
			}
			internal set
			{
				_textExtents = value;
			}
		}

		/// <summary>
		/// Gets the extents of each glyph in the measured text.
		/// </summary>
		/// <param name="i">The index of the glyph.</param>
		/// <returns>The extents of the specified glyph.</returns>
		public RectangleF this[int i]
		{
			get
			{
				return _glyphExtents[i];
			}
			internal set
			{
				_glyphExtents[i] = value;
			}
		}

		/// <summary>
		/// Gets the extents of each glyph in the measured text.
		/// </summary>
		public IEnumerable<RectangleF> GlyphExtents
		{
			get
			{
				return _glyphExtents;
			}
		}

		/// <summary>
		/// Gets the number of the measured glyphs.
		/// </summary>
		public int Count
		{
			get
			{
				return _glyphExtents.Count;
			}
		}

		#endregion

		#region Methods

		internal void Add(RectangleF glyphExtent)
		{
			_glyphExtents.Add(glyphExtent);
		}

		internal void AddRange(IEnumerable<RectangleF> glyphExtents)
		{
			_glyphExtents.AddRange(glyphExtents);
		}

		internal void Clear()
		{
			BoundingBox = RectangleF.Empty;
			_glyphExtents.Clear();
		}

		internal TextExtents Clone()
		{
			var extents = new TextExtents();
			extents._glyphExtents.AddRange(GlyphExtents);
			extents.BoundingBox = BoundingBox;
			return extents;
		}

		/// <summary>
		/// Frees the resources consumed by this TextExtents instance.
		/// </summary>
		public virtual void Dispose()
		{
		}

		#endregion
	}
}