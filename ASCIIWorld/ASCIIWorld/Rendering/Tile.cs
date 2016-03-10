using ASCIIWorld.Data;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using System;
using System.Drawing;

namespace ASCIIWorld.Rendering
{
	/// <summary>
	/// The most basic rendering unit; this class references a single tile index, associating it with a color.
	/// </summary>
	/// <remarks>
	/// A Frame is composed of a stack of Tiles.
	/// </remarks>
	public class Tile : IBlockRenderer
	{
		#region Fields

		private static int _numChunkLayers;
		private TileSet _tileSet;

		#endregion

		#region Constructors

		static Tile()
		{
			_numChunkLayers = Enum.GetValues(typeof(ChunkLayer)).Length;
		}

		public Tile(TileSet tileSet, Color color, string name)
		{
			_tileSet = tileSet;
			Color = color;
			Name = name;
			TileIndex = tileSet.GetTileIndexFromName(Name);
		}

		public Tile(TileSet tileSet, Color color, int tileIndex)
		{
			_tileSet = tileSet;
			Color = color;
			TileIndex = tileIndex;
			Name = tileSet.GetNameFromTileIndex(TileIndex);
		}

		#endregion

		#region Properties

		public Color Color { get; private set; }

		public string Name { get; private set; }

		public int TileIndex { get; private set; }

		#endregion

		#region Methods

		public void Update(TimeSpan elapsed)
		{
		}

		public void Render(ITessellator tessellator)
		{
			// This will cause tile images to become darker as they move into the background layers.
			var layer =  -tessellator.WorldToScreenPoint(Vector3.Zero).Z;
			var multiplier = 1.0 - (_numChunkLayers - layer) / (_numChunkLayers * 2) + 0.25f;
			var color = Color.FromArgb((int)(Color.R * multiplier), (int)(Color.G * multiplier), (int)(Color.B * multiplier));

			tessellator.BindColor(color);
			_tileSet.Render(tessellator, TileIndex);
		}

		public void Render(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int x, int y)
		{
			Render(tessellator);
		}

		#endregion
	}
}
