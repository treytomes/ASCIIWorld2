using GameCore.Rendering;
using OpenTK;

namespace ASCIIWorld.Rendering
{
	public class Transformer
	{
		private Transformer(Builder builder)
		{
			Scale = builder.Scale;
			Rotation = builder.Rotation;
			Translation = builder.Translation;
			MirrorX = builder.MirrorX;
			MirrorY = builder.MirrorY;
		}

		public Vector2 Scale { get; set; }

		public float Rotation { get; set; }

		public Vector2 Translation { get; set; }

		public bool MirrorX { get; set; }

		public bool MirrorY { get; set; }

		public void Apply(ITessellator tessellator)
		{
			var position = tessellator.Transform(Vector2.Zero);
			tessellator.Translate(-position);
			tessellator.Scale(Scale.X, Scale.Y);
			tessellator.Rotate(Rotation, 0, 0, 1);
			tessellator.Translate(Translation);
			tessellator.Translate(position);
		}

		public static Builder New()
		{
			return new Builder();
		}

		public class Builder
		{
			internal Vector2 Scale { get; private set; }
			internal float Rotation { get; private set; }
			internal Vector2 Translation { get; private set; }
			internal bool MirrorX { get; private set; }
			internal bool MirrorY { get; private set; }

			internal Builder()
			{
				Scale = Vector2.One;
				Rotation = 0.0f;
				Translation = Vector2.Zero;
				MirrorX = false;
				MirrorY = false;
			}

			public Builder SetScale(Vector2 scale)
			{
				Scale = scale;
				return this;
			}

			public Builder SetScale(float scale)
			{
				return SetScale(new Vector2(scale, scale));
			}

			public Builder SetRotation(float degrees)
			{
				Rotation = degrees;
				return this;
			}

			public Builder SetTranslation(Vector2 translation)
			{
				Translation = translation;
				return this;
			}

			public Builder SetTranslation(float x, float y)
			{
				return SetTranslation(new Vector2(x, y));
			}

			public Builder SetMirrorX(bool value)
			{
				MirrorX = value;
				return this;
			}

			public Builder SetMirrorY(bool value)
			{
				MirrorY = value;
				return this;
			}

			public Transformer Build()
			{
				return new Transformer(this);
			}
		}
	}
}