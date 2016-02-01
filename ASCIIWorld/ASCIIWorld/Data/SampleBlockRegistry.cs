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
			Water = content.Load<Block>("Blocks/Water.xml");
			Grass = content.Load<Block>("Blocks/Grass.xml");

			RegisterBlock(1, Water);
			RegisterBlock(2, Grass);
		}

		public Block Water { get; private set; }

		public Block Grass { get; private set; }
	}
}
