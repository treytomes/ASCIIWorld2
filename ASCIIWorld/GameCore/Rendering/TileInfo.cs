namespace GameCore.Rendering
{
	public class TileInfo
	{
		public TileInfo(string name, ITileContentSource source)
		{
			Name = name;
			Source = source;
		}

		public string Name { get; private set; }

		public ITileContentSource Source { get; private set; }
	}
}