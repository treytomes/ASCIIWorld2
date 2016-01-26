using System;
using System.Drawing;

namespace GameCore.Platform
{
	using Graphics = System.Drawing.Graphics;

	/// <summary>
	/// Provides methods to access internal GdiPlus fields. This is necessary for managed <-> native GdiPlus interoperability.
	/// Note that the fields are named differently between .Net and Mono.
	/// GdiPlus is considered deprecated by Microsoft - it is highly unlikely that
	/// future framework upgrades will break this code, but it is something to keep in mind.
	/// </summary>
	interface IGdiPlusInternals
    {
        IntPtr GetNativeGraphics(Graphics graphics);

        IntPtr GetNativeFont(Font font);

        IntPtr GetNativeStringFormat(StringFormat format);
    }
}