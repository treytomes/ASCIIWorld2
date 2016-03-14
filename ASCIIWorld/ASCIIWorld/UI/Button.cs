using ASCIIWorld.Rendering;
using System;
using GameCore.Rendering;
using GameCore;
using GameCore.IO;
using System.Drawing;
using CommonCore.Math;
using OpenTK.Input;
using OpenTK;

namespace ASCIIWorld.UI
{
	public class Button : IUpdateable, IRenderable, IDisposable
	{
		#region Events

		public event EventHandler Clicked;

		#endregion

		#region Constants

		private static readonly Color COLOR_BACKGROUND = Color.Black;
		private static readonly Color COLOR_BORDER = Color.Blue;
		private static readonly Color COLOR_BORDER_HOVER = Color.LightBlue;
		private static readonly Color COLOR_BORDER_CLICKED = Color.DarkBlue;
		private static readonly Color COLOR_TEXT = Color.Yellow;
		private static readonly Color COLOR_TEXT_CLICKED = Color.Brown;

		private const int ASCII_DOUBLELINE_TOPLEFT = 201;
		private const int ASCII_DOUBLELINE_TOPRIGHT = 187;
		private const int ASCII_DOUBLELINE_BOTTOMLEFT = 200;
		private const int ASCII_DOUBLELINE_BOTTOMRIGHT = 188;
		private const int ASCII_DOUBLELINE_HORIZONTAL = 205;
		private const int ASCII_DOUBLELINE_VERTICAL = 186;

		private const int ASCII_SOLID = 219;

		#endregion

		#region Fields

		private Camera<OrthographicProjection> _hudCamera;
		private TileSet _ascii;
		private Vector2 _position;
		private bool _clickStarted;

		#endregion

		#region Constructors

		public Button(Camera<OrthographicProjection> hudCamera, Vector2 position, string text)
		{
			_hudCamera = hudCamera;
			_position = position;
			_clickStarted = false;

			Text = text;
			HasMouseHover = false;

			InputManager.Instance.Mouse.Move += Mouse_Move;
			InputManager.Instance.Mouse.ButtonDown += Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp += Mouse_ButtonUp;
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		public bool HasMouseHover { get; private set; }

		public RectangleF Bounds { get; private set; }

		#endregion

		#region Methods

		public void LoadContent(ContentManager content)
		{
			_ascii = content.Load<TileSet>("TileSets/ASCII.xml", false);
			_ascii.IsNormalized = false;

			Bounds = new RectangleF(_position.X, _position.Y, _ascii.Width * (Text.Length + 2), _ascii.Height * 3);
		}

		public void Update(TimeSpan elapsed)
		{
		}

		public void Render(ITessellator tessellator)
		{
			for (var y = 0; y < 3; y++)
			{
				for (var x = 0; x < Text.Length + 2; x++)
				{
					_ascii.Render(tessellator, ASCII_SOLID);
					tessellator.Translate(_ascii.Width, 0);
				}
				tessellator.Translate(-_ascii.Width * (Text.Length + 2), _ascii.Height);
			}
			tessellator.Translate(0, -_ascii.Height * 3);

			tessellator.BindColor(GetBorderColor());

			_ascii.Render(tessellator, ASCII_DOUBLELINE_TOPLEFT);

			for (var n = 0; n < Text.Length; n++)
			{
				tessellator.Translate(_ascii.Width, 0);
				_ascii.Render(tessellator, ASCII_DOUBLELINE_HORIZONTAL); // top
			}

			tessellator.Translate(_ascii.Width, 0);
			_ascii.Render(tessellator, ASCII_DOUBLELINE_TOPRIGHT);

			tessellator.Translate(0, _ascii.Height * 2);
			_ascii.Render(tessellator, ASCII_DOUBLELINE_BOTTOMRIGHT);

			for (var n = 0; n < Text.Length; n++)
			{
				tessellator.Translate(-_ascii.Width, 0);
				_ascii.Render(tessellator, ASCII_DOUBLELINE_HORIZONTAL); // bottom
			}

			tessellator.Translate(-_ascii.Width, 0);
			_ascii.Render(tessellator, ASCII_DOUBLELINE_BOTTOMLEFT);

			tessellator.Translate(0, -_ascii.Height);
			_ascii.Render(tessellator, ASCII_DOUBLELINE_VERTICAL); // left

			tessellator.Translate(_ascii.Width * (Text.Length + 1), 0);
			_ascii.Render(tessellator, ASCII_DOUBLELINE_VERTICAL); // right

			tessellator.BindColor(GetTextColor());
			tessellator.Translate(-_ascii.Width * (Text.Length), 0);
			_ascii.RenderText(tessellator, Text);
		}

		public void Dispose()
		{
			InputManager.Instance.Mouse.Move -= Mouse_Move;
			InputManager.Instance.Mouse.ButtonDown -= Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp -= Mouse_ButtonUp;
		}

		private void UpdateMouseStatus(MouseEventArgs e)
		{
			var mousePosition = _hudCamera.UnProject(e.X, e.Y);
			HasMouseHover = Bounds.Contains(mousePosition.X, mousePosition.Y);
		}

		private Color GetBorderColor()
		{
			if (HasMouseHover)
			{
				if (_clickStarted)
				{
					return COLOR_BORDER_CLICKED;
				}
				else
				{
					return COLOR_BORDER_HOVER;
				}
			}
			else
			{
				return COLOR_BORDER;
			}
		}

		private Color GetTextColor()
		{
			if (_clickStarted)
			{
				return COLOR_TEXT_CLICKED;
			}
			else
			{
				return COLOR_TEXT;	
			}
		}

		#endregion

		#region Event Handlers

		private void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
		{
			UpdateMouseStatus(e);
			if (HasMouseHover && (e.Button == MouseButton.Left) && _clickStarted)
			{
				_clickStarted = false;
				if (Clicked != null)
				{
					Clicked(this, EventArgs.Empty);
				}
			}
		}

		private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
		{
			UpdateMouseStatus(e);
			if (HasMouseHover && (e.Button == MouseButton.Left))
			{
				_clickStarted = true;
			}
		}

		private void Mouse_Move(object sender, MouseMoveEventArgs e)
		{
			UpdateMouseStatus(e);
		}

		#endregion 
	}
}
