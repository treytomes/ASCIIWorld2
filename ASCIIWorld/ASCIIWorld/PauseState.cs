using GameCore.StateManagement;
using System;
using GameCore.IO;
using GameCore.Rendering;
using GameCore.Rendering.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using GameCore;

namespace ASCIIWorld
{
	public class PauseState : GameState
	{
		#region Constants

		private const string PAUSE_MESSAGE = "PAUSED";

		#endregion

		#region Fields

		private Viewport _viewport;
		private IProjection _projection;
		private ITessellator _tessellator;
		//private GLTextWriter _writer;
		private TileSet _ascii;

		#endregion

		#region Constructors

		/// <param name="closeOnFocus">Should this state close when the game receives input focus?</param>
		public PauseState(GameStateManager manager)
			: base(manager)
		{
			_viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);
			_projection = new OrthographicProjection(_viewport)
			{
				ZNear = -10,
				ZFar = 10
			};
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_ascii = content.Load<TileSet>("TileSets/ASCII.xml");

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
			//_writer = new GLTextWriter(new Font("Consolas", 64, FontStyle.Bold));

			//var size = _writer.Measure(PAUSE_MESSAGE);
			//_writer.Position = new Vector2(Manager.GameWindow.Width - size.Width, Manager.GameWindow.Height - size.Height) / 2.0f;
			//_writer.Color = Color.White;

			InputManager.Instance.Keyboard.KeyDown += Keyboard_KeyDown;
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			InputManager.Instance.Keyboard.KeyDown -= Keyboard_KeyDown;
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);
		}

		public override void Render()
		{
			base.Render();

			_projection.Apply();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Translate(0, 0, 10);

			_tessellator.BindTexture(null);
			_tessellator.BindColor(Color.FromArgb(64, Color.Black));
			_tessellator.AddPoint(0, 0);
			_tessellator.AddPoint(0, Manager.GameWindow.Height);
			_tessellator.AddPoint(Manager.GameWindow.Width, Manager.GameWindow.Height);
			_tessellator.AddPoint(Manager.GameWindow.Width, 0);

			_tessellator.BindColor(Color.White);
			_tessellator.Scale(_ascii.Width * 4, _ascii.Height * 4);
			_ascii.RenderText(_tessellator, (Manager.GameWindow.Width - _ascii.Width * 4 * PAUSE_MESSAGE.Length) / 2, (Manager.GameWindow.Height - _ascii.Height * 4) / 2, PAUSE_MESSAGE);

			_tessellator.End();

			//_writer.Write(PAUSE_MESSAGE);
		}

		#endregion

		#region Event Handlers

		private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				if (e.Key == Key.Enter)
				{
					LeaveState();
				}
			}
		}

		#endregion
	}
}
