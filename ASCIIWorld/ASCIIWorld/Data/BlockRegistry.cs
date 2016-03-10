using CommonCore;
using System;

namespace ASCIIWorld.Data
{
	public class BlockRegistry : ObjectRegistry<Block>
	{
		public void Update(TimeSpan elapsed)
		{
			foreach (var block in this)
			{
				block.Renderer.Update(elapsed);
			}
		}
	}
}
