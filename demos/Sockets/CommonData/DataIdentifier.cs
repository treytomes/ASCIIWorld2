namespace CommonData
{
	/* ----------------
	 * Packet Structure
	 * ----------------
	 *
	 * Description   -> |packetSize|dataIdentifier|numParameters|parameter length [1]|parameter [1]   |parameter length [n]|parameter [n]   |
	 * Size in bytes -> |    4     |       4      |      4      |          4         |parameter length|          4         |parameter length|
	 */

	public enum DataIdentifier
	{
		Null = 0,
		LogIn = 1,
		LogOut = 2,
		Message = 3
	}

}
