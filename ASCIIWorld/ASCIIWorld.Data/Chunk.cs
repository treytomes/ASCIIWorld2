using CommonCore.Math;
using System;
using System.Runtime.Serialization;

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
				return _blockIndex[(int)layer, y, x];
			}
			set
			{
				_blockIndex[(int)layer, y, x] = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Search the chunk randomly for a spot that isn't blocked.
		/// </summary>
		public Vector2I? FindSpawnPoint()
		{
			var random = new Random();
			for (var n = 0; n < 32; n++)
			//while (true)
			{
				var x = random.Next(0, _width);
				var y = random.Next(0, _height);
				if (this[ChunkLayer.Blocking, x, y] == 0)
				{
					return new Vector2I(x, y);
				}
			}
			return null;
		}

		public bool CanSeeSky(ChunkLayer layer, int blockX, int blockY)
		{
			switch (layer)
			{
				case ChunkLayer.Ceiling:
					return true;
				case ChunkLayer.Blocking:
					return this[ChunkLayer.Ceiling, blockX, blockY] == 0;
				case ChunkLayer.Floor:
					return CanSeeSky(ChunkLayer.Blocking, blockX, blockY) && this[ChunkLayer.Blocking, blockX, blockY] == 0;
				case ChunkLayer.Background:
					return CanSeeSky(ChunkLayer.Floor, blockX, blockY) && this[ChunkLayer.Floor, blockX, blockY] == 0;
				default:
					return false;
			}
		}

		#endregion
	}
}
