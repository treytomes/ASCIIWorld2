using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore
{
	public class Camera
	{
		#region Fields

		private IProjection _projection;

		#endregion

		#region Constructors

		public Camera(IProjection projection)
		{
			_projection = projection;

			Eye = new Vector3(0, 0, 0);
			Forward = Vector3.UnitZ;
			Up = Vector3.UnitY;
		}

		#endregion

		#region Properties

		public Viewport Viewport
		{
			get
			{
				return _projection.Viewport;
			}
		}

		public Vector3 Eye { get; set; }

		public Vector3 Forward { get; set; }

		public Vector3 Up { get; set; }

		public Vector3 Target
		{
			get
			{
				return Vector3.Subtract(Eye, Forward);
			}
		}

		#endregion

		#region Methods

		public void MoveTo(Vector3 position)
		{
			Eye = position;
		}

		public void MoveBy(Vector3 amount)
		{
			Eye = Vector3.Add(Eye, amount);
		}

		public void Resize(Viewport viewport)
		{
			_projection.Resize(viewport);
		}

		public void Apply()
		{
			_projection.Apply();

			var lookat = Matrix4.LookAt(Eye, Target, Up);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref lookat);
		}

		public static Camera CreatePerspectiveCamera(int width, int height)
		{
			return CreatePerspectiveCamera(new Viewport(0, 0, width, height));
		}

		public static Camera CreatePerspectiveCamera(Viewport viewport)
		{
			return new Camera(new PerspectiveProjection(viewport));
		}

		public static Camera CreateOrthographicCamera(int width, int height)
		{
			return CreateOrthographicCamera(new Viewport(0, 0, width, height));
		}

		public static Camera CreateOrthographicCamera(Viewport viewport)
		{
			return new Camera(new OrthographicProjection(viewport));
		}

		#endregion
	}
}
