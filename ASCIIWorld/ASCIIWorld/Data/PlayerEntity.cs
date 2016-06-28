using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASCIIWorld.Data
{
	[Serializable]
	public class PlayerEntity : Entity
	{
		#region Constants

		private const int INVENTORY_SIZE = 10 * 4;
		private const int TOOLBELT_SIZE = 10;

		#endregion

		#region Fields

		private Guid _playerId;

		#endregion

		#region Constructors

		public PlayerEntity()
		{
			_playerId = Guid.NewGuid();

			Inventory = new InventoryContainer(INVENTORY_SIZE);
			Toolbelt = new InventoryContainer(TOOLBELT_SIZE);

			Speed = 0.1f;
			Size = 0.8f;
		}

		#endregion

		#region Properties

		public InventoryContainer Inventory { get; private set; }

		public InventoryContainer Toolbelt { get; private set; }

		private string Filename
		{
			get
			{
				return $"{_playerId.ToString()}.player";
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Re-load the given player.
		/// </summary>
		public static PlayerEntity Load(PlayerEntity player)
		{
			return Load(player.Filename);
		}

		public static PlayerEntity Load(string filename)
		{
			if (!File.Exists(filename))
			{
				Console.WriteLine($"{filename} does not exist.");
				return null;
			}

			var fileStream = new FileStream(filename, FileMode.Open);
			var formatter = new BinaryFormatter();
			try
			{
				var player = (PlayerEntity)formatter.Deserialize(fileStream);
				Console.WriteLine($"Player loaded from '{filename}'.");
				return player;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
			finally
			{
				fileStream.Close();
			}
		}

		public static void Save(PlayerEntity player)
		{
			var fileStream = new FileStream(player.Filename, FileMode.Create);
			var formatter = new BinaryFormatter();
			try
			{
				formatter.Serialize(fileStream, player);
				Console.WriteLine($"Player saved to '{player.Filename}'.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				fileStream.Close();
			}
		}

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
