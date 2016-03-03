using ASCIIWorld.Rendering;
using CommonCore;
using GameCore;
using System;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Many Blocks make a Chunk.
	/// </summary>
	public class Block : IRegisteredObject
	{
		#region Fields

		private Dictionary<string, object> _properties;

		#endregion

		#region Constructors

		public Block(string name, IBlockRenderer renderer)
		{
			Name = name;
			Renderer = renderer;

			_properties = new Dictionary<string, object>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// This will be assigned by the BlockRegistry.
		/// </summary>
		public int Id { get; set; }

		public string Name { get; private set; }

		public IBlockRenderer Renderer { get; private set; }

		#endregion

		#region Methods

		public bool HasProperty(string propertyName)
		{
			return _properties.ContainsKey(propertyName);
		}

		public T GetProperty<T>(string propertyName)
		{
			return ConvertEx.ChangeType<T>(_properties[propertyName]);
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
