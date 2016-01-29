using System.IO;

namespace GameCore.IO
{
	public interface IContentProvider<T>
	{
		T Load(ContentManager content, FileInfo contentPath);
	}
}
