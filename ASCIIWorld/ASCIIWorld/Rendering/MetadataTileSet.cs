using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASCIIWorld.Data;
using GameCore.Rendering;

namespace ASCIIWorld.Rendering
{
	/// <summary>
	/// This is used to display a different tile for each metadata key.
	/// </summary>
	/// <remarks>
	/// The metadata value will be logical-ANDed with the _mask value.
	/// </remarks>
	/// <seealso cref="ASCIIWorld.Rendering.IBlockRenderer" />
	public class MetadataTileSet : IBlockRenderer
	{
		#region Fields

		private int _mask;
		private Dictionary<int, IRenderable> _layers;
		private int _minKey;
		private int _maxKey;

		#endregion

		#region Constructors

		public MetadataTileSet(int mask, Dictionary<int, IRenderable> tiles)
		{
			_mask = mask;
			_layers = tiles;
			_minKey = _layers.Keys.Min();
			_maxKey = _layers.Keys.Max();
		}

		#endregion

		#region Methods

		public void Render(ITessellator tessellator)
		{
			_layers[_minKey].Render(tessellator);
		}

		public void Render(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int x, int y)
		{
			var metadata = chunk.GetMetadata(layer, x, y);
			if (_layers.ContainsKey(metadata))
			{
				_layers[metadata].Render(tessellator);
			}
			else
			{
				if (metadata > _maxKey)
				{
					_layers[_maxKey].Render(tessellator);
				}
				else
				{
					_layers[_minKey].Render(tessellator);
				}
			}
		}

		public void Update(TimeSpan elapsed)
		{
		}

		#endregion
	}
}
