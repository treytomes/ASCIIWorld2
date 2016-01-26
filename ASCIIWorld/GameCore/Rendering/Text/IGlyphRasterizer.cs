using System.Drawing;

namespace GameCore.Rendering.Text
{
	interface IGlyphRasterizer
	{
		Bitmap Rasterize(Glyph glyph);
		Bitmap Rasterize(Glyph glyph, TextQuality quality);
		TextExtents MeasureText(ref TextBlock block);
		TextExtents MeasureText(ref TextBlock block, TextQuality quality);
		void Clear();
	}
}
