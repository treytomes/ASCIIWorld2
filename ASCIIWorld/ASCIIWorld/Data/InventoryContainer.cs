using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data
{
	public class InventoryContainer : IEnumerable<ItemStack>
	{
		#region Fields

		private ItemStack[] _inventory;

		#endregion

		#region Constructors

		public InventoryContainer(int size)
		{
			_inventory = new ItemStack[size];
		}

		#endregion

		#region Properties

		public int Count
		{
			get
			{
				return _inventory.Length;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Get the item stack in the slot.  This may return null.
		/// </summary>
		public ItemStack GetSlot(int index)
		{
			return _inventory[index];
		}

		public void SetSlot(int index, ItemStack itemStack)
		{
			if (itemStack == null)
			{
				_inventory[index] = itemStack;
			}
			else
			{
				if (_inventory[index] != null)
				{
					throw new InvalidOperationException($"Slot {index} already contains an item.");
				}
				else
				{
					_inventory[index] = itemStack;
				}
			}
		}

		private bool HasEmptySlot()
		{
			return _inventory.Any(x => x == null);
		}

		private int FindEmptySlot()
		{
			for (var index = 0; index < _inventory.Length; index++)
			{
				if (_inventory[index] == null)
				{
					return index;
				}
			}
			throw new InvalidOperationException("The inventory is full.");
		}

		public bool HasCompatibleSlot(ItemStack itemStack)
		{
			for (var index = 0; index < _inventory.Length; index++)
			{
				if ((_inventory[index] != null) && _inventory[index].IsCompatibleWith(itemStack))
				{
					return true;
				}
			}
			return HasEmptySlot();
		}

		public int FindCompatibleSlot(ItemStack itemStack)
		{
			for (var index = 0; index < _inventory.Length; index++)
			{
				if ((_inventory[index] != null) && _inventory[index].IsCompatibleWith(itemStack))
				{
					return index;
				}
			}
			return FindEmptySlot();
		}

		/// <summary>
		/// Add the stack to the first compatible slot, or place it in the first empty slot.
		/// </summary>
		public void SetFirstCompatibleSlot(ItemStack itemStack)
		{
			var slotIndex = FindCompatibleSlot(itemStack);
			if (_inventory[slotIndex] == null)
			{
				_inventory[slotIndex] = itemStack;
			}
			else
			{
				_inventory[slotIndex].AddToStack(itemStack.StackSize);
			}
		}

		public IEnumerator<ItemStack> GetEnumerator()
		{
			foreach (var item in _inventory)
			{
				yield return item;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
