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

			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
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

		#endregion

		#region Methods

		public void LoadContent(ContentManager content)
		{
			// TODO: If the UI has mouse hover, I don't want the game world to respond to it.
			var testButton = new Button(_hudCamera, new Vector2(100, 100), "Save");
			testButton.LoadContent(content);
			testButton.Clicked += (sender, e) => Console.WriteLine("Clicked");
			_children.Add(testButton);

			var label = new Label(_hudCamera, new Vector2(-300, -300), "Hello");
			label.LoadContent(content);
			_children.Add(label);
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
	}
}
