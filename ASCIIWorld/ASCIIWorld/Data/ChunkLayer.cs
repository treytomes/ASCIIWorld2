using System;
using System.ComponentModel;

namespace ASCIIWorld.Data
{
	public enum ChunkLayer
	{
		[Description("Background")]
		Background = 0,

		[Description("Floor")]
		Floor = 1,

		[Description("Blocking")]
		Blocking = 2,
		
		[Description("Ceiling")]
		Ceiling = 3
	}

	public static class ChunkLayerExtensions
	{
		public static bool HasLayerAbove(this ChunkLayer @this)
		{
			return @this != ChunkLayer.Ceiling;
		}

		public static ChunkLayer GetLayerAbove(this ChunkLayer @this)
		{
			switch (@this)
			{
				case ChunkLayer.Background:
					return ChunkLayer.Floor;
				case ChunkLayer.Floor:
					return ChunkLayer.Blocking;
				case ChunkLayer.Blocking:
					return ChunkLayer.Ceiling;
				default:
					throw new InvalidOperationException("This isn't possible.");
			}
		}
	}
}