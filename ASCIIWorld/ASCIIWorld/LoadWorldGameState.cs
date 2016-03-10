using ASCIIWorld.Data;
using GameCore.Rendering;
using GameCore.StateManagement;
using System;
using System.Threading.Tasks;
using GameCore.IO;
using ASCIIWorld.Generation;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Threading;
using System.Collections.Concurrent;
using GameCore;

namespace ASCIIWorld
{
	public class LoadWorldGameState : GameState
	{
		#region Fields

		private ITessellator _tessellator;
		private Camera<OrthographicProjection> _hudCamera;

		private BlockRegistry _blocks;
		private Level _level;
		private TileSet _ascii;

		private ConcurrentStack<string> _progressMessages;
		private Task _loadingTask;

		#endregion

		#region Constructors

		public LoadWorldGameState(GameStateManager manager)
			: base(manager)
		{
			var viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);
			_hudCamera = Camera.CreateOrthographicCamera(viewport);
			_hudCamera.Projection.OrthographicSize = viewport.Height / 2;
			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };

			_progressMessages = new ConcurrentStack<string>();
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_ascii = content.Load<TileSet>("TileSets/ASCII.xml");
			
			var progress = new Progress<string>(message => _progressMessages.Push(message));
			_blocks = content.Load<BlockRegistry>("Blocks/SampleBlockRegistry.xml");

			_loadingTask = Task.Run(() =>
			{
				_level = new Level(_blocks.ToDictionary());
				var chunk = _level[ChunkLayer.Floor, 0, 0]; // generate the first chunk
			}).ContinueWith(x => Thread.Sleep(100));
		}

		public override void Resize(Viewport viewport)
		{
			base.Resize(viewport);
			_hudCamera.Resize(viewport);
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			if (_loadingTask.IsCompleted)
			{
				Manager.SwitchStates(new GameplayState(Manager, _blocks, _level));
			}
		}

		public override void Render()
		{
			base.Render();

			_hudCamera.Apply();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Translate(0, 0, 10);

			var scale = new Vector2(_ascii.Width, _ascii.Height) * 2;
			_tessellator.Scale(scale.X, scale.Y);

			var alpha = 255;

			_tessellator.Translate(_hudCamera.Projection.Left, _hudCamera.Projection.Bottom - scale.Y);
			foreach (var message in _progressMessages)
			{
				_tessellator.BindColor(Color.FromArgb(alpha, Color.White));
				_ascii.RenderText(_tessellator, message);
				_tessellator.Translate(0, -scale.Y);
				alpha = MathHelper.Clamp(alpha - 8, 0, 255);
			}

			_tessellator.End();
		}

		#endregion
	}
}
