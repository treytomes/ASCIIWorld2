using System;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	interface ITextOutputProvider : IDisposable
	{
		void Render(ref TextBlock block, Color color, IGlyphRasterizer rasterizer);
		void Clear();
		void Begin();
		void End();
	}
}
