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

		private const int CHUNK_ROWS = 256;
		private const int CHUNK_COLUMNS = 256;
		//private const int CHUNK_ROWS = 64;
		//private const int CHUNK_COLUMNS = 64;

		#endregion

		#region Fields

		private int[,,] _blockIndex;
		
		#endregion

		#region Constructors

		public Chunk()
		{
			_blockIndex = new int[Enum.GetValues(typeof(ChunkLayer)).Length, CHUNK_ROWS, CHUNK_COLUMNS];
		}

		#endregion

		#region Properties

		public int Rows
		{
			get
			{
				return CHUNK_ROWS;
			}
		}

		public int Columns
		{
			get
			{
				return CHUNK_COLUMNS;
			}
		}

		public int this[ChunkLayer layer, int row, int column]
		{
			get
			{
				if ((row < 0) || (row >= CHUNK_ROWS) || (column < 0) || (column >= CHUNK_COLUMNS))
				{
					return 0;
				}
				else
				{
					return _blockIndex[(int)layer, row, column];
				}
			}
			set
			{
				//if ((value != 0) && !_blockRegistry.IsDefined(value))
				//{
				//	throw new ArgumentException("This id is not registered.");
				//}

				if ((row >= 0) && (row < CHUNK_ROWS) && (column >= 0) && (column < CHUNK_COLUMNS))
				{
					_blockIndex[(int)layer, row, column] = value;
				}
			}
		}

		#endregion
	}
}
