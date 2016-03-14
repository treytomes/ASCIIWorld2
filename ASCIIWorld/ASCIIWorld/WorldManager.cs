using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using GameCore;
using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld
{
	/// <summary>
	/// Manage data and rendering for the game world.
	/// </summary>
	public class WorldManager
	{
		#region Fields

		private ChunkRenderer _chunkRenderer;

		#endregion

		#region Constructors

		public WorldManager(Viewport viewport, BlockRegistry blocks, Level level)
		{
			Blocks = blocks;
			Level = level;

			Camera = GameCore.Camera.CreateOrthographicCamera(viewport);
			_chunkRenderer = new ChunkRenderer(viewport, Blocks);
		}

		#endregion

		#region Properties

		public Camera<OrthographicProjection> Camera { get; private set; }

		public Level Level { get; private set; }

		public BlockRegistry Blocks { get; private set; }

		#endregion

		#region Methods

		public void Resize(Viewport viewport)
		{
			Camera.Resize(viewport);
		}

		public void Update(TimeSpan elapsed)
		{
			Blocks.Update(elapsed);
		}

		public void Render()
		{
			Camera.Apply();
			_chunkRenderer.Render(Camera, Level);
		}

		#endregion
	}
}
