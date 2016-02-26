using OpenTK;

namespace GameCore.Math
{
	public interface ICamera
	{
		Matrix4 Transformation { get; }

		void LoadIdentity();
		void Rotate(float angle, float x, float y, float z);
		void Scale(float x, float y);
		void Scale(float x, float y, float z);
		void Translate(float x, float y, float z);
		void Translate(float x, float y);
		void Translate(Vector3 position);
		void Translate(Vector2 position);

		/// <summary>
		/// Apply the given transformation matrix to the current transformation.
		/// </summary>
		void Apply(Matrix4 matrix);

		Vector3 WorldToScreenPoint(Vector3 vector);
		Vector2 WorldToScreenPoint(Vector2 vector);

		Vector3 ScreenToWorldPoint(Vector3 vector);
		Vector2 ScreenToWorldPoint(Vector2 vector);
	}
}
