using ASCIIWorld.Rendering;
using CommonCore;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace ASCIIWorld.UI
{
	public class IconButton : BaseButton
	{
		#region Fields

		private bool _isSelected;

		#endregion

		#region Constructors

		public IconButton(Camera<OrthographicProjection> camera, Vector2 position, IRenderable renderable, Key? hotkey = null)
			: base(camera, position)
		{
			Renderable = renderable;
			IsSelected = false;
			Hotkey = hotkey;
		}

		#endregion

		#region Properties

		protected TileSet ASCII { get; private set; }

		public virtual IRenderable Renderable { get; protected set; }

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

		public Key? Hotkey { get; private set; }

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);
			Bounds = new RectangleF(Bounds.X, Bounds.Y, UI_ASCII.Width * 5, UI_ASCII.Height * 5);
			SetBorderSize(5, 5);

			ASCII = content.Load<TileSet>("TileSets/ASCII.xml");

			InputManager.Instance.Keyboard.KeyDown += Keyboard_KeyDown;
		}

		public override void Dispose()
		{
			InputManager.Instance.Keyboard.KeyDown -= Keyboard_KeyDown;

			base.Dispose();
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			base.RenderContent(tessellator);

			// TODO: Item.Renderer.Color = ModifyColorByState(_itemColor);

			tessellator.Translate(UI_ASCII.Width * 1.5f, UI_ASCII.Height * 1.5f);
			Scale(tessellator, 8.0f * 2.0f);

			if (Renderable != null)
			{
				tessellator.BindColor(Color.White);
				Renderable.Render(tessellator);
			}

			if (Hotkey.HasValue)
			{
				tessellator.BindColor(Color.White);
				Scale(tessellator, 8f);
				tessellator.Translate(16, 16, -1);
				ASCII.RenderText(tessellator, ConvertEx.ChangeType<char>(Hotkey.Value).ToString());
				tessellator.Translate(-16, -16, 1);
			}
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

		#region Event Handlers

		private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (Hotkey.HasValue && (e.Key == Hotkey.Value))
			{
				RaiseClickedEvent();
			}
		}

		#endregion
	}

	public class IconButton<TRenderable> : IconButton
		where TRenderable : IRenderable
	{

		#region Constructors

		public IconButton(Camera<OrthographicProjection> camera, Vector2 position, TRenderable renderable, Key? hotkey = null)
			: base(camera, position, renderable, hotkey)
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
			protected set
			{
				base.Renderable = value;
			}
		}

		#endregion
	}
}
