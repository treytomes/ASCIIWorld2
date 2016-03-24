using CommonCore.Math;
using System;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Chunk is composed of a 3D array of Blocks.
	/// A Level is composed of a 2D array of Chunks, but only certain Chunks will be updated each frame.
	/// A World is composed of many Levels, each of the same size, all stacked on top of eachother.
	/// </summary>
	public class Chunk : IChunkAccess
	{
		#region Constants

		private const int NULL_BLOCK_ID = 0;

		#endregion

		#region Fields

		private int _width;
		private int _height;
		private int[,,] _blockIndex;
		
		#endregion

		#region Constructors

		public Chunk(int width, int height)
		{
			_width = width;
			_height = height;
			_blockIndex = new int[Enum.GetValues(typeof(ChunkLayer)).Length, _height, _width];
		}

		#endregion

		#region Properties

		public int Width
		{
			get
			{
				return _width;
			}
		}

		public int Height
		{
			get
			{
				return _height;
			}
		}

		public int this[ChunkLayer layer, int x, int y]
		{
			get
			{
				if (MathHelper.IsInRange(x, 0, Width) && MathHelper.IsInRange(y, 0, Height))
				{
					return _blockIndex[(int)layer, y, x];
				}
				else
				{
					return 0;
				}
			}
			set
			{
				if (MathHelper.IsInRange(x, 0, Width) && MathHelper.IsInRange(y, 0, Height))
				{
					_blockIndex[(int)layer, y, x] = value;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Search the chunk randomly for a spot that isn't blocked.
		/// </summary>
		public Vector2I? FindSpawnPoint()
		{
			const int MAX_SPAWN_ATTEMPTS = 32;
			var random = new Random();
			for (var n = 0; n < MAX_SPAWN_ATTEMPTS; n++)
			{
				var x = random.Next(0, _width);
				var y = random.Next(0, _height);
				if (this[ChunkLayer.Blocking, x, y] == NULL_BLOCK_ID)
				{
					return new Vector2I(x, y);
				}
			}
			return null;
		}

		public ChunkLayer GetHighestVisibleLayer(int blockX, int blockY)
		{
			if (this[ChunkLayer.Ceiling, blockX, blockY] != 0)
			{
				return ChunkLayer.Ceiling;
			}
			else if (this[ChunkLayer.Blocking, blockX, blockY] != 0)
			{
				return ChunkLayer.Blocking;
			}
			else if (this[ChunkLayer.Floor, blockX, blockY] != 0)
			{
				return ChunkLayer.Floor;
			}
			else if (this[ChunkLayer.Background, blockX, blockY] != 0)
			{
				return ChunkLayer.Background;
			}
			return ChunkLayer.Background;
		}

		public bool CanSeeSky(ChunkLayer layer, int blockX, int blockY)
		{
			if (!layer.HasLayerAbove())
			{
				return true;
			}
			else
			{
				var layerAbove = layer.GetLayerAbove();
				return CanSeeSky(layerAbove, blockX, blockY) && ((this[layerAbove, blockX, blockY] == NULL_BLOCK_ID) || !BlockRegistry.Instance.GetById(this[layerAbove, blockX, blockY]).IsOpaque);
			}
		}

		#endregion
	}
}
