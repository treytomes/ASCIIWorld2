using OpenTK;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GameCore.Rendering
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexBufferElement
	{
		public readonly Vector3 Position;
		public readonly int Color;
		public readonly Vector2 TextureUV;
		public Vector3 Normal;

		private VertexBufferElement(Vector3 position, int color, Vector2 textureUV, Vector3 normal)
		{
			Position = position;
			Color = color;
			TextureUV = textureUV;
			Normal = normal;
		}

		public static Builder New()
		{
			return new Builder();
		}

		public class Builder
		{
			private Vector3 _position;
			private int _color;
			private Vector2 _textureUV;
			private Vector3 _normal;

			internal Builder()
			{
				_position = Vector3.Zero;
				_color = 0;
				_textureUV = Vector2.Zero;
				_normal = Vector3.Zero;
			}

			public Builder Position(Vector3 position)
			{
				_position = position;
				return this;
			}

			public Builder Position(Vector2 position)
			{
				return Position(new Vector3(position));
			}

			public Builder Position(float x, float y, float z = 0)
			{
				return Position(new Vector3(x, y, z));
			}
			
			public Builder Color(int color)
			{
				_color = color;
				return this;
			}

			public Builder Color(Color color)
			{
				return Color(ColorHelper.ToArgb(color));
			}

			public Builder TextureUV(Vector2 textureUV)
			{
				_textureUV = textureUV;
				return this;
			}

			public Builder TextureUV(float u, float v)
			{
				return TextureUV(new Vector2(u, v));
			}

			public Builder Normal(Vector3 normal)
			{
				_normal = normal;
				return this;
			}

			public VertexBufferElement Build()
			{
				return new VertexBufferElement(_position, _color, _textureUV, _normal);
			}
		}
	}
}