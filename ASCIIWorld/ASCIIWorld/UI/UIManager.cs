using ASCIIWorld.Data;
using GameCore;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.UI
{
	public class UIManager
	{
		#region Fields
		
		private Camera<OrthographicProjection> _hudCamera;
		private ITessellator _tessellator;

		private List<UIElement> _children;

		#endregion

		#region Constructors

		public UIManager(Viewport viewport)
		{
			_children = new List<UIElement>();

			_hudCamera = Camera.CreateOrthographicCamera(viewport);
			_hudCamera.Projection.OrthographicSize = viewport.Height / 2;

			//_tessellator = new ImmediateModeTessellator();
			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };

			ToolbarItems = new List<ItemButton>();
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
		public List<ItemButton> ToolbarItems { get; private set; }

		public Item SelectedToolbarItem
		{
			get
			{
				return ToolbarItems.Where(x => x.IsSelected).DefaultIfEmpty(null).FirstOrDefault()?.Renderable;
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
			SaveButton = new TextButton(_hudCamera, new Vector2(100, 100), "Save");
			SaveButton.LoadContent(content);
			_children.Add(SaveButton);

			LoadButton = new TextButton(_hudCamera, new Vector2(100, SaveButton.Bounds.Bottom), "Load");
			LoadButton.LoadContent(content);
			_children.Add(LoadButton);

			// TODO: Find a better was to manage user items.
			var itemButton = new ItemButton(_hudCamera, Vector2.Zero, new PickaxeItem(content));
			itemButton.LoadContent(content);
			itemButton.Clicked += ToolbarItemButton_Clicked;
			_children.Add(itemButton);
			ToolbarItems.Add(itemButton);

			itemButton = new ItemButton(_hudCamera, Vector2.Zero, new HoeItem(content));
			itemButton.LoadContent(content);
			itemButton.Clicked += ToolbarItemButton_Clicked;
			_children.Add(itemButton);
			ToolbarItems.Add(itemButton);

			var totalWidth = ToolbarItems.Count * ToolbarItems[0].Bounds.Width;
			var buttonX = -totalWidth / 2.0f;
			var height = ToolbarItems[0].Bounds.Height;
			for (var index = 0; index < ToolbarItems.Count; index++)
			{
				var button = ToolbarItems[index];
				button.MoveTo(new Vector2(buttonX, _hudCamera.Projection.Bottom - height));
				buttonX += button.Bounds.Width;
			}

			FPSLabel = new Label(_hudCamera, new Vector2(256, 300), "FPS:");
			FPSLabel.LoadContent(content);
			_children.Add(FPSLabel);
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

		#endregion

		#region Event Handlers

		private void ToolbarItemButton_Clicked(object sender, EventArgs e)
		{
			foreach (var btn in ToolbarItems)
			{
				if (!btn.Equals(sender))
				{
					btn.IsSelected = false;
				}
			}
			(sender as ItemButton).IsSelected = !(sender as ItemButton).IsSelected;
		}

		#endregion
	}
}
