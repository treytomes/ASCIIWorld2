using GameCore.IO;
using GameCore.Rendering;
using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace GameCore
{
	public class BasicGameWindow : GameWindow, IGameWindow
	{
		#region Fields

		private Viewport _viewport;
		private IProjection _projection;

		#endregion

		#region Constructors

		public BasicGameWindow(int width, int height, string title, string contentRootPath)
			: base(width, height, new GraphicsMode(new ColorFormat(32), 1, 0, 4, new ColorFormat(32), 2), title)
		{
			_viewport = new Viewport(0, 0, Width, Height);
			_projection = new OrthographicProjection(_viewport)
			{
				ZNear = -10,
				ZFar = 10
			};

			Content = new ContentManager(contentRootPath);
			States = new GameStateManager(this, Content);

			Load += BasicGameWindow_Load;
			Resize += BasicGameWindow_Resize;
			UpdateFrame += BasicGameWindow_UpdateFrame;
			RenderFrame += BasicGameWindow_RenderFrame;
		}

		#endregion

		#region Properties

		public ContentManager Content { get; private set; }

		public GameStateManager States { get; private set; }

		#endregion

		#region Event Handlers

		/// <summary>
		/// Setup settings, load textures, sounds.
		/// </summary>
		private void BasicGameWindow_Load(object sender, EventArgs e)
		{
			VSync = VSyncMode.Adaptive;
			OpenGLState.SetDefaultState();
		}

		private void BasicGameWindow_Resize(object sender, EventArgs e)
		{
			_viewport.Width = Width;
			_viewport.Height = Height;
			_projection.Resize(_viewport);
		}

		private void BasicGameWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			if (States.CurrentState == null)
			{
				Exit();
			}
			else
			{
				States.Update(TimeSpan.FromSeconds(e.Time));
			}
		}

		private void BasicGameWindow_RenderFrame(object sender, FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			_projection.Apply();
			States.Render();
			SwapBuffers();
		}

		#endregion
	}
}
