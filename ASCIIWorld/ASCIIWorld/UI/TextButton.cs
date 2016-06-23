using GameCore.Rendering;
using GameCore;
using GameCore.IO;
using System.Drawing;
using OpenTK;

namespace ASCIIWorld.UI
{
	public class TextButton : BaseButton
	{
		#region Fields

		private Label _label;
		private Color _labelColor;

		#endregion

		#region Constructors

		public TextButton(IGameWindow window, Camera<OrthographicProjection> camera, Vector2 position, string text)
			: base(window, camera, position)
		{
			Text = text;
		}

		#endregion

		#region Properties

		public string Text { get; private set; }

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);
			Bounds = new RectangleF(Bounds.X, Bounds.Y, UI_ASCII.Width * (Text.Length + 2), UI_ASCII.Height * 3);
			SetBorderSize(Text.Length + 2, 3);

			_label = new Label(Window, Camera, new Vector2(UI_ASCII.Width, UI_ASCII.Height), Text);
			_label.LoadContent(content);
			_label.CanHaveMouseHover = false;
			_labelColor = _label.TextColor;
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			base.RenderContent(tessellator);

			_label.TextColor = ModifyColorByState(_labelColor);
			_label.Render(tessellator);
		}

		#endregion
	}
}
