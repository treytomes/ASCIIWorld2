﻿using CommonCore.Math;
using System;
using System.Runtime.Serialization;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Chunk is composed of a 3D array of Blocks.
	/// A Level is composed of many Chunks, but only certain Chunks will be updated each frame.
	/// A World is composed of many Levels, each of the same size, all stacked on top of eachother.
	/// </summary>
	[DataContract]
	public class Chunk
	{
		#region Constants

		private const int CHUNK_HEIGHT = 64;
		private const int CHUNK_WIDTH = 64;

		#endregion

		#region Fields

		[DataMember]
		private int[,,] _blockIndex;
		
		#endregion

		#region Constructors

		public Chunk()
		{
			_blockIndex = new int[Enum.GetValues(typeof(ChunkLayer)).Length, CHUNK_HEIGHT, CHUNK_WIDTH];
		}

		#endregion

		#region Properties

		/// <summary>
		/// The chunk id is assigned by WorldService.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

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
				if ((y >= 0) && (y < CHUNK_HEIGHT) && (x >= 0) && (x < CHUNK_WIDTH))
				{
					_blockIndex[(int)layer, y, x] = value;
				}
				// TODO: Should this return 0 if out of range?
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Search the chunk randomly for a spot that isn't blocked.
		/// </summary>
		public Point FindSpawnPoint()
		{
			var random = new Random();
			while (true)
			{
				var x = random.Next(0, Width);
				var y = random.Next(0, Height);
				if (this[ChunkLayer.Blocking, x, y] == 0)
				{
					return new Point(x, y);
				}
			}
		}

		#endregion
	}
}
