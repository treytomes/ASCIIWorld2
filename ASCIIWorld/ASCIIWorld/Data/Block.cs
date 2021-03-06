﻿using ASCIIWorld.Rendering;
using CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Many Blocks make a Chunk.
	/// </summary>
	public class Block : IBlockAccess
	{
		#region Fields

		private Dictionary<string, object> _properties;
		private BlockBehavior[] _behaviors;

		#endregion

		#region Constructors

		public Block(string name, bool isOpaque, IBlockRenderer renderer, string description, IEnumerable<BlockBehavior> behaviors)
		{
			Name = name;
			Renderer = renderer;
			Description = description;

			_properties = new Dictionary<string, object>();
			_behaviors = behaviors.ToArray();
		}

		#endregion

		#region Properties

		/// <summary>
		/// This will be assigned by the BlockRegistry.
		/// </summary>
		public int Id { get; set; }

		public string Name { get; private set; }

		public string Description { get; private set; }

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

		public virtual void Update(TimeSpan elapsed, Level level, ChunkLayer layer, int blockX, int blockY)
		{
			// TODO: Blocks need behavioral components.  Best if written in Lua.

			foreach (var behavior in _behaviors)
			{
				behavior.Update(elapsed, level, layer, blockX, blockY);
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(Description))
			{
				return $"{Name}: {Description}";
			}
			else
			{
				return $"{Name}";
			}
		}

		#endregion
	}
}
