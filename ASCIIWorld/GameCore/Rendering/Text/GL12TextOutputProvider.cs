using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace GameCore.Rendering.Text
{
	[Obsolete]
	sealed class GL12TextOutputProvider : GL1TextOutputProvider
	{
		#region Fields

		TextQuality quality;
		GlyphCache cache;

		#endregion

		#region Constuctors

		public GL12TextOutputProvider(TextQuality quality)
		{
			this.quality = quality;

			cache = new GlyphCache<Texture2D>();
		}

		#endregion

		protected override void SetBlendFunction()
		{
			GL.BlendFunc(BlendingFactorSrc.ConstantColorExt, BlendingFactorDest.OneMinusSrcColor);
		}

		protected override void SetColor(Color color)
		{
			GL.Color3(Color.White);
			GL.BlendColor(color);
		}

		protected override TextQuality TextQuality
		{
			get
			{
				return quality;
			}
		}

		protected override GlyphCache Cache
		{
			get
			{
				return cache;
			}
		}
	}
}
