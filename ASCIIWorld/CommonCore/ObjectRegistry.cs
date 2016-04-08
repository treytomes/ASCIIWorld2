using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CommonCore
{
	[DataContract]
	public class ObjectRegistry<TObject> : IEnumerable<TObject>
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

		#region Properties

		public int NextId
		{
			get
			{
				if (_objects.Count == 0)
				{
					return 1;
				}
				else
				{
					return _objects.Keys.Max() + 1;
				}
			}
		}

		#endregion

		#region Methods

		public Dictionary<int, string> ToDictionary()
		{
			// Reverse the name index so that we have a mapping of object id to object name.
			return _nameIndex.ToDictionary(x => x.Value, x => x.Key);
		}

		public void Register(TObject @object)
		{
			Register(NextId, @object);
		}

		public void Register(int id, TObject @object)
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
