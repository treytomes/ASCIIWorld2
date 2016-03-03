using System;

namespace CommonCore
{
	public interface IPoolable : IDisposable
	{
		void OnAcquire();
		void OnRelease();
	}

	public interface IPoolable<T> : IPoolable
		where T : IPoolable<T>, new()
	{
		ObjectPool<T> Owner { get; set; }
	}
}