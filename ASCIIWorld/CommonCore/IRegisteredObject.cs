namespace CommonCore
{
	/// <summary>
	/// An object that can be stored in an ObjectRegistry.  The Id property will be assigned by the registry.
	/// </summary>
	public interface IRegisteredObject
	{
		int Id { get; set; }

		string Name { get; }
	}
}