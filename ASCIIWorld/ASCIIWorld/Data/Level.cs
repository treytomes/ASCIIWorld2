﻿using ASCIIWorld.Data.Generation.BSP;
using ASCIIWorld.Data.Generation.Dugout;
using ASCIIWorld.Data.Generation.Labyrinth;
using ASCIIWorld.Data.Generation;
using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Chunk is composed of a 3D array of Blocks.
	/// A Level is composed of a 2D array of Chunks, but only certain Chunks will be updated each frame.
	/// A World is composed of many Levels, each of the same size, all stacked on top of eachother.
	/// </summary>
	[Serializable]
	public class Level : IChunkAccess
	{
		#region Constants

		private const int CHUNK_WIDTH = 64;
		private const int CHUNK_HEIGHT = 64;

		/// <summary>
		/// How many chunk wide?
		/// </summary>
		private const int LEVEL_WIDTH = 16;

		/// <summary>
		/// How many chunks long?
		/// </summary>
		private const int LEVEL_HEIGHT = 16;

		#endregion

		#region Fields
		
		private Chunk[,] _chunks;

		#endregion

		#region Constructors

		public Level()
		{
			_chunks = new Chunk[LEVEL_HEIGHT, LEVEL_WIDTH];
		}

		#endregion

		#region Properties

		public int Width
		{
			get
			{
				return LEVEL_WIDTH;
			}
		}

		public int Height
		{
			get
			{
				return LEVEL_HEIGHT;
			}
		}

		public int this[ChunkLayer layer, int blockX, int blockY]
		{
			get
			{
				var chunk = GetChunk(blockX, blockY);

				var chunkX = (int)MathHelper.Modulo(blockX, CHUNK_WIDTH);
				var chunkY = (int)MathHelper.Modulo(blockY, CHUNK_HEIGHT);
				return chunk[layer, chunkX, chunkY];
			}
			set
			{
				var chunk = GetChunk(blockX, blockY);

				var chunkX = (int)MathHelper.Modulo(blockX, CHUNK_WIDTH);
				var chunkY = (int)MathHelper.Modulo(blockY, CHUNK_HEIGHT);
				chunk[layer, chunkX, chunkY] = value;
			}
		}

		public IEnumerable<Entity> Entities
		{
			get
			{
				foreach (var chunk in _chunks)
				{
					if (chunk == null)
					{
						continue;
					}

					foreach (var entity in chunk.Entities)
					{
						yield return entity;
					}
				}
				yield break;
			}
		}

		#endregion

		#region Methods

		public void AddEntity(Entity entity)
		{
			var chunk = GetChunk((int)entity.Position.X, (int)entity.Position.Y);
			chunk.AddEntity(entity);
		}

		public void RemoveEntity(Entity entity)
		{
			var chunk = GetChunk((int)entity.Position.X, (int)entity.Position.Y);
			chunk.RemoveEntity(entity);
		}

		public ChunkLayer GetHighestVisibleLayer(int blockX, int blockY)
		{
			var chunk = GetChunk(blockX, blockY);
			var chunkX = (int)MathHelper.Modulo(blockX, CHUNK_WIDTH);
			var chunkY = (int)MathHelper.Modulo(blockY, CHUNK_HEIGHT);
			return chunk.GetHighestVisibleLayer(chunkX, chunkY);
		}

		public bool CanSeeSky(ChunkLayer layer, int blockX, int blockY)
		{
			var chunk = GetChunk(blockX, blockY);
			var chunkX = (int)MathHelper.Modulo(blockX, CHUNK_WIDTH);
			var chunkY = (int)MathHelper.Modulo(blockY, CHUNK_HEIGHT);
			return chunk.CanSeeSky(layer, chunkX, chunkY);
		}

		private void GenerateChunk(int chunkX, int chunkY)
		{
			IProgress<string> progress = new Progress<string>(message => Console.WriteLine(message));

			//_chunks[chunkY, chunkX] = new CavernChunkGenerator(_blocks, CHUNK_WIDTH, CHUNK_HEIGHT, "hello!").Generate(progress);
			//_chunks[chunkY, chunkX] = new CavernChunkGenerator(_blocks, CHUNK_WIDTH, CHUNK_HEIGHT, null).Generate(progress);
			//_chunks[chunkY, chunkX] = new DugoutDungeonChunkGenerator(_blocks, CHUNK_WIDTH, CHUNK_HEIGHT, null).Generate(progress);
			//_chunks[chunkY, chunkX] = new LabyrinthChunkGenerator(_blocks, CHUNK_WIDTH, CHUNK_HEIGHT, null).Generate(progress);
			//_chunks[chunkY, chunkX] = new BSPDungeonChunkGenerator(_blocks, CHUNK_WIDTH, CHUNK_HEIGHT, null).Generate(progress);
			_chunks[chunkY, chunkX] = new OverworldChunkGenerator(CHUNK_WIDTH, CHUNK_HEIGHT, null, chunkX, chunkY).Generate(progress);
		}

		/// <summary>
		/// Get the chunk at the given block position.
		/// </summary>
		/// <remarks>
		/// The chunk will be generated if it doesn't exist yet.
		/// </remarks>
		public Chunk GetChunk(int blockX, int blockY)
		{
			var levelX = (int)MathHelper.Modulo(Math.Floor((float)blockX / CHUNK_WIDTH), LEVEL_WIDTH);
			var levelY = (int)MathHelper.Modulo(Math.Floor((float)blockY / CHUNK_HEIGHT), LEVEL_HEIGHT);
			if (_chunks[levelY, levelX] == null)
			{
				GenerateChunk(levelX, levelY);
			}
			return _chunks[levelY, levelX];
		}

		public Chunk GetChunk(OpenTK.Vector2 position)
		{
			return GetChunk((int)position.X, (int)position.Y);
		}

		public Chunk GetChunk(Entity entity)
		{
			return GetChunk(entity.Position);
		}

		#endregion
	}
}
