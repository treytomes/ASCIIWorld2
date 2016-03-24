using ASCIIWorld.Data;
using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Rendering
{
	/// <summary>
	/// A graphic composed of a stack of rendered Tiles.
	/// </summary>
	public class TileStack : IBlockRenderer
	{
		#region Fields

		private List<IRenderable> _layers;

		#endregion

		#region Constructors

		public TileStack(IEnumerable<IRenderable> layers)
		{
			if (layers.Count() == 0)
			{
				throw new ArgumentException("layers");
			}
			_layers = new List<IRenderable>(layers);
		}

		#endregion

		#region Properties

		public int Count
		{
			get
			{
				return _layers.Count;
			}
		}

		public IRenderable this[int index]
		{
			get
			{
				return _layers[index];
			}
		}

		#endregion

		#region Methods

		public void Update(TimeSpan elapsed)
		{
		}

		public void Render(ITessellator tessellator)
		{
			foreach (var layer in _layers)
			{
				layer.Render(tessellator);
			}
		}

		public virtual void Render(ITessellator tessellator, IChunkAccess chunk, ChunkLayer layer, int x, int y)
		{
			Render(tessellator);
		}

		#endregion
	}
}
