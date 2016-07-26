using ASCIIWorld.Data;
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

		public Tile(TileSet tileSet, int tileIndex)
		{
			_tileSet = tileSet;
			TileIndex = tileIndex;
			Name = tileSet.GetNameFromTileIndex(TileIndex);
		}

		public Tile(TileSet tileSet, string name)
		{
			_tileSet = tileSet;
			Name = name;
			TileIndex = tileSet.GetTileIndexFromName(Name);
		}

		#endregion

		#region Properties

		public string Name { get; private set; }

		public int TileIndex { get; private set; }

		public Transformer Transform { get; set; }

		#endregion

		#region Methods

		public void Update(TimeSpan elapsed)
		{
		}

		public void Render(ITessellator tessellator)
		{
			var color = tessellator.CurrentColor;

			// TODO: If this is an entity tile, then don't adjust the color...?

			// This will cause tile images to become darker as they move into the background layers.
			var layer =  -tessellator.Transform(Vector3.Zero).Z;
			if (layer < _numChunkLayers)
			{
				var multiplier = 1.0 - (_numChunkLayers - layer) / (_numChunkLayers * 2); // + 0.25f;
				tessellator.BindColor(Color.FromArgb((int)MathHelper.Clamp(color.R * multiplier, 0, 255), (int)MathHelper.Clamp(color.G * multiplier, 0, 255), (int)MathHelper.Clamp(color.B * multiplier, 0, 255)));
			}

			if (Transform != null)
			{
				tessellator.PushTransform();
				Transform.Apply(tessellator);

				_tileSet.Render(tessellator, TileIndex, Transform.MirrorX, Transform.MirrorY);
				tessellator.PopTransform();
			}
			else
			{
				_tileSet.Render(tessellator, TileIndex);
			}

			tessellator.BindColor(color);
		}

		public void Render(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int x, int y)
		{
			Render(tessellator);
		}

		#endregion
	}
}
