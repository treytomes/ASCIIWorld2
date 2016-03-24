using CommonCore;
using System;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// There can only be one block registry.
	/// </summary>
	/// <seealso cref="CommonCore.ObjectRegistry{ASCIIWorld.Data.Block}" />
	public class BlockRegistry : ObjectRegistry<Block>
	{
		static BlockRegistry()
		{
			Instance = new BlockRegistry();
		}

		private BlockRegistry()
		{
		}

		public static BlockRegistry Instance { get; private set; }

		public void Update(TimeSpan elapsed)
		{
			foreach (var block in this)
			{
				block.Renderer.Update(elapsed);
			}
		}
	}
}
