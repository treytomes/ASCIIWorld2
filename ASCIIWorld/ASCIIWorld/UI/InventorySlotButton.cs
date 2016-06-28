using ASCIIWorld.Data;
using GameCore;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Input;
using System;

namespace ASCIIWorld.UI
{
	public class InventorySlotButton : ItemStackButton
	{
		private Func<InventoryContainer> _inventory;
		private int _slotIndex;

		public InventorySlotButton(IGameWindow window, Camera<OrthographicProjection> camera, Vector2 position, Func<InventoryContainer> inventory, int slotIndex, Key? hotkey = null)
			: base(window, camera, position, null, hotkey)
		{
			_inventory = inventory;
			_slotIndex = slotIndex;
		}

		public override ItemStack ItemStack
		{
			get
			{
				return _inventory().GetSlot(_slotIndex);
			}
		}
	}
}