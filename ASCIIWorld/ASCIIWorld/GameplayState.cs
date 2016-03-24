﻿using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using ASCIIWorld.UI;
using CommonCore;
using CommonCore.Math;
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
		#region Constants

		private const float ZOOM_MIN = 3.0f;
		private const float ZOOM_MAX = 25.0f;

		#endregion

		#region Fields

		//private GLTextWriter _writer;

		// These should be moved up to the GameWindow level.
		private int _frameCount;
		private Stopwatch _timer;
		private TimeSpan _totalGameTime;
		private TimeSpan _lastRenderTime = TimeSpan.Zero;

		private Vector2 _cameraMoveStart;
		private Vector2 _mouseBlockPosition;

		private ITessellator _tessellator;

		private UIManager _uiManager;
		private WorldManager _worldManager;

		#endregion

		#region Constructors

		public GameplayState(GameStateManager manager, Level level)
			: base(manager)
		{
			var viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);

			_frameCount = 0;
			_totalGameTime = TimeSpan.Zero;
			_timer = Stopwatch.StartNew();

			_worldManager = new WorldManager(viewport, level);
			_uiManager = new UIManager(viewport);

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
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
							// TODO: This is verbose.  Use EnterState<PauseState>().
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

			//_writer = new GLTextWriter();
			_uiManager.LoadContent(content);

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
			_worldManager.Resize(viewport);
			_uiManager.Resize(viewport);
		}


		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			_frameCount++;
			_totalGameTime = _totalGameTime.Add(elapsed);

			if (HasFocus)
			{
				_worldManager.Update(elapsed);
				_uiManager.Update(elapsed);

				_uiManager.FPSLabel.Text = string.Format("FPS: {0:0}", _frameCount / _totalGameTime.TotalSeconds);
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

			_worldManager.Render();

			if (!_uiManager.HasMouseHover)
			{
				_tessellator.Begin(PrimitiveType.Quads);
				_tessellator.LoadIdentity();
				_tessellator.Translate(0, 0, -9); // map overlay render layer

				// Render the tile selector.
				_tessellator.BindTexture(null);
				_tessellator.BindColor(Color.FromArgb(64, Color.Black));
				_tessellator.AddPoint(_mouseBlockPosition.X, _mouseBlockPosition.Y);
				_tessellator.AddPoint(_mouseBlockPosition.X, _mouseBlockPosition.Y + 1);
				_tessellator.AddPoint(_mouseBlockPosition.X + 1, _mouseBlockPosition.Y + 1);
				_tessellator.AddPoint(_mouseBlockPosition.X + 1, _mouseBlockPosition.Y);

				//_tessellator.Scale(0.125f, 0.125f);
				//_uiManager.ToolbarItems[0].Render(_tessellator);

				_tessellator.End();
			}

			_uiManager.Render();
        }

		private void Inspect(int blockX, int blockY)
		{
			var layer = _worldManager.Level.GetHighestVisibleLayer(blockX, blockY);
			var blockId = _worldManager.Level[ChunkLayer.Ceiling, blockX, blockY];
			if (blockId > 0)
			{
				var block = BlockRegistry.Instance.GetById(blockId);
				Console.WriteLine($"{ChunkLayer.Ceiling.GetDescription()} block name: {block.Name}");
			}
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
				//switch (e.Key)
				//{
				//	case Key.Up:
				//	case Key.Down:
				//		_cameraVelocity.Y = 0;
				//		break;
				//	case Key.Left:
				//	case Key.Right:
				//		_cameraVelocity.X = 0;
				//		break;
				//}
			}
		}

		private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (HasFocus && !_uiManager.HasMouseHover)
			{
				if (e.Button == MouseButton.Left)
				{
					if (_uiManager.SelectedToolbarItem != null)
					{
						_uiManager.SelectedToolbarItem.Use(_worldManager.Level, _worldManager.Level.GetHighestVisibleLayer((int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y), (int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y);
					}
				}
				if (e.Button == MouseButton.Middle)
				{
					_cameraMoveStart = _worldManager.Camera.UnProject(e.X, e.Y);
				}
				else if (e.Button == MouseButton.Right)
				{
					Inspect((int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y);
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
				_mouseBlockPosition = _worldManager.Camera.UnProject(e.X, e.Y);

				if (e.Mouse.IsButtonDown(MouseButton.Middle))
				{
					var delta = (_cameraMoveStart - _mouseBlockPosition);

					_worldManager.Camera.MoveBy(delta);
					_cameraMoveStart = _mouseBlockPosition;
				}

				_mouseBlockPosition.X = (float)Math.Floor(_mouseBlockPosition.X);
				_mouseBlockPosition.Y = (float)Math.Floor(_mouseBlockPosition.Y);
			}
		}

		private void Mouse_WheelChanged(object sender, MouseWheelEventArgs e)
		{
			_worldManager.Camera.Projection.OrthographicSize = (float)Math.Ceiling(_worldManager.Camera.Projection.OrthographicSize - _worldManager.Camera.Projection.OrthographicSize * (e.DeltaPrecise / 10));
			_worldManager.Camera.Projection.OrthographicSize = (float)Math.Floor(CommonCore.Math.MathHelper.Clamp(_worldManager.Camera.Projection.OrthographicSize, ZOOM_MIN, ZOOM_MAX));
		}

		#endregion
	}
}

