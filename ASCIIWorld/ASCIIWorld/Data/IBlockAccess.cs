using CommonCore;

namespace ASCIIWorld.Data
{
	public interface IBlockAccess : IRegisteredObject
	{
		bool IsOpaque { get; }

		bool HasProperty(string propertyName);
		T GetProperty<T>(string propertyName);
		void SetProperty<T>(string propertyName, T value);
	}
}
