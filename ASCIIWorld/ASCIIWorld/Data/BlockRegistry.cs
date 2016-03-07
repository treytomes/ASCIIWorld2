using CommonCore;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;

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
