﻿using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation
{
	public abstract class BaseChunkGenerator : IChunkGenerator
	{
		#region Fields

		#endregion

		#region Constructors

		public BaseChunkGenerator(int width, int height, string seed)
		{
			Width = width;
			Height = height;

			Seed = seed ?? DateTime.Now.GetHashCode().ToString();
			Random = new Random(Seed.GetHashCode());
		}

		#endregion

		#region Properties

		public abstract float AmbientLightLevel { get; }

		protected int Width { get; private set; }

		protected int Height { get; private set; }

		protected Random Random { get; private set; }

		protected string Seed { get; private set; }

		#endregion

		#region Methods

		public abstract Chunk Generate(IProgress<string> progress, int chunkX, int chunkY);

		public void Fill(Chunk chunk, ChunkLayer layer, int blockId)
		{
			Fill(chunk, layer, new RectI(0, 0, chunk.Width, chunk.Height), blockId);
		}

		public void Fill(Chunk chunk, ChunkLayer layer, RectI bounds, int blockId)
		{
			for (var row = bounds.Top; row <= bounds.Bottom; row++)
			{
				for (var column = bounds.Left; column <= bounds.Right; column++)
				{
					chunk[layer, column, row] = blockId;
				}
			}
		}

		public void DrawVerticalLine(Chunk chunk, ChunkLayer layer, int top, int bottom, int x, int blockId)
		{
			for (var row = top; row <= bottom; row++)
			{
				chunk[layer, x, row] = blockId;
			}
		}

		public void DrawHorizontalLine(IChunkAccess chunk, ChunkLayer layer, int y, int left, int right, int blockId)
		{
			for (var column = left; column <= right; column++)
			{
				chunk[layer, column, y] = blockId;
			}
		}

		protected void Reseed(int chunkX, int chunkY)
		{
			// TODO: This will probably need to change.
			Random = new Random(Seed.GetHashCode() + chunkX + chunkY * Width);
		}

		#endregion
	}
}
