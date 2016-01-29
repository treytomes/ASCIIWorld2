using GameCore.Rendering;
using System.Drawing;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// The most basic rendering unit.
	/// </summary>
	/// <remarks>
	/// A TileFrame is composed of multiple TileLayers.
	/// </remarks>
	public class TileLayer
	{
		#region Fields

		private TileSet _tileSet;

		#endregion

		#region Constructors

		public TileLayer(TileSet tileSet, Color color, int tileIndex)
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

		public void Render(ITessellator tessellator, int x, int y)
		{
			tessellator.Translate(x, y);
			Render(tessellator);
			tessellator.Translate(-x, -y);
		}

		#endregion
	}
}
