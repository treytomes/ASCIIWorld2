using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace GameCore.Rendering
{
	public interface ITessellator
	{
		PrimitiveType PrimitiveType { get; }

		void Begin(PrimitiveType primitiveType);
		void End();

		void LoadIdentity();
		void Rotate(float angle, float x, float y, float z);
		void Scale(float x, float y);
		void Scale(float x, float y, float z);
		void Translate(float x, float y, float z);
		void Translate(float x, float y);
		void Translate(Vector3 position);
		void Translate(Vector2 position);

		void BindTexture(Texture2D texture);
		void BindColor(Color color);

		/// <summary>
		/// Bind an integer color in the format of 0xRRGGBB.
		/// </summary>
		void BindColor(int color);

		void BindColor(byte red, byte green, byte blue, byte alpha = 255);
		void BindColor(float red, float green, float blue, float alpha = 1.0f);

		void AddPoint(float x, float y, float z, float u, float v);
		void AddPoint(float x, float y, float z);

		void AddPoint(float x, float y, float u, float v);
		void AddPoint(float x, float y);

		Vector3 Transform(Vector3 vector);
		Vector2 Transform(Vector2 vector);
	}
}