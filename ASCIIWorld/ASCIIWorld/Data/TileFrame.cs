using GameCore.Rendering;
using System;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A tile composed of a stack of rendered TileLayers.
	/// </summary>
	public class TileFrame
	{
		#region Fields

		private List<TileLayer> _layers;

		#endregion

		#region Constructors

		public TileFrame(params TileLayer[] layers)
		{
			if (layers.Length == 0)
			{
				throw new ArgumentException("layers");
			}
			_layers = new List<TileLayer>(layers);
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

		public TileLayer this[int index]
		{
			get
			{
				return _layers[index];
			}
		}

		#endregion

		#region Methods

		public void Render(ITessellator tessellator, int x, int y)
		{
			tessellator.Translate(x, y);

			foreach (var layer in _layers)
			{
				layer.Render(tessellator);
			}

			tessellator.Translate(-x, -y);
		}

		#endregion
	}
}
