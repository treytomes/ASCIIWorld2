﻿using CommonCore.Math;
using System;

namespace ASCIIWorld.Data.Generation
{
	public interface IChunkGenerator
	{
		float AmbientLightLevel { get; }

		Chunk Generate(IProgress<string> progress, int chunkX, int chunkY);

		void Fill(Chunk chunk, ChunkLayer layer, int blockId);
		void Fill(Chunk chunk, ChunkLayer layer, RectI bounds, int blockId);
		void DrawVerticalLine(Chunk chunk, ChunkLayer layer, int top, int bottom, int x, int blockId);
		void DrawHorizontalLine(IChunkAccess chunk, ChunkLayer layer, int y, int left, int right, int blockId);
	}
}
