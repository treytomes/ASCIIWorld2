using GameCore.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A graphic composed of a stack of rendered Tiles.
	/// </summary>
	public class TileStack : IRenderable
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

		public void Render(ITessellator tessellator)
		{
			foreach (var layer in _layers)
			{
				layer.Render(tessellator);
			}
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
