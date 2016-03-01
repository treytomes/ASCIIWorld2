using System.Drawing;

namespace GameCore.Rendering
{
	public interface ITileContentSource
	{
		int Width { get; }
		int Height { get; }

		void Render(Graphics graphics, int x, int y);
	}
}