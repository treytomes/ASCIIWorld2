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
			RegisterBlock(1, Water = content.Load<Block>("Blocks/Water.xml"));
			RegisterBlock(2, Grass = content.Load<Block>("Blocks/Grass.xml"));
			RegisterBlock(3, Stone = content.Load<Block>("Blocks/Stone.xml"));
		}

		public Block Water { get; private set; }

		public Block Grass { get; private set; }

		public Block Stone { get; private set; }
	}
}
