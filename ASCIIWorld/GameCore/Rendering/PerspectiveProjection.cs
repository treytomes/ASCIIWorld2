using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace GameCore.Rendering
{
	public class PerspectiveProjection : IProjection
	{
		#region Constants

		private const float DEFAULT_FIELDOFVIEW = MathHelper.PiOver4;
		private const float DEFAULT_ZNEAR = 1;
		private const float DEFAULT_ZFAR = 64;

		#endregion

		#region Fields

		private Matrix4 _projection;

		#endregion

		#region Constructors

		public PerspectiveProjection(Viewport viewport)
		{
			Resize(viewport);

			FieldOfViewY = DEFAULT_FIELDOFVIEW;
			ZNear = DEFAULT_ZNEAR;
			ZFar = DEFAULT_ZFAR;

			_projection = Matrix4.Identity;
		}

		#endregion

		#region Properties

		public Viewport Viewport { get; private set; }

		/// <summary>
		/// The field of view in the y direction, in radians.
		/// </summary>
		public float FieldOfViewY { get; set; }

		/// <summary>
		/// The distance to the near clipping plane.
		/// </summary>
		public float ZNear { get; set; }

		/// <summary>
		/// The distance to the far clipping plane.
		/// </summary>
		public float ZFar { get; set; }

		public Matrix4 ProjectionMatrix
		{
			get
			{
				return _projection;
			}
		}

		#endregion

		#region Methods

		public void Resize(Viewport viewport)
		{
			Viewport = viewport.Clone();
		}

		public void Apply()
		{
			Viewport.Apply();
			_projection = Matrix4.CreatePerspectiveFieldOfView(FieldOfViewY, Viewport.AspectRatio, ZNear, ZFar);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref _projection);
		}
		
		public bool Contains(float x, float y, float z = 0)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}

		#endregion
	}
}
