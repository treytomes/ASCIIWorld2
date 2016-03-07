using GameCore.IO;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Created for demo purposes.
	/// </summary>
	// TODO: Put this in an xml file.
	public class SampleBlockRegistry : BlockRegistry
	{
		public SampleBlockRegistry(ContentManager content)
		{
			RegisterBlock(1, content.Load<Block>("Blocks/Water.xml"));
			RegisterBlock(2, content.Load<Block>("Blocks/Grass.xml"));
			RegisterBlock(3, content.Load<Block>("Blocks/Stone.xml"));
			RegisterBlock(4, content.Load<Block>("Blocks/Bush.xml"));
		}
	}
}
