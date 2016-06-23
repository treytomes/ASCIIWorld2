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

		private ITessellator _tessellator;
		//private GLTextWriter _writer;
		private TileSet _ascii;
		private Camera<OrthographicProjection> _hudCamera;

		#endregion

		#region Constructors

		/// <param name="closeOnFocus">Should this state close when the game receives input focus?</param>
		public PauseState(GameStateManager manager)
			: base(manager)
		{
			var viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);
			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
			_hudCamera = Camera.CreateOrthographicCamera(viewport);
			_hudCamera.Projection.OrthographicSize = viewport.Height / 2;
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_ascii = content.Load<TileSet>("TileSets/ASCII.xml");

			//_writer = new GLTextWriter(new Font("Consolas", 64, FontStyle.Bold));

			//var size = _writer.Measure(PAUSE_MESSAGE);
			//_writer.Position = new Vector2(Manager.GameWindow.Width - size.Width, Manager.GameWindow.Height - size.Height) / 2.0f;
			//_writer.Color = Color.White;
		}

		public override void Resize(Viewport viewport)
		{
			base.Resize(viewport);
			_hudCamera.Resize(viewport);
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);
		}

		public override void Render()
		{
			base.Render();

			_hudCamera.Apply();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Translate(0, 0, -10);

			_tessellator.BindTexture(null);
			_tessellator.BindColor(Color.FromArgb(64, Color.Black));
			_tessellator.AddPoint(_hudCamera.Projection.Left, _hudCamera.Projection.Top);
			_tessellator.AddPoint(_hudCamera.Projection.Left, _hudCamera.Projection.Bottom);
			_tessellator.AddPoint(_hudCamera.Projection.Right, _hudCamera.Projection.Bottom);
			_tessellator.AddPoint(_hudCamera.Projection.Right, _hudCamera.Projection.Top);

			_tessellator.BindColor(Color.White);
			var scale = new Vector2(_ascii.Width, _ascii.Height) * 4;
			_tessellator.Scale(scale.X, scale.Y);
			_tessellator.Translate(-scale.X * PAUSE_MESSAGE.Length / 2, -scale.Y / 2);
			_ascii.RenderText(_tessellator, PAUSE_MESSAGE);

			_tessellator.End();

			//_writer.Write(PAUSE_MESSAGE);
		}

		protected override void OnKeyboardKeyDown(KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				if (e.Key == Key.Escape)
				{
					LeaveState();
				}
			}
		}

		#endregion
	}
}
