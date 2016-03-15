using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using System.Drawing;
using System;

namespace ASCIIWorld.UI
{
	public class Label : UIElement
	{
		#region Constants

		// TODO: Create a style configuration manager.

		private static readonly Color COLOR_SHADOW = Color.Black;
		private static readonly Color COLOR_TEXT = Color.Yellow;

		private static bool SHADOWED = true;

		#endregion

		#region Fields

		#endregion

		#region Constructors

		public Label(Camera<OrthographicProjection> camera, Vector2 position, string text)
			: base(camera, position)
		{
			Text = text;
			TextColor = COLOR_TEXT;
			Shadowed = SHADOWED;
			ShadowColor = COLOR_SHADOW;
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		public Color TextColor { get; set; }

		public bool Shadowed { get; private set; }

		public Color ShadowColor { get; private set; }

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			Bounds = new RectangleF(Bounds.X, Bounds.Y, ASCII.Width * Text.Length, ASCII.Height);
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			if (Shadowed)
			{
				tessellator.Translate(1, 1);
				tessellator.BindColor(ShadowColor);
				ASCII.RenderText(tessellator, Text);
				tessellator.Translate(-1, -1);
			}

			tessellator.BindColor(TextColor);
			ASCII.RenderText(tessellator, Text);
		}

		#endregion
	}
}
