using System.Net;

namespace Server
{
	/// <summary>
	/// Structure to store the client information.
	/// </summary>
	public struct Client<TClientInfo>
	{
		public readonly EndPoint EndPoint;
		public readonly TClientInfo Info;

		public Client(EndPoint endPoint, TClientInfo info)
		{
			EndPoint = endPoint;
			Info = info;
		}
	}
}