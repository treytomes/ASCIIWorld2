using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonCore
{
	public class ObjectRegistry<TObject> : IObjectRegistryAccess, IEnumerable<TObject>
		where TObject : IRegisteredObject
	{
		#region Fields

		private Dictionary<int, TObject> _objects;
		private Dictionary<string, int> _nameIndex;

		#endregion

		#region Constructors

		public ObjectRegistry()
		{
			_objects = new Dictionary<int, TObject>();
			_nameIndex = new Dictionary<string, int>();
		}

		#endregion

		#region Methods

		public void RegisterBlock(int id, TObject @object)
		{
			if (_objects.ContainsKey(id))
			{
				throw new Exception("The object id has already been defined.");
			}
			@object.Id = id;
			_objects[id] = @object;
			_nameIndex[@object.Name] = id;
		}

		public TObject GetById(int blockId)
		{
			return _objects[blockId];
		}

		public TObject GetByName(string name)
		{
			return _objects[_nameIndex[name]];
		}

		public string GetName(int id)
		{
			return _objects[id].Name;
		}

		public int GetId(string name)
		{
			return _nameIndex[name];
		}

		public bool IsDefined(int id)
		{
			return _objects.ContainsKey(id);
		}

		public bool IsDefined(string name)
		{
			return _nameIndex.ContainsKey(name);
		}

		public IEnumerator<TObject> GetEnumerator()
		{
			foreach (var obj in _objects)
			{
				yield return obj.Value;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
