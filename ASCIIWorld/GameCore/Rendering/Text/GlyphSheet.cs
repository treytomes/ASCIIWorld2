using System;

namespace GameCore.Rendering.Text
{
	[Obsolete]
	class GlyphSheet<T> : IDisposable where T : Texture2D
	{
		#region Fields

		readonly T texture;
		readonly GlyphPacker packer;

		bool disposed;

		#endregion

		#region Constructors

		public GlyphSheet(int width, int height)
		{
			texture = (T)typeof(T).GetConstructor(new Type[] { typeof(int), typeof(int) }).Invoke(new object[] { width, height });
			//texture.MagnificationFilter = TextureMagFilter.Nearest;
			//texture.MinificationFilter = TextureMinFilter.Nearest;
			packer = new GlyphPacker(width, height);
		}

		#endregion

		#region Public Members

		public T Texture
		{
			get { return texture; }
		}

		public GlyphPacker Packer
		{
			get { return packer; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (!disposed)
			{
				texture.Dispose();
				disposed = true;
			}
		}

		#endregion
	}
}
