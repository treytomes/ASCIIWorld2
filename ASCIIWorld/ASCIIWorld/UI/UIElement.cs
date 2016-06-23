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
	public abstract class UIElement : InputReceiver, IUpdateable, IRenderable, IDisposable
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

		public UIElement(IGameWindow window, Camera<OrthographicProjection> camera, Vector2 position)
			: base(window)
		{
			ClickStarted = false;

			Camera = camera;
			HasMouseHover = false;
			CanHaveMouseHover = true;

			Bounds = new RectangleF(position.X, position.Y, 0, 0);
		}

		#endregion

		#region Properties

		public RectangleF Bounds { get; protected set; }

		public virtual bool HasMouseHover { get; private set; }

		public bool CanHaveMouseHover { get; set; }

		protected Camera<OrthographicProjection> Camera { get; private set; }

		protected TileSet UI_ASCII { get; private set; }

		protected bool ClickStarted { get; private set; }

		#endregion

		#region Methods

		public void MoveTo(Vector2 position)
		{
			Bounds = new RectangleF(position.X, position.Y, Bounds.Width, Bounds.Height);
		}

		public virtual void LoadContent(ContentManager content)
		{
			UI_ASCII = content.Load<TileSet>("TileSets/UI-ASCII.xml");
			UI_ASCII.IsNormalized = false;
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
			if (CanHaveMouseHover)
			{
				var mousePosition = Camera.UnProject(e.X, e.Y);
				HasMouseHover = Bounds.Contains(mousePosition.X, mousePosition.Y);
			}
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
					UI_ASCII.Render(tessellator, ASCII_SOLID);
					tessellator.Translate(UI_ASCII.Width, 0);
				}
				tessellator.Translate(-UI_ASCII.Width * tileWidth, UI_ASCII.Height);
			}
			tessellator.PopTransform();
		}

		protected virtual Color ModifyColorByState(Color color)
		{
			var hue = color.GetHue();
			var saturation = color.GetSaturation();
			var brightness = color.GetBrightness();

			if (ClickStarted)
			{
				brightness *= 0.5f;
			}
			if (HasMouseHover)
			{
				brightness *= 1.5f;
			}

			return ColorHelper.FromAHSB(color.A, hue, saturation, brightness);
		}

		/// <summary>
		/// Calculate a scale according to the current position.
		/// </summary>
		/// <remarks>
		/// This assumes you are inside of RenderContent, and handles pushing and popping the current transformation accordingly.
		/// </remarks>
		protected void Scale(ITessellator tessellator, float scale)
		{
			var position = tessellator.Transform(Vector2.Zero);
			tessellator.PopTransform();
			tessellator.PushTransform();
			tessellator.Scale(scale, scale);
			tessellator.Translate(position);
		}

		protected void RaiseClickedEvent()
		{
			if (Clicked != null)
			{
				Clicked(this, EventArgs.Empty);
			}
		}

		protected override void OnMouseButtonUp(MouseButtonEventArgs e)
		{
			UpdateMouseStatus(e);
			if (HasMouseHover && (e.Button == MouseButton.Left) && ClickStarted)
			{
				if (Clicked != null)
				{
					Clicked(this, EventArgs.Empty);
				}
			}
			ClickStarted = false;
		}

		protected override void OnMouseButtonDown(MouseButtonEventArgs e)
		{
			UpdateMouseStatus(e);
			if (HasMouseHover && (e.Button == MouseButton.Left))
			{
				ClickStarted = true;
			}
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			UpdateMouseStatus(e);
		}

		#endregion
	}
}
