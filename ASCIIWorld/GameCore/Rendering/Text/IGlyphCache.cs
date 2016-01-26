using System;

namespace GameCore.Rendering.Text
{
	[Obsolete]
	interface IGlyphCache : IDisposable
	{
		void Add(Glyph glyph, IGlyphRasterizer rasterizer, TextQuality quality);
		bool Contains(Glyph glyph);
		CachedGlyphInfo this[Glyph glyph] { get; }
		void Clear();
	}
}