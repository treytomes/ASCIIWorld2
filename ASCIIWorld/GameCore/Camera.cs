using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore
{
	public class Camera
	{
		public static Camera<PerspectiveProjection> CreatePerspectiveCamera(int width, int height)
		{
			return CreatePerspectiveCamera(new Viewport(0, 0, width, height));
		}

		public static Camera<PerspectiveProjection> CreatePerspectiveCamera(Viewport viewport)
		{
			return new Camera<PerspectiveProjection>(new PerspectiveProjection(viewport));
		}

		public static Camera<OrthographicProjection> CreateOrthographicCamera(int width, int height)
		{
			return CreateOrthographicCamera(new Viewport(0, 0, width, height));
		}

		public static Camera<OrthographicProjection> CreateOrthographicCamera(Viewport viewport)
		{
			return new Camera<OrthographicProjection>(new OrthographicProjection(viewport));
		}
	}

	public class Camera<TProjection>
		where TProjection : IProjection
	{
		#region Constructors

		public Camera(TProjection projection)
		{
			Projection = projection;

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
				return Projection.Viewport;
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

		public TProjection Projection { get; private set; }

		public Matrix4 ModelViewMatrix
		{
			get
			{
				return Matrix4.LookAt(Eye, Target, Up);
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
			Projection.Resize(viewport);
		}

		public void Apply()
		{
			Projection.Apply();

			var lookAt = ModelViewMatrix;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref lookAt);
		}

		#endregion
	}
}
