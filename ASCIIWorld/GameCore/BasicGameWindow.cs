using GameCore.IO;
using GameCore.Rendering;
using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace GameCore
{
	public class BasicGameWindow : GameWindow, IGameWindow
	{
		#region Constructors

		public BasicGameWindow(int width, int height, string title, Icon icon, string contentRootPath)
			: base(width, height, new GraphicsMode(new ColorFormat(32), 1, 0, 4, new ColorFormat(32), 2), title)
		{
			Icon = icon;
			Content = new ContentManager(contentRootPath);
			States = new GameStateManager(this, Content);

			InputManager.Initialize(this);

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

		#region Methods

		/// <summary>
		/// Toggle the window to either fullscreen or windowed mode.
		/// </summary>
		public void ToggleFullscreen()
		{
			if (WindowState != WindowState.Normal)
			{
				//DisplayDevice.GetDisplay(DisplayIndex.Default).RestoreResolution();
				WindowState = WindowState.Normal;
			}
			else
			{
				//DisplayDevice.GetDisplay(DisplayIndex.Default).ChangeResolution(1280, 720, 32, DisplayDevice.GetDisplay(DisplayIndex.Default).RefreshRate);
				WindowState = WindowState.Fullscreen;
			}
		}

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
			States.Resize(new Viewport(0, 0, Width, Height));
		}

		private void BasicGameWindow_UpdateFrame(object sender, OpenTK.FrameEventArgs e)
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

		private void BasicGameWindow_RenderFrame(object sender, OpenTK.FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			States.Render();
			SwapBuffers();
		}

		#endregion
	}
}
