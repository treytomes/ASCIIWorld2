using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld
{
	public class ASCIIWorldGameWindow : GameWindow
	{
		#region Fields

		private GameStateManager _states;

		#endregion

		#region Constructors

		public ASCIIWorldGameWindow()
			: base(1280, 720, GraphicsMode.Default, "ASCII World")
		{
			Load += ASCIIWorldGameWindow_Load;
			Resize += ASCIIWorldGameWindow_Resize;
			UpdateFrame += ASCIIWorldGameWindow_UpdateFrame;
			RenderFrame += ASCIIWorldGameWindow_RenderFrame;

			_states = new GameStateManager();
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Setup settings, load textures, sounds.
		/// </summary>
		private void ASCIIWorldGameWindow_Load(object sender, EventArgs e)
		{
			VSync = VSyncMode.On;

			_states.EnterState(new GameplayState(_states));
		}

		private void ASCIIWorldGameWindow_Resize(object sender, EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
		}

		private void ASCIIWorldGameWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			if (_states.CurrentState == null)
			{
				Exit();
			}
			else
			{
				_states.Update(TimeSpan.FromSeconds(e.Time));
			}
		}

		private void ASCIIWorldGameWindow_RenderFrame(object sender, FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_states.Render();

			SwapBuffers();
		}

		#endregion
	}
}
