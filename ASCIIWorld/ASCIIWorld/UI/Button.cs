using GameCore.Rendering;
using GameCore;
using GameCore.IO;
using System.Drawing;
using OpenTK;
using System;

namespace ASCIIWorld.UI
{
	public class Button : UIElement
	{
		#region Constants

		// TODO: Create a style configuration manager.

		private static readonly Color COLOR_BORDER = Color.Blue;
		private static readonly Color COLOR_BORDER_HOVER = Color.LightBlue;
		private static readonly Color COLOR_BORDER_CLICKED = Color.DarkBlue;

		private static readonly Color COLOR_TEXT = Color.Yellow;
		private static readonly Color COLOR_TEXT_CLICKED = Color.Brown;

		#endregion

		#region Fields

		private Border _border;
		private Label _label;

		#endregion

		#region Constructors

		public Button(Camera<OrthographicProjection> camera, Vector2 position, string text)
			: base(camera, position)
		{
			Text = text;
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		public override bool HasMouseHover
		{
			get
			{
				return base.HasMouseHover || _label.HasMouseHover || _border.HasMouseHover;
			}
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);
			Bounds = new RectangleF(Bounds.X, Bounds.Y, ASCII.Width * (Text.Length + 2), ASCII.Height * 3);

			_border = new Border(Camera, Vector2.Zero, Text.Length + 2, 3);
			_border.LoadContent(content);

			_label = new Label(Camera, new Vector2(ASCII.Width, ASCII.Height), Text);
			_label.LoadContent(content);
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			// TODO: Calculate the text and border colors based on the HSL values of the default color.

			_border.BorderColor = GetBorderColor();
			_border.Render(tessellator);

			_label.TextColor = ClickStarted ? COLOR_TEXT_CLICKED : COLOR_TEXT;
			_label.Render(tessellator);
		}

		private Color GetBorderColor()
		{
			if (HasMouseHover)
			{
				if (ClickStarted)
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

		#endregion
	}
}
