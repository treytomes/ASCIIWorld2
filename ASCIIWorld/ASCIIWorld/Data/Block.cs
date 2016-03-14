using ASCIIWorld.Rendering;
using CommonCore;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Many Blocks make a Chunk.
	/// </summary>
	public class Block : IBlockAccess
	{
		#region Fields

		private Dictionary<string, object> _properties;

		#endregion

		#region Constructors

		public Block(string name, bool isOpaque, IBlockRenderer renderer)
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

		public bool IsOpaque { get; private set; }

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

		#endregion
	}
}
