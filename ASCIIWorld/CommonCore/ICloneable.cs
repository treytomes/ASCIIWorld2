using System;

namespace CommonCore
{
	public interface ICloneable<T> : ICloneable
	{
		new T Clone();
	}
}
