using System;

namespace ASCIIWorld.Data
{
	[Serializable]
	public class PlayerEntity : Entity
	{
		#region Constants

		private const int INVENTORY_SIZE = 10 * 4;
		private const int TOOLBELT_SIZE = 10;

		#endregion

		#region Constructors

		public PlayerEntity()
		{
			Inventory = new InventoryContainer(INVENTORY_SIZE);
			Toolbelt = new InventoryContainer(TOOLBELT_SIZE);

			Speed = 0.1f;
			Size = 0.8f;
		}

		#endregion

		#region Properties

		public InventoryContainer Inventory { get; private set; }

		public InventoryContainer Toolbelt { get; private set; }

		#endregion

		#region Methods

		public override void Update(Level level, TimeSpan elapsed)
		{
			base.Update(level, elapsed);

			// Clear out empty item stacks.
			UpdateItems(Inventory);
			UpdateItems(Toolbelt);
		}

		public void ReceiveItem(Item item)
		{
			var itemStack = new ItemStack(item.Id);

			if (Toolbelt.HasCompatibleSlot(itemStack))
			{
				Toolbelt.SetFirstCompatibleSlot(itemStack);
			}
			else if (Inventory.HasCompatibleSlot(itemStack))
			{
				Inventory.SetFirstCompatibleSlot(itemStack);
			}
			else
			{
				throw new InvalidOperationException("The inventories are all full!");
			}
		}

		private void UpdateItems(InventoryContainer container)
		{
			for (int slotIndex = 0; slotIndex < container.Count; slotIndex++)
			{
				var itemStack = container.GetSlot(slotIndex);
				if ((itemStack != null) && (itemStack.StackSize == 0))
				{
					container.SetSlot(slotIndex, null);
				}
			}
		}

		#endregion
	}
}
