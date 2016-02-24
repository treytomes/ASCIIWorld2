using System;
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

		public float Left { get; set; }

		public float Right { get; set; }

		public float Top { get; set; }

		public float Bottom { get; set; }

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

			Left = Viewport.Left;
			Right = Viewport.Right;
			Top = Viewport.Top;
			Bottom = Viewport.Bottom;
		}

		public void Apply()
		{
			Viewport.Apply();

			var projection = Matrix4.CreateOrthographicOffCenter(Left, Right + 1, Bottom + 1, Top, ZNear, ZFar);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);
		}

		public bool Contains(float x, float y, float z = 0)
		{
			// TODO: Make this work if Top and Bottom are flipped.
			return (Left <= x) && (x <= Right) && (Top <= y) && (y <= Bottom);
		}

		public void MoveBy(Vector2 delta)
		{
			MoveBy(delta.X, delta.Y);
		}

		public void MoveBy(Vector3 delta)
		{
			MoveBy(delta.X, delta.Y, delta.Z);
		}

		public void MoveBy(float deltaX, float deltaY, float deltaZ = 0)
		{
			Top += deltaY;
			Bottom += deltaY;

			Left += deltaX;
			Right += deltaX;

			ZNear += deltaZ;
			ZFar += deltaZ;
		}

		public void MoveTo(Vector2 position)
		{
			MoveTo(position.X, position.Y);
		}

		public void MoveTo(Vector3 position)
		{
			MoveTo(position.X, position.Y, position.Z);
		}

		public void MoveTo(float x, float y, float z = 0)
		{
			Top = y;
			Bottom = y;

			Left = x;
			Right = x;

			ZNear = z;
			ZFar = z;
		}

		#endregion
	}
}
