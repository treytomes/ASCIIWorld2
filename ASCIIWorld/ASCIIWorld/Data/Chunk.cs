using System;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Chunk is composed of a 3D array of Blocks.
	/// A Level is composed of many Chunks, but only certain Chunks will be updated each frame.
	/// A World is composed of many Levels, each of the same size, all stacked on top of eachother.
	/// </summary>
	public class Chunk
	{
		#region Constants

		private const int CHUNK_HEIGHT = 256;
		private const int CHUNK_WIDTH = 256;

		#endregion

		#region Fields

		private int[,,] _blockIndex;
		
		#endregion

		#region Constructors

		public Chunk()
		{
			_blockIndex = new int[Enum.GetValues(typeof(ChunkLayer)).Length, CHUNK_HEIGHT, CHUNK_WIDTH];
		}

		#endregion

		#region Properties

		public int Height
		{
			get
			{
				return CHUNK_HEIGHT;
			}
		}

		public int Width
		{
			get
			{
				return CHUNK_WIDTH;
			}
		}

		public int this[ChunkLayer layer, int x, int y]
		{
			get
			{
				if ((y < 0) || (y >= CHUNK_HEIGHT) || (x < 0) || (x >= CHUNK_WIDTH))
				{
					return 0;
				}
				else
				{
					return _blockIndex[(int)layer, y, x];
				}
			}
			set
			{
				//if ((value != 0) && !_blockRegistry.IsDefined(value))
				//{
				//	throw new ArgumentException("This id is not registered.");
				//}

				if ((y >= 0) && (y < CHUNK_HEIGHT) && (x >= 0) && (x < CHUNK_WIDTH))
				{
					_blockIndex[(int)layer, y, x] = value;
				}
			}
		}

		#endregion
	}
}
