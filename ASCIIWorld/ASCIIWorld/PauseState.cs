using GameCore.StateManagement;
using System;
using GameCore.IO;
using GameCore.Rendering;
using GameCore.Rendering.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace ASCIIWorld
{
	public class PauseState : GameState
	{
		#region Constants

		private const string PAUSE_MESSAGE = "PAUSED";

		#endregion

		#region Fields

		private ITessellator _tessellator;
		private GLTextWriter _writer;

		#endregion

		#region Constructors

		/// <param name="closeOnFocus">Should this state close when the game receives input focus?</param>
		public PauseState(GameStateManager manager)
			: base(manager)
		{
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
			_writer = new GLTextWriter(new Font("Consolas", 64, FontStyle.Bold));

			var size = _writer.Measure(PAUSE_MESSAGE);
			_writer.Position = new Vector2(Manager.GameWindow.Width - size.Width, Manager.GameWindow.Height - size.Height) / 2.0f;
			_writer.Color = Color.White;
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			if (HasFocus)
			{
				if (Keyboard.GetState().IsKeyDown(Key.Enter))
				{
					LeaveState();
				}
			}
		}

		public override void Render()
		{
			base.Render();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.BindTexture(null);
			_tessellator.BindColor(Color.FromArgb(64, 255, 255, 255));
			_tessellator.AddPoint(0, 0);
			_tessellator.AddPoint(0, Manager.GameWindow.Height);
			_tessellator.AddPoint(Manager.GameWindow.Width, Manager.GameWindow.Height);
			_tessellator.AddPoint(Manager.GameWindow.Width, 0);
			_tessellator.End();

			_writer.Write(PAUSE_MESSAGE);
		}

		#endregion
	}
}
