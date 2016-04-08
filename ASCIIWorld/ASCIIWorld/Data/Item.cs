using CommonCore;

namespace ASCIIWorld.Data
{
	public abstract class Item : IRegisteredObject
	{
		#region Fields

		#endregion

		#region Constructors

		public Item(string name)
		{
			Name = name;
		}

		#endregion

		#region Properties

		/// <summary>
		/// This will be assigned by the ItemRegistry.
		/// </summary>
		public int Id { get; set; }

		public string Name { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Use this item on the selected location.
		/// </summary>
		public virtual void Use(Level level, ChunkLayer layer, int blockX, int blockY, out bool isConsumed)
		{
			isConsumed = false;
		}

		/// <summary>
		/// Use this item on the specified entity.
		/// </summary>
		public virtual void Use(Entity target, out bool isConsumed)
		{
			isConsumed = false;
		}

		#endregion
	}
}
