namespace ASCIIWorld.Data
{
	public enum ChunkLayer
	{
		Background = 0,
		Floor = 1,
		Blocking = 2,
		Ceiling = 3,

		/// <summary>
		/// Entities will be drawn on the blocking layer.
		/// </summary>
		Entity = Blocking
	}
}