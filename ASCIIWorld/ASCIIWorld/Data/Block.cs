using System;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Many Blocks make a Chunk.
	/// </summary>
	public class Block : Animation
	{
		#region Fields

		private Dictionary<string, object> _properties;

		#endregion

		#region Constructors

		public Block(int framesPerSecond, IEnumerable<IRenderable> frames)
			: base(framesPerSecond, frames)
		{
			_properties = new Dictionary<string, object>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// This will be assigned by the BlockRegistry.
		/// </summary>
		public int Id { get; set; }

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

		#endregion
	}
}
