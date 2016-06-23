using GameCore;
using GameCore.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.IO;

namespace ASCIIWorld.UI
{
	public class BaseButton : UIElement
	{
		#region Fields

		private Border _border;

		#endregion

		#region Constructors

		public BaseButton(IGameWindow window, Camera<OrthographicProjection> camera, Vector2 position)
			: base(window, camera, position)
		{
		}

		#endregion

		#region Properties

		public Color BorderColor { get; set; }

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_border = new Border(Window, Camera, Vector2.Zero, 0, 0);
			_border.LoadContent(content);
			_border.CanHaveMouseHover = false;
			BorderColor = _border.BorderColor;
		}

		protected void SetBorderSize(int tileWidth, int tileHeight)
		{
			_border.Resize(tileWidth, tileHeight);
		}

		protected override void RenderContent(ITessellator tessellator)
		{
			_border.BorderColor = ModifyColorByState(BorderColor);
			_border.Render(tessellator);
		}

		#endregion
	}
}
