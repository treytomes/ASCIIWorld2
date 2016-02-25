using ASCIIWorld.Data;
using GameCore.Rendering;
using GameCore.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.IO;
using ASCIIWorld.Generation;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using System.Threading;
using System.Collections.Concurrent;

namespace ASCIIWorld
{
	public class LoadWorldGameState : GameState
	{
		#region Fields

		private Viewport _viewport;
		private OrthographicProjection _projection;
		private ITessellator _tessellator;

		private BlockRegistry _blocks;
		private Chunk _chunk;
		private TileSet _ascii;

		private ConcurrentStack<string> _progressMessages;
		private Task _loadingTask;

		#endregion

		#region Constructors

		public LoadWorldGameState(GameStateManager manager)
			: base(manager)
		{
			_viewport = new Viewport(0, 0, manager.GameWindow.Width, manager.GameWindow.Height);
			_projection = new OrthographicProjection(_viewport)
			{
				ZNear = -10,
				ZFar = 10
			};
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
			_blocks = new SampleBlockRegistry(content);
			_loadingTask = Task.Run(() => _chunk = new CavernChunkGenerator(_blocks as SampleBlockRegistry, 50, "hello!").Generate(progress))
				.ContinueWith(x => Thread.Sleep(100));
		}

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);

			if (_loadingTask.IsCompleted)
			{
				Manager.SwitchStates(new GameplayState(Manager, _blocks, _chunk));
			}
		}

		public override void Render()
		{
			base.Render();

			_projection.Apply();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Translate(0, 0, 10);

			var scale = new Vector2(_ascii.Width, _ascii.Height) * 2;
			_tessellator.Scale(scale.X, scale.Y);

			var alpha = 255;

			_tessellator.Translate(0, Manager.GameWindow.Height - scale.Y);
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
