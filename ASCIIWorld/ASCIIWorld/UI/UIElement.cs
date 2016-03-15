using ASCIIWorld.Rendering;
using System;
using GameCore.Rendering;
using GameCore;
using GameCore.IO;
using OpenTK.Input;
using System.Drawing;
using OpenTK;

namespace ASCIIWorld.UI
{
	public abstract class UIElement : IUpdateable, IRenderable, IDisposable
	{
		#region Constants
		
		private const int ASCII_SOLID = 219;

		#endregion

		#region Events

		public event EventHandler Clicked;

		#endregion

		#region Fields

		#endregion

		#region Constructors

		public UIElement(Camera<OrthographicProjection> camera, Vector2 position)
		{
			ClickStarted = false;

			Camera = camera;
			HasMouseHover = false;

			Bounds = new RectangleF(position.X, position.Y, 0, 0);

			InputManager.Instance.Mouse.Move += Mouse_Move;
			InputManager.Instance.Mouse.ButtonDown += Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp += Mouse_ButtonUp;
		}

		#endregion

		#region Properties

		public RectangleF Bounds { get; protected set; }

		public virtual bool HasMouseHover { get; private set; }

		protected Camera<OrthographicProjection> Camera { get; private set; }

		protected TileSet ASCII { get; private set; }

		protected bool ClickStarted { get; private set; }

		#endregion

		#region Methods

		public virtual void LoadContent(ContentManager content)
		{
			ASCII = content.Load<TileSet>("TileSets/UI-ASCII.xml");
			ASCII.IsNormalized = false;
		}

		public virtual void Dispose()
		{
			InputManager.Instance.Mouse.Move -= Mouse_Move;
			InputManager.Instance.Mouse.ButtonDown -= Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp -= Mouse_ButtonUp;
		}

		public void Render(ITessellator tessellator)
		{
			tessellator.PushTransform();
			tessellator.Translate(Bounds.X, Bounds.Y);

			RenderContent(tessellator);

			tessellator.PopTransform();
		}

		public virtual void Update(TimeSpan elapsed)
		{
		}

		protected abstract void RenderContent(ITessellator tessellator);

		private void UpdateMouseStatus(MouseEventArgs e)
		{
			var mousePosition = Camera.UnProject(e.X, e.Y);
			HasMouseHover = Bounds.Contains(mousePosition.X, mousePosition.Y);
		}

		/// <summary>
		/// Render a solid rectangle.
		/// </summary>
		protected void RenderRectangle(ITessellator tessellator, Color color, int tileWidth, int tileHeight)
		{
			tessellator.PushTransform();
			tessellator.BindColor(color);
			for (var y = 0; y < tileHeight; y++)
			{
				for (var x = 0; x < tileWidth; x++)
				{
					ASCII.Render(tessellator, ASCII_SOLID);
					tessellator.Translate(ASCII.Width, 0);
				}
				tessellator.Translate(-ASCII.Width * tileWidth, ASCII.Height);
			}
			tessellator.PopTransform();
		}

		#endregion

		#region Event Handlers

		private void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
		{
			UpdateMouseStatus(e);
			if (HasMouseHover && (e.Button == MouseButton.Left) && ClickStarted)
			{
				ClickStarted = false;
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
				ClickStarted = true;
			}
		}

		private void Mouse_Move(object sender, MouseMoveEventArgs e)
		{
			UpdateMouseStatus(e);
		}

		#endregion
	}
}
