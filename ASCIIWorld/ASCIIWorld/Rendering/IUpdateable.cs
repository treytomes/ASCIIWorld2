using System;

namespace ASCIIWorld.Rendering
{
	public interface IUpdateable
	{
		void Update(TimeSpan elapsed);
	}
}