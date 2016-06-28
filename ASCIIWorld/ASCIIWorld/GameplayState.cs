using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using ASCIIWorld.UI;
using CommonCore;
using CommonCore.Math;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

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
			_uiManager = new UIManager(manager.GameWindow, viewport, _worldManager);

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
		}

		#endregion

		#region Properties

		// TODO: This freaks out if you pause by alt-tabbing.
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

			EntityRenderManager.Instance.LoadContent(content);
			ItemRenderManager.Instance.LoadContent(content);

			//_writer = new GLTextWriter();
			_uiManager.LoadContent(content);
			_uiManager.SaveButton.Clicked += (sender, e) => _worldManager.Save("test.sav");
			_uiManager.LoadButton.Clicked += (sender, e) => _worldManager.Load("test.sav");
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
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
				// TODO: This should be attached to the window's Focus Lost event.
				//IsPaused = true;
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
				if (_worldManager.Player.CanReach((int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y))
				{
					_tessellator.Begin(PrimitiveType.Quads);
					_tessellator.LoadIdentity();
					_tessellator.Translate(0, 0, -9); // map overlay render layer

					// Render the tile selector.
					_tessellator.BindTexture(null);
					_tessellator.BindColor(Color.FromArgb(64, Color.Black));

					_tessellator.Translate(_mouseBlockPosition.X, _mouseBlockPosition.Y);
					_tessellator.AddPoint(0, 0);
					_tessellator.AddPoint(0, 1);
					_tessellator.AddPoint(1, 1);
					_tessellator.AddPoint(1, 0);

					if (_uiManager.SelectedToolbarSlot != null)
					{
						_tessellator.BindColor(Color.FromArgb(128, Color.White));
						ItemRenderManager.Instance.Render(_tessellator, _uiManager.SelectedToolbarSlot.GetItem());
					}

					_tessellator.End();
				}
			}

			_uiManager.Render();
        }

		private void Inspect(int blockX, int blockY)
		{
			var layer = _worldManager.Level.GetHighestVisibleLayer(blockX, blockY);
			var blockId = _worldManager.Level[layer, blockX, blockY];
			if (blockId > 0)
			{
				var block = BlockRegistry.Instance.GetById(blockId);
				Console.WriteLine($"You found a {block.Name} on the {layer.GetDescription()} layer: {block.Description}");
			}
		}

		protected override void OnKeyboardKeyDown(KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				switch (e.Key)
				{
					case Key.I:
						Console.WriteLine("Show some inventory.");
						break;
					case Key.W:
						_worldManager.Player.Orientation = Direction.North;
						_worldManager.Player.IsMoving = true;
						break;
					case Key.S:
						_worldManager.Player.Orientation = Direction.South;
						_worldManager.Player.IsMoving = true;
						break;
					case Key.A:
						_worldManager.Player.Orientation = Direction.West;
						_worldManager.Player.IsMoving = true;
						break;
					case Key.D:
						_worldManager.Player.Orientation = Direction.East;
						_worldManager.Player.IsMoving = true;
						break;
				}
			}
		}

		protected override void OnKeyboardKeyUp(KeyboardKeyEventArgs e)
		{
			if (HasFocus)
			{
				switch (e.Key)
				{
					case Key.Escape:
						EnterState(new PauseState(Manager));
						break;
					case Key.I:
						Console.WriteLine("Show some inventory.");
						break;
					case Key.W:
					case Key.S:
					case Key.A:
					case Key.D:
						_worldManager.Player.IsMoving = false;
						break;
				}
			}
		}

		protected override void OnMouseButtonDown(MouseButtonEventArgs e)
		{
			if (HasFocus && !_uiManager.HasMouseHover)
			{
				if (e.Button == MouseButton.Left)
				{
					// Only interact with blocks that are within reach.
					if (_worldManager.Player.CanReach((int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y)) // the reach should be partially dependent on the tool used
					{
						// TODO: Level.Entities should only return the entities for the active chunk.  The active chunk will need to be tracked somehow.
						var hoverEntity = _worldManager.Level.Entities.FirstOrDefault(x => x.ContainsPoint(_mouseBlockPosition));
						if ((hoverEntity == null) || (hoverEntity == _worldManager.Player)) // don't let the player hit himself
						{
							if (_uiManager.SelectedToolbarSlot != null)
							{
								_uiManager.SelectedToolbarSlot.Use(_worldManager.Level, _worldManager.Level.GetHighestVisibleLayer((int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y), (int)_mouseBlockPosition.X, (int)_mouseBlockPosition.Y);
							}
						}
						else
						{
							// TODO: First try to use the active item on hoverEntity.  Save the Touched event for impacts.
							hoverEntity.Touched(_worldManager.Player);
							Console.WriteLine(hoverEntity.ToString());
						}
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

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			if (HasFocus)
			{
				_mouseBlockPosition = _worldManager.Camera.UnProject(e.X, e.Y);

				// The world manager forces the camera to center on the player, so this does nothing.
				//if (e.Mouse.IsButtonDown(MouseButton.Middle))
				//{
				//	var delta = (_cameraMoveStart - _mouseBlockPosition);
				//	_worldManager.Camera.MoveBy(delta);
				//	_cameraMoveStart = _mouseBlockPosition;
				//}

				_mouseBlockPosition.X = (float)Math.Floor(_mouseBlockPosition.X);
				_mouseBlockPosition.Y = (float)Math.Floor(_mouseBlockPosition.Y);
			}
		}

		protected override void OnMouseWheelChanged(MouseWheelEventArgs e)
		{
			_worldManager.Camera.Projection.OrthographicSize = (float)Math.Ceiling(_worldManager.Camera.Projection.OrthographicSize - _worldManager.Camera.Projection.OrthographicSize * (e.DeltaPrecise / 10));
			_worldManager.Camera.Projection.OrthographicSize = (float)Math.Floor(CommonCore.Math.MathHelper.Clamp(_worldManager.Camera.Projection.OrthographicSize, ZOOM_MIN, ZOOM_MAX));
		}

		#endregion
	}
}

