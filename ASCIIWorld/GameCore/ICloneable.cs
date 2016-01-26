using System;

namespace GameCore
{
	public interface ICloneable<T> : ICloneable
	{
		new T Clone();
	}
}
