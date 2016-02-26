using OpenTK;

namespace GameCore.Math
{
	/// <summary>
	/// Contains the math behind the Tessellator classes.
	/// </summary>
	public class ViewCamera : ICamera
	{
		#region Fields

		private Matrix4 _currentTransformation;

		#endregion

		#region Constructors

		public ViewCamera()
		{
			LoadIdentity();
		}

		#endregion

		#region Properties

		public Matrix4 Transformation
		{
			get
			{
				return _currentTransformation;
			}
		}

		#endregion

		#region Methods

		public void LoadIdentity()
		{
			_currentTransformation = Matrix4.Identity;
		}

		public void Scale(float x, float y)
		{
			Scale(x, y, 1);
		}

		public void Scale(float x, float y, float z)
		{
			var scale = Matrix4.CreateScale(x, y, z);
			_currentTransformation *= scale;
		}

		public void Rotate(float angle, float x, float y, float z)
		{
			var rotation = Matrix4.CreateFromAxisAngle(new Vector3(x, y, z), angle * OpenTK.MathHelper.Pi / 180.0f);
			_currentTransformation *= rotation;
		}

		public void Translate(Vector3 position)
		{
			var translation = Matrix4.CreateTranslation(position);
			_currentTransformation *= translation;
		}

		public void Translate(Vector2 position)
		{
			Translate(new Vector3(position));
		}

		public void Translate(float x, float y)
		{
			Translate(new Vector3(x, y, 0));
		}

		public void Translate(float x, float y, float z)
		{
			Translate(new Vector3(x, y, z));
		}

		public void Apply(Matrix4 matrix)
		{
			_currentTransformation *= matrix;
		}

		/// <summary>
		/// Transform <paramref name="vector"/> according to the current matrix stack.
		/// </summary>
		public Vector3 WorldToScreenPoint(Vector3 vector)
		{
			return Vector3.TransformPosition(vector, _currentTransformation);
		}

		/// <summary>
		/// Transform <paramref name="vector"/> according to the current matrix stack.
		/// </summary>
		public Vector2 WorldToScreenPoint(Vector2 vector)
		{
			var v3 = WorldToScreenPoint(new Vector3(vector));
			return new Vector2(v3.X, v3.Y);
		}

		public Vector3 ScreenToWorldPoint(Vector3 vector)
		{
			return Vector3.TransformPosition(vector, _currentTransformation.Inverted());
		}

		public Vector2 ScreenToWorldPoint(Vector2 vector)
		{
			var v3 = ScreenToWorldPoint(new Vector3(vector));
			return new Vector2(v3.X, v3.Y);
		}

		#endregion
	}
}
