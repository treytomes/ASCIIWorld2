namespace CommonCore
{
	public interface IObjectRegistryAccess
	{
		string GetName(int id);
		int GetId(string name);
		bool IsDefined(int id);
		bool IsDefined(string name);
	}
}