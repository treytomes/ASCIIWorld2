using OpenTK;
using System;
using System.Diagnostics;
using System.Threading;

namespace GameCore
{
	/// <summary>
	/// Handle rendering on a separate thread.
	/// </summary>
	public class BackgroundRenderer : IDisposable
	{
		#region Events

		public event EventHandler<EventArgs> Initialize;

		public event EventHandler<EventArgs> DisposeResources;

		/// <summary>
		/// Add your game rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		public event EventHandler<FrameEventArgs> RenderFrame;

		#endregion

		#region Fields

		private bool _disposed;
		private GameWindow _game;
		private Thread _renderTask;
		private bool _exitRequested;

		#endregion

		#region Constructors

		public BackgroundRenderer(GameWindow game)
		{
			_disposed = false;
			_game = game;

			_renderTask = new Thread(RenderLoop);
			_exitRequested = false;
		}

		#endregion

		#region Properties

		public GameWindow Game
		{
			get
			{
				return _game;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Start the renderer thread running in the background.
		/// </summary>
		public void Start()
		{
			// Release the OpenGL context so it can be used on the rendering thread.
			_game.Context.MakeCurrent(null);

			_renderTask.Start();
		}

		public void Exit()
		{
			_exitRequested = true;
			_renderTask.Join();
		}

		private void RenderLoop()
		{
			// We will dispose of this ourself.
			GC.SuppressFinalize(this);

			// The OpenGL context now belongs to the rendering thread.  No other thread may use it!
			_game.MakeCurrent();

			if (Initialize != null)
			{
				Initialize(this, EventArgs.Empty);
			}

			var renderTimer = Stopwatch.StartNew();
			var frameTime = TimeSpan.Zero;
			while (!_exitRequested)
			{
				HandleRender(frameTime);
				frameTime = renderTimer.Elapsed;
				renderTimer.Restart();
			}

			Dispose();
		}

		private void HandleRender(TimeSpan elapsed)
		{
			if (RenderFrame != null)
			{
				RenderFrame(this, new FrameEventArgs(elapsed));
			}
			_game.SwapBuffers();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (DisposeResources != null)
					{
						DisposeResources(this, EventArgs.Empty);
					}
				}
				_game.Context.MakeCurrent(null);
				_disposed = true;
			}
		}

		~BackgroundRenderer()
		{
			Dispose(false);
		}

		#endregion
	}
}
