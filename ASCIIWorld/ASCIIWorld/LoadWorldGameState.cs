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
		private Chunk _chunk;
		private TileSet _ascii;

		private ConcurrentStack<string> _progressMessages;
		private WorldServiceReference.WorldServiceClient _worldServer;
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
			_worldServer = new WorldServiceReference.WorldServiceClient();
		}

		#endregion

		#region Methods

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			_ascii = content.Load<TileSet>("TileSets/ASCII.xml");
			
			var progress = new Progress<string>(message => _progressMessages.Push(message));
			_blocks = new SampleBlockRegistry(content);

			_loadingTask = _worldServer.GenerateChunkAsync(_blocks.ToDictionary(), "hello!").ContinueWith(task => _chunk = task.Result);

			//_loadingTask = Task.Run(() => _chunk = new CavernChunkGenerator(_blocks, "hello!").Generate(progress))
			//	.ContinueWith(x => SpawnBushes(progress))
			//	.ContinueWith(x => Thread.Sleep(100));
		}

		public override void UnloadContent()
		{
			_worldServer.Close();

			base.UnloadContent();
		}

		//private void SpawnBushes(IProgress<string> progress)
		//{
		//	for (var n = 0; n < 10; n++)
		//	{
		//		progress.Report($"Planting bush (x{n + 1})...");
		//		var spawnPoint = _chunk.FindSpawnPoint();
		//		_chunk[ChunkLayer.Blocking, spawnPoint.X, spawnPoint.Y] = _blocks.GetId("Bush");
		//	}
		//}

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
				Manager.SwitchStates(new GameplayState(Manager, _blocks, _chunk));
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
