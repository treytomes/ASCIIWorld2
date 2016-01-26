using GameCore.Rendering.Text;
using OpenTK;
using System.Drawing;

namespace GameCore.UI
{
	public interface ITextDescription
	{
		Font Font { get; }

		Color Color { get; }

		Vector2 Position { get; }

		TextPrinterOptions Options { get; }

		TextAlignment Alignment { get; }

		TextDirection Direction { get; }
	}
}
