using System;

namespace ASCIIWorld.Generation
{
	public interface IGenerator<T>
	{
		T Generate(IProgress<string> progress);
	}
}
