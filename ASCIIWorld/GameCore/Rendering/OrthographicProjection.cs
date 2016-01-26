using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Rendering
{
	public class OrthographicProjection : IProjection
	{
		#region Constants

		private const float DEFAULT_ZNEAR = 1;
		private const float DEFAULT_ZFAR = -1;

		#endregion

		#region Constructors

		public OrthographicProjection(Viewport viewport)
		{
			Resize(viewport);

			ZNear = DEFAULT_ZNEAR;
			ZFar = DEFAULT_ZFAR;
		}

		#endregion

		#region Properties

		public Viewport Viewport { get; private set; }

		public int Left { get; set; }

		public int Right { get; set; }

		public int Top { get; set; }

		public int Bottom { get; set; }

		/// <summary>
		/// The distance to the near clipping plane.
		/// </summary>
		public float ZNear { get; set; }

		/// <summary>
		/// The distance to the far clipping plane.
		/// </summary>
		public float ZFar { get; set; }

		#endregion

		#region Methods

		public void Resize(Viewport viewport)
		{
			Viewport = viewport.Clone();

			Left = Viewport.X;
			Right = Viewport.Width;
			Top = Viewport.Y;
			Bottom = Viewport.Height;
		}

		public void Apply()
		{
			Viewport.Apply();

			var projection = Matrix4.CreateOrthographicOffCenter(Left, Right, Bottom, Top, ZNear, ZFar);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);
		}

		#endregion
	}
}
