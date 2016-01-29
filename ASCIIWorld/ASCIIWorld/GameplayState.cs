using ASCIIWorld.Data;
using GameCore.IO;
using GameCore.Rendering;
using GameCore.Rendering.Text;
using GameCore.StateManagement;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;

namespace ASCIIWorld
{
	public class GameplayState : GameState
	{
		#region Fields

		private Texture2D _texture;
		private GLTextWriter _writer;
		private TileSet _tiles;

		private Tile _grassTile;
		private Tile _waterTile;

		#endregion

		#region Constructors

		public GameplayState(GameStateManager manager)
			: base(manager)
		{
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
					if (IsPaused && !value)
					{
						// Un-pause the game.
						Manager.LeaveState();
					}
					else
					{
						Manager.EnterState(new PauseState(Manager));
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
			_texture = content.Load<Texture2D>("Textures/OEM437_8.png");
			_tiles = content.Load<TileSet>("TileSets/ASCII.xml");

			// TODO: Test an animated frame.
			//_grassTile = new Tile(1, new TileFrame(
			//	new TileLayer(_tiles, Color.FromArgb(0xFF, 0x28, 0xB5, 0x15), 219),
			//	new TileLayer(_tiles, Color.FromArgb(0xFF, 0x68, 0xD3, 0x61), 176)));

			_grassTile = content.Load<Tile>("Tiles/Grass.xml");
			_waterTile = content.Load<Tile>("Tiles/Water.xml");
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			if (HasFocus)
			{
				if (Keyboard.GetState().IsKeyDown(Key.Escape))
				{
					LeaveState();
				}
				else if (Keyboard.GetState().IsKeyDown(Key.P))
				{
					EnterState(new PauseState(Manager));
				}

				_grassTile.Update(elapsed);
				_waterTile.Update(elapsed);
			}
			else
			{
				IsPaused = true;
			}
		}

		public override void Render()
		{
			base.Render();

			var tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };

			tessellator.Begin(PrimitiveType.Quads);
			tessellator.Scale(1, 1);
			tessellator.BindColor(Color.White);
			tessellator.BindTexture(_texture);
			tessellator.AddPoint(0, 0, 0, 0);
			tessellator.AddPoint(0, _texture.Height, 0, 1);
			tessellator.AddPoint(_texture.Width, _texture.Height, 1, 1);
			tessellator.AddPoint(_texture.Width, 0, 1, 0);
			tessellator.End();

			tessellator.Begin(PrimitiveType.Quads);
			tessellator.Scale(4, 4);
			tessellator.Translate(400, 100);
			tessellator.BindColor(Color.Red);
			_tiles.Render(tessellator, 3);
			tessellator.End();

			tessellator.Begin(PrimitiveType.Quads);
			tessellator.LoadIdentity();
			tessellator.Scale(4, 4);
			_grassTile.Render(tessellator, 400, 200);
			_waterTile.Render(tessellator, 400 + 8 * 4, 200);
			tessellator.End();

			_writer.Color = Color.Thistle;
			_writer.Position = new Vector2(256, 256);
			_writer.Write("Hello, world!");
		}

		#endregion
	}
}

