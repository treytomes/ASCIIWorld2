﻿using System;
using System.Collections.Generic;

namespace GameCore
{
	public class ObjectPool<T> where T : IPoolable<T>, new()
	{
		Queue<T> pool = new Queue<T>();

		public ObjectPool()
		{
		}

		public T Acquire()
		{
			T item;

			if (pool.Count > 0)
			{
				item = pool.Dequeue();
				item.OnAcquire();
			}
			else
			{
				item = new T();
				item.Owner = this;
				item.OnAcquire();
			}

			return item;
		}

		public void Release(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			item.OnRelease();
			pool.Enqueue(item);
		}
	}
}
