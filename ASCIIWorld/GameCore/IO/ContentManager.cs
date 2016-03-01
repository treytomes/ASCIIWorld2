using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameCore.IO
{
	public class ContentManager
	{
		private struct ContentKey
		{
			public string Path;
			public Type Type;

			public ContentKey(string path, Type type)
			{
				Path = path;
				Type = type;
			}
		}

		#region Fields

		private DirectoryInfo _rootPath;
		private Dictionary<Type, object> _contentProviders;

		private Dictionary<ContentKey, object> _contentCache;

		#endregion

		#region Constructors

		public ContentManager(string rootPath)
		{
			_rootPath = new DirectoryInfo(rootPath);
			if (!_rootPath.Exists)
			{
				throw new ArgumentException("Path does not exist.", "rootPath");
			}

			_contentProviders = new Dictionary<Type, object>();
			RegisterContentProvider(new Texture2DContentProvider());
			RegisterContentProvider(new TileSetContentProvider());
			RegisterContentProvider(new XElementContentProvider());
			RegisterContentProvider(new BitmapContentProvider());
			RegisterContentProvider(new AtlasTileSetContentProvider());

			_contentCache = new Dictionary<ContentKey, object>();
		}

		#endregion

		#region Methods

		public void RegisterContentProvider<T>(IContentProvider<T> provider)
		{
			_contentProviders.Add(typeof(T), provider);
		}

		public T Load<T>(string contentPath, bool cached = true)
		{
			if (cached)
			{
				if (_contentCache.Keys.Any(key => (key.Path == contentPath) && (key.Type == typeof(T))))
				{
					return (T)_contentCache.First(kv => (kv.Key.Path == contentPath) && (kv.Key.Type == typeof(T))).Value;
				}
				else
				{
					T loadedContent = LoadContent<T>(contentPath);
					_contentCache[new ContentKey(contentPath, typeof(T))] = loadedContent;
					return loadedContent;
				}
			}
			else
			{
				return LoadContent<T>(contentPath);
			}
		}

		private T LoadContent<T>(string contentPath)
		{
			var fullContentPath = GetFullContentPath(contentPath);
			var provider = LocateContentProvider<T>();
			return provider.Load(this, fullContentPath);
		}

		private FileInfo GetFullContentPath(string contentPath)
		{
			var fullContentPath = new FileInfo(Path.Combine(_rootPath.FullName, contentPath));
			if (!fullContentPath.Exists)
			{
				throw new ArgumentException("Path does not exist.", "contentPath");
			}
			return fullContentPath;
		}

		private IContentProvider<T> LocateContentProvider<T>()
		{
			if (_contentProviders.ContainsKey(typeof(T)))
			{
				return (IContentProvider<T>)_contentProviders[typeof(T)];
			}
			throw new Exception("Could not find a provider for content type.");
		}

		#endregion
	}
}
