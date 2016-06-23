using ASCIIWorld.Data;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.UI
{
	public class UIManager
	{
		#region Fields

		private IGameWindow _window;
		
		private Camera<OrthographicProjection> _hudCamera;
		private ITessellator _tessellator;

		private List<UIElement> _children;

		private WorldManager _worldManager;

		#endregion

		#region Constructors

		public UIManager(IGameWindow window, Viewport viewport, WorldManager worldManager)
		{
			_window = window;

			_worldManager = worldManager;

			_children = new List<UIElement>();

			_hudCamera = Camera.CreateOrthographicCamera(viewport);
			_hudCamera.Projection.OrthographicSize = viewport.Height / 2;

			//_tessellator = new ImmediateModeTessellator();
			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };

			ToolbarItems = new List<ItemStackButton>();
		}

		#endregion

		#region Properties

		public bool HasMouseHover
		{
			get
			{
				return _children.Select(x => x.HasMouseHover).Where(x => x).DefaultIfEmpty(false).FirstOrDefault();
			}
		}

		// TODO: Need a better way of managing inventory.
		public List<ItemStackButton> ToolbarItems { get; private set; }

		public ItemStack SelectedToolbarSlot
		{
			get
			{
				return ToolbarItems.Where(x => x.IsSelected).DefaultIfEmpty(null).FirstOrDefault()?.ItemStack;
			}
		}

		public Label FPSLabel { get; private set; }

		public TextButton SaveButton { get; private set; }

		public TextButton LoadButton { get; private set; }

		#endregion

		#region Methods

		public void LoadContent(ContentManager content)
		{
			// TODO: Create a Pause menu for saving and loading.
			// TODO: If the UI has mouse hover, I don't want the game world to respond to it.
			SaveButton = new TextButton(_window, _hudCamera, new Vector2(100, 100), "Save");
			SaveButton.LoadContent(content);
			_children.Add(SaveButton);

			LoadButton = new TextButton(_window, _hudCamera, new Vector2(100, SaveButton.Bounds.Bottom), "Load");
			LoadButton.LoadContent(content);
			_children.Add(LoadButton);

			// TODO: Find a better was to manage user items.
			BuildItemToolbar(content);

			FPSLabel = new Label(_window, _hudCamera, new Vector2(256, 300), "FPS:");
			FPSLabel.LoadContent(content);
			_children.Add(FPSLabel);
		}

		// TODO: Move UnloadContent to Dispose.  Create a Disposable base class.
		public void UnloadContent()
		{
			foreach (var child in _children)
			{
				child.Dispose();
			}
			_children.Clear();
		}

		// TODO: This is only used in LoadContent.  Is there a better place for it?
		private void BuildItemToolbar(ContentManager content)
		{
			// TODO: Bind buttons directly to the inventory slot.
			AddToolbarItem(content, 0, Key.Number1);
			AddToolbarItem(content, 1, Key.Number2);
			AddToolbarItem(content, 2, Key.Number3);
			AddToolbarItem(content, 3, Key.Number4);
			AddToolbarItem(content, 4, Key.Number5);
			AddToolbarItem(content, 5, Key.Number6);
			AddToolbarItem(content, 6, Key.Number7);
			AddToolbarItem(content, 7, Key.Number8);
			AddToolbarItem(content, 8, Key.Number9);
			AddToolbarItem(content, 9, Key.Number0);

			// Center the toolbar at the bottom of the screen.
			var totalWidth = ToolbarItems.Count * ToolbarItems[0].Bounds.Width;
			var buttonX = -totalWidth / 2.0f;
			var height = ToolbarItems[0].Bounds.Height;
			for (var index = 0; index < ToolbarItems.Count; index++)
			{
				var button = ToolbarItems[index];
				button.MoveTo(new Vector2(buttonX, _hudCamera.Projection.Bottom - height));
				buttonX += button.Bounds.Width;
			}
		}

		// TODO: This is only used in BuildItemToolbar.  Is there a better place for it?
		private void AddToolbarItem(ContentManager content, int slotIndex, Key hotkey)
		{
			var itemButton = new InventorySlotButton(_window, _hudCamera, Vector2.Zero, _worldManager.Player.Toolbelt, slotIndex, hotkey);
			itemButton.LoadContent(content);
			itemButton.Clicked += ToolbarItemButton_Clicked;
			_children.Add(itemButton);
			ToolbarItems.Add(itemButton);
		}

		public void Resize(Viewport viewport)
		{
			_hudCamera.Resize(viewport);
		}

		public void Update(TimeSpan elapsed)
		{
			foreach (var child in _children)
			{
				child.Update(elapsed);
			}
		}

		public void Render()
		{
			_hudCamera.Apply();

			_tessellator.LoadIdentity();
			_tessellator.Translate(0, 0, -10); // hud render layer
			_tessellator.Begin(PrimitiveType.Quads);

			foreach (var child in _children)
			{
				child.Render(_tessellator);
			}

			_tessellator.End();
		}

		private void SelectToolbarItem(ItemStackButton itemButton)
		{
			foreach (var btn in ToolbarItems)
			{
				if (!btn.Equals(itemButton))
				{
					btn.IsSelected = false;
				}
			}
			itemButton.IsSelected = !itemButton.IsSelected;
		}

		#endregion

		#region Event Handlers

		private void ToolbarItemButton_Clicked(object sender, EventArgs e)
		{
			SelectToolbarItem(sender as ItemStackButton);
		}

		#endregion
	}
}
