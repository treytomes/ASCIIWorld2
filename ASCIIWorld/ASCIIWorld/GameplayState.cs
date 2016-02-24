using ASCIIWorld.Data;
using ASCIIWorld.Generation;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using GameCore.Rendering.Text;
using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Drawing;

namespace ASCIIWorld
{
	public class GameplayState : GameState
	{
		#region Fields

		private Viewport _viewport;
		private OrthographicProjection _projection;
		private OrthographicProjection _chunkProjection;

		private GLTextWriter _writer;
		private Chunk _chunk;
		private BlockRegistry _blocks;

		// These should be moved up to the GameWindow level.
		private int _frameCount;
		private TimeSpan _totalGameTime;
		private Stopwatch _timer;
		private Vector2 _cameraVelocity;

		#endregion

		#region Constructors

		// TODO: All game states will need to handle the Resize event.
		public GameplayState(GameStateManager manager)
			: base(manager)
		{
			_viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);
			_projection = new OrthographicProjection(_viewport)
			{
				ZNear = -10,
				ZFar = 10
			};
			_chunkProjection = new OrthographicProjection(_viewport)
			{
				ZNear = -10,
				ZFar = 10
			};

			_frameCount = 0;
			_totalGameTime = TimeSpan.Zero;
			_timer = Stopwatch.StartNew();
		}

		#endregion

		#region Properties

		public bool IsPaused
		{
			get
			{
				return Manager.CurrentState is PauseState;
			}
			set
			{
				if (IsPaused != value)
				{
					if (!value)
					{
						if (IsPaused)
						{
							Manager.LeaveState();
						}
					}
					else if (value)
					{
						if (!IsPaused)
						{
							Manager.EnterState(new PauseState(Manager));
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_writer = new GLTextWriter();

			_blocks = new SampleBlockRegistry(content);
			_chunk = new ChunkGenerator(_blocks).Generate();

			InputManager.Instance.Keyboard.KeyDown += Keyboard_KeyDown;
			InputManager.Instance.Keyboard.KeyUp += Keyboard_KeyUp;
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			InputManager.Instance.Keyboard.KeyDown -= Keyboard_KeyDown;
			InputManager.Instance.Keyboard.KeyUp -= Keyboard_KeyUp;
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			_frameCount++;
			_totalGameTime = _totalGameTime.Add(elapsed);

			if (HasFocus)
			{
				_blocks.Update(elapsed);

				_chunkProjection.MoveBy(_cameraVelocity);
			}
			else
			{
				IsPaused = true;
			}
		}

		private TimeSpan _lastRenderTime = TimeSpan.Zero;
		public override void Render()
		{
			base.Render();

			_chunk.Render(_chunkProjection);

			_projection.Apply();

			_writer.Color = Color.White;
			_writer.Position = new Vector2(256, 256);
			_writer.Write("Hello, world!");

			_writer.Position = new Vector2(256, 300);
			_writer.Write("Update FPS: {0}", _frameCount / _totalGameTime.TotalSeconds);
			
			_writer.Position = new Vector2(256, 320);
			_writer.Write("Render FPS: {0}", 1.0 / (_timer.Elapsed.TotalSeconds - _lastRenderTime.TotalSeconds));
			_lastRenderTime = _timer.Elapsed;
        }

		#endregion

		#region Event Handlers

		private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				switch (e.Key)
				{
					case Key.Escape:
						LeaveState();
						break;
					case Key.P:
						EnterState(new PauseState(Manager));
						break;
					case Key.Up:
						_cameraVelocity.Y = -1;
						break;
					case Key.Down:
						_cameraVelocity.Y = 1;
						break;
					case Key.Left:
						_cameraVelocity.X = -1;
						break;
					case Key.Right:
						_cameraVelocity.X = 1;
						break;
				}
			}
		}

		private void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				switch (e.Key)
				{
					case Key.Escape:
						LeaveState();
						break;
					case Key.P:
						EnterState(new PauseState(Manager));
						break;
					case Key.Up:
					case Key.Down:
						_cameraVelocity.Y = 0;
						break;
					case Key.Left:
					case Key.Right:
						_cameraVelocity.X = 0;
						break;
				}
			}
		}

		#endregion
	}
}

