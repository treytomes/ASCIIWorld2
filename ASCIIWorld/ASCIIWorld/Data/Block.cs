using ASCIIWorld.Rendering;
using System;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Many Blocks make a Chunk.
	/// </summary>
	public class Block
	{
		#region Fields

		private Dictionary<string, object> _properties;

		#endregion

		#region Constructors

		public Block(IBlockRenderer renderer)
		{
			// TODO: A Block should contain a set of animations, where the animation can be chosen according to the block's world state.
			_properties = new Dictionary<string, object>();

			Renderer = renderer;
		}

		#endregion

		#region Properties

		/// <summary>
		/// This will be assigned by the BlockRegistry.
		/// </summary>
		public int Id { get; set; }

		public IBlockRenderer Renderer { get; private set; }

		#endregion

		#region Methods

		public bool HasProperty(string propertyName)
		{
			return _properties.ContainsKey(propertyName);
		}

		public T GetProperty<T>(string propertyName)
		{
			return (T)Convert.ChangeType(_properties[propertyName], typeof(T));
		}

		public void SetProperty<T>(string propertyName, T value)
		{
			_properties[propertyName] = value;
		}

		public Animation GetAnimation(Chunk chunk, ChunkLayer layer, int x, int y)
		{
			// TODO: Call this from ChunkRenderer.RenderLayer to get a reference to the animation that needs to be rendered.
			throw new NotImplementedException();
		}

		#endregion
	}
}
