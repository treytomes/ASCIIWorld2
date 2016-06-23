using CommonCore;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace GameCore.Rendering
{
	public class ImmediateModeTessellator : BaseTessellator
	{
		#region Fields

		private bool _isStarted;

		#endregion

		#region Constructors

		public ImmediateModeTessellator()
			: base()
		{
			_isStarted = false;

			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.Color4(Color.White);

			TextureChanging += BaseTessellator_TextureChanging;
			ColorChanging += BaseTessellator_ColorChanging;
		}

		#endregion

		#region Properties

		#endregion

		#region Methods

		public override void Begin(PrimitiveType primitiveType)
		{
			base.Begin(primitiveType);

			if (_isStarted)
			{
				End();
			}

			GL.Begin(primitiveType);

			_isStarted = true;
		}

		public override void End()
		{
			if (!_isStarted)
			{
				throw new Exception("You have to start before you can end.");
			}
			GL.End();
			_isStarted = false;
		}

		protected override void AddTransformedPoint(Vector3 transformedVector, float u, float v)
		{
			GL.TexCoord2(u, v);
			GL.Vertex3(transformedVector);
		}

		#endregion

		#region Event Handlers

		private void BaseTessellator_TextureChanging(object sender, PropertyChangingEventArgs<Texture2D> e)
		{
			var isStarted = _isStarted;
			if (isStarted)
			{
				End();
			}

			e.NewValue.Bind();

			if (isStarted)
			{
				Begin(PrimitiveType);
			}
		}

		private void BaseTessellator_ColorChanging(object sender, PropertyChangingEventArgs<Color> e)
		{
			GL.Color4(e.NewValue);
		}

		#endregion
	}
}