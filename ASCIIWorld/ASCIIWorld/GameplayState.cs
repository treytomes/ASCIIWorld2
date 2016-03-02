using ASCIIWorld.Data;
using ASCIIWorld.Generation;
using ASCIIWorld.Rendering;
using GameCore;
using GameCore.IO;
using GameCore.Math;
using GameCore.Rendering;
using GameCore.Rendering.Text;
using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace ASCIIWorld
{
	public class GameplayState : GameState
	{
		#region Constants

		private const float ZOOM_MIN = 3.0f;
		private const float ZOOM_MAX = 25.0f;

		#endregion

		#region Fields

		private GLTextWriter _writer;
		private BlockRegistry _blocks;
		private Chunk _chunk;
		private ChunkRenderer _chunkRenderer;

		private Camera<OrthographicProjection> _camera;
		private Camera<OrthographicProjection> _hudCamera;

		// These should be moved up to the GameWindow level.
		private int _frameCount;
		private Stopwatch _timer;
		private TimeSpan _totalGameTime;
		private TimeSpan _lastRenderTime = TimeSpan.Zero;

		private Vector3 _cameraMoveStart;

		#endregion

		#region Constructors

		// TODO: All game states will need to handle the Resize event.
		public GameplayState(GameStateManager manager, BlockRegistry blocks, Chunk chunk)
			: base(manager)
		{
			var viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);

			_blocks = blocks;
			_chunk = chunk;
			_chunkRenderer = new ChunkRenderer(viewport, _blocks);

			_frameCount = 0;
			_totalGameTime = TimeSpan.Zero;
			_timer = Stopwatch.StartNew();

			_camera = Camera.CreateOrthographicCamera(viewport);
			_hudCamera = Camera.CreateOrthographicCamera(viewport);
			_hudCamera.Projection.OrthographicSize = viewport.Height / 2;
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
			_chunkRenderer.LoadContent(content);

			InputManager.Instance.Keyboard.KeyDown += Keyboard_KeyDown;
			InputManager.Instance.Keyboard.KeyUp += Keyboard_KeyUp;
			InputManager.Instance.Mouse.ButtonDown += Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp += Mouse_ButtonUp;
			InputManager.Instance.Mouse.Move += Mouse_Move;
			InputManager.Instance.Mouse.WheelChanged += Mouse_WheelChanged;
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			InputManager.Instance.Keyboard.KeyDown -= Keyboard_KeyDown;
			InputManager.Instance.Keyboard.KeyUp -= Keyboard_KeyUp;
			InputManager.Instance.Mouse.ButtonDown -= Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp -= Mouse_ButtonUp;
			InputManager.Instance.Mouse.Move -= Mouse_Move;
			InputManager.Instance.Mouse.WheelChanged -= Mouse_WheelChanged;
		}

		public override void Resize(Viewport viewport)
		{
			base.Resize(viewport);
			_camera.Resize(viewport);
			_hudCamera.Resize(viewport);
		}


		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			_frameCount++;
			_totalGameTime = _totalGameTime.Add(elapsed);

			if (HasFocus)
			{
				_blocks.Update(elapsed);
			}
			else
			{
				IsPaused = true;
			}
		}

		public override void Render()
		{
			base.Render();

			GL.ClearColor(Color.FromArgb(48, 48, 48));
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_camera.Apply();
			_chunkRenderer.Render(_camera, _chunk);

			_hudCamera.Apply();

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
						EnterState(new PauseState(Manager));
						//LeaveState();
						break;
					//case Key.Up:
					//	_cameraVelocity.Y = -1;
					//	break;
					//case Key.Down:
					//	_cameraVelocity.Y = 1;
					//	break;
					//case Key.Left:
					//	_cameraVelocity.X = -1;
					//	break;
					//case Key.Right:
					//	_cameraVelocity.X = 1;
					//	break;
				}
			}
		}

		private void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				switch (e.Key)
				{
					//case Key.Up:
					//case Key.Down:
					//	_cameraVelocity.Y = 0;
					//	break;
					//case Key.Left:
					//case Key.Right:
					//	_cameraVelocity.X = 0;
					//	break;
				}
			}
		}

		private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (HasFocus)
			{
				if (e.Button == MouseButton.Middle)
				{
					_cameraMoveStart = new Vector3(InputManager.Instance.Mouse.X, InputManager.Instance.Mouse.Y, 0);
				}
			}
		}

		private void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void Mouse_Move(object sender, MouseMoveEventArgs e)
		{
			if (HasFocus)
			{
				if (e.Mouse.IsButtonDown(MouseButton.Middle))
				{
					var mousePosition = new Vector3(e.X, e.Y, 0);
					var delta = (_cameraMoveStart - mousePosition) / _camera.Projection.OrthographicSize;

					_camera.MoveBy(delta);
					_cameraMoveStart = mousePosition;
				}
			}
		}

		private void Mouse_WheelChanged(object sender, MouseWheelEventArgs e)
		{
			_camera.Projection.OrthographicSize = (float)Math.Ceiling(_camera.Projection.OrthographicSize - _camera.Projection.OrthographicSize * (e.DeltaPrecise / 10));
			_camera.Projection.OrthographicSize = (float)Math.Floor(GameCore.Math.MathHelper.Clamp(_camera.Projection.OrthographicSize, ZOOM_MIN, ZOOM_MAX));
		}

		#endregion
	}
}

