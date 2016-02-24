using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace GameCore.Rendering
{
	/// <summary>
	/// Manage the OpenGL state.
	/// </summary>
	public static class OpenGLState
	{
		public static void SetBlendMode(BlendingFactorSrc source, BlendingFactorDest destination)
		{
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(source, destination);
		}

		public static void SetDepthMode(DepthFunction depth)
		{
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(depth);
		}

		public static void SetCullMode(CullFaceMode cull)
		{
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(cull);
		}

		public static void SetDefaultState()
		{
			SetBlendMode(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			SetDepthMode(DepthFunction.Lequal);
			SetCullMode(CullFaceMode.Back);

			//GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
			//GL.Enable(EnableCap.ColorMaterial);

			GL.Enable(EnableCap.Texture2D);

			GL.ClearColor(Color.Black);
		}
	}
}