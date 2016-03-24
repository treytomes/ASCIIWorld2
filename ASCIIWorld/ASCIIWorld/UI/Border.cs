using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using System.Drawing;

namespace ASCIIWorld.UI
{
	public class Border : UIElement
	{
		#region Constants

		private const int BACKGROUND_OPACITY = 255;
		private static readonly Color COLOR_BACKGROUND = Color.FromArgb(BACKGROUND_OPACITY, Color.Black);
		private static readonly Color COLOR_BORDER = Color.Blue;

		private const int ASCII_DOUBLELINE_TOPLEFT = 201;
		private const int ASCII_DOUBLELINE_TOPRIGHT = 187;
		private const int ASCII_DOUBLELINE_BOTTOMLEFT = 200;
		private const int ASCII_DOUBLELINE_BOTTOMRIGHT = 188;
		private const int ASCII_DOUBLELINE_HORIZONTAL = 205;
		private const int ASCII_DOUBLELINE_VERTICAL = 186;

		private const int ASCII_SINGLELINE_TOPLEFT = 218;
		private const int ASCII_SINGLELINE_TOPRIGHT = 191;
		private const int ASCII_SINGLELINE_BOTTOMLEFT = 192;
		private const int ASCII_SINGLELINE_BOTTOMRIGHT = 217;
		private const int ASCII_SINGLELINE_HORIZONTAL = 196;
		private const int ASCII_SINGLELINE_VERTICAL = 179;

		private const bool DOUBLELINED = false;

		#endregion

		#region Fields

		private int _tileWidth;
		private int _tileHeight;

		#endregion

		#region Constructors

		public Border(Camera<OrthographicProjection> camera, Vector2 position, int tileWidth, int tileHeight)
			: base(camera, position)
		{
			_tileWidth = tileWidth;
			_tileHeight = tileHeight;

			BackgroundColor = COLOR_BACKGROUND;
			BorderColor = COLOR_BORDER;
		}

		#endregion

		#region Properties

		public Color BorderColor { get; set; }

		public Color BackgroundColor { get; set; }

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			Bounds = new RectangleF(Bounds.X, Bounds.Y, ASCII.Width * _tileWidth, ASCII.Height * _tileHeight);
		}

		public void Resize(int tileWidth, int tileHeight)
		{
			_tileWidth = tileWidth;
			_tileHeight = tileHeight;
			Bounds = new RectangleF(Bounds.X, Bounds.Y, ASCII.Width * _tileWidth, ASCII.Height * _tileHeight);
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			RenderRectangle(tessellator, BackgroundColor, _tileWidth, _tileHeight);

			tessellator.BindColor(BorderColor);

			ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_TOPLEFT : ASCII_SINGLELINE_TOPLEFT);

			for (var n = 0; n < _tileWidth - 2; n++)
			{
				tessellator.Translate(ASCII.Width, 0);
				ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_HORIZONTAL : ASCII_SINGLELINE_HORIZONTAL); // top
			}

			tessellator.Translate(ASCII.Width, 0);
			ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_TOPRIGHT : ASCII_SINGLELINE_TOPRIGHT);

			tessellator.Translate(0, ASCII.Height * (_tileHeight - 1));
			ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_BOTTOMRIGHT : ASCII_SINGLELINE_BOTTOMRIGHT);

			for (var n = 0; n < _tileWidth - 2; n++)
			{
				tessellator.Translate(-ASCII.Width, 0);
				ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_HORIZONTAL : ASCII_SINGLELINE_HORIZONTAL); // bottom
			}

			tessellator.Translate(-ASCII.Width, 0);
			ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_BOTTOMLEFT : ASCII_SINGLELINE_BOTTOMLEFT);

			for (var n = 0; n < _tileHeight - 2; n++)
			{
				tessellator.Translate(0, -ASCII.Height);
				ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_VERTICAL : ASCII_SINGLELINE_VERTICAL); // left
			}

			tessellator.Translate(ASCII.Width * (_tileWidth - 1), 0);
			for (var n = 0; n < _tileHeight - 2; n++)
			{
				ASCII.Render(tessellator, DOUBLELINED ? ASCII_DOUBLELINE_VERTICAL : ASCII_SINGLELINE_VERTICAL); // right
				tessellator.Translate(0, ASCII.Height);
			}
		}

		#endregion
	}
}
