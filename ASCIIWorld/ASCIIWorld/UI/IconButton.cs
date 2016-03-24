using ASCIIWorld.Rendering;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using System.Drawing;

namespace ASCIIWorld.UI
{
	public class IconButton : BaseButton
	{
		#region Fields

		private bool _isSelected;

		#endregion

		#region Constructors

		public IconButton(Camera<OrthographicProjection> camera, Vector2 position, IRenderable renderable)
			: base(camera, position)
		{
			Renderable = renderable;
			IsSelected = false;
		}

		#endregion

		#region Properties

		public IRenderable Renderable { get; private set; }

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				_isSelected = value;
				var color = BorderColor;
				if (_isSelected)
				{
					BorderColor = Color.FromArgb(255, BorderColor.G, BorderColor.B);
				}
				else
				{
					BorderColor = Color.FromArgb(0, BorderColor.G, BorderColor.B);
				}
			}
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);
			Bounds = new RectangleF(Bounds.X, Bounds.Y, ASCII.Width * 4, ASCII.Height * 4);
			SetBorderSize(4, 4);
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			base.RenderContent(tessellator);

			// TODO: Item.Renderer.Color = ModifyColorByState(_itemColor);
			tessellator.Translate(ASCII.Width, ASCII.Height);
			Scale(tessellator, 2.0f);
			Renderable.Render(tessellator);
		}

		protected override Color ModifyColorByState(Color color)
		{
			color = base.ModifyColorByState(color);

			var hue = color.GetHue();
			var saturation = color.GetSaturation();
			var brightness = color.GetBrightness();

			if (IsSelected)
			{
				hue = (hue + 60) % 360;
			}

			return ColorHelper.FromAHSB(color.A, hue, saturation, brightness);
		}

		#endregion
	}

	public class IconButton<TRenderable> : IconButton
		where TRenderable : IRenderable
	{

		#region Constructors

		public IconButton(Camera<OrthographicProjection> camera, Vector2 position, TRenderable renderable)
			: base(camera, position, renderable)
		{
		}

		#endregion

		#region Properties

		public new TRenderable Renderable
		{
			get
			{
				return (TRenderable)base.Renderable;
			}
		}

		#endregion
	}
}
