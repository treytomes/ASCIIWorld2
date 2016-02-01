using GameCore.Rendering;
using OpenTK;
using System.Drawing;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// The most basic rendering unit; this class references a single tile index, associating it with a color.
	/// </summary>
	/// <remarks>
	/// A Frame is composed of a stack of Tiles.
	/// </remarks>
	public class Tile : IRenderable
	{
		#region Fields

		private TileSet _tileSet;

		#endregion

		#region Constructors

		public Tile(TileSet tileSet, Color color, int tileIndex)
		{
			_tileSet = tileSet;
			Color = color;
			TileIndex = tileIndex;
		}

		#endregion
		
		#region Properties

		public Color Color { get; private set; }

		public int TileIndex { get; private set; }

		#endregion

		#region Methods

		public void Render(ITessellator tessellator)
		{
			tessellator.BindColor(Color);
			_tileSet.Render(tessellator, TileIndex);
		}

		public void Render(ITessellator tessellator, float x, float y)
		{
			var position = tessellator.Transform(new Vector2(x, y));
			tessellator.Translate(position);
			Render(tessellator);
			tessellator.Translate(-position);
		}

		#endregion
	}
}
