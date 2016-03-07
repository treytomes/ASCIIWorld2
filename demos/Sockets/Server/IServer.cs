using CommonData;
using System.Net;

namespace Server
{
	public interface IServer
	{
		void Broadcast(ChatPacket sendPacket);
		void Kick(EndPoint clientEndPoint);
	}
}