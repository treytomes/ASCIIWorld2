using CommonData;
using System.IO;
using System.Net;

namespace Server
{
	public class ChatServer : UdpListener<ChatClientInfo>
	{
		#region Fields

		#endregion

		#region Constructors

		public ChatServer(TextWriter logger)
			: base(logger)
		{
			Packets.Register(DataIdentifier.LogIn, ProcessLoginPacket);
			Packets.Register(DataIdentifier.Message, ProcessMessagePacket);
			Packets.Register(DataIdentifier.LogOut, ProcessLogoutPacket);
		}

		#endregion

		#region Methods

		/// <returns>The packet to send out to all clients.</returns>
		private ChatPacket ProcessMessagePacket(EndPoint senderEndPoint, ChatPacket receivedData)
		{
			return new ChatPacket()
			{
				ChatDataIdentifier = receivedData.ChatDataIdentifier,
				ChatName = receivedData.ChatName,
				ChatMessage = $"{receivedData.ChatName}: {receivedData.ChatMessage}"
			};
		}

		/// <returns>The packet to send out to all clients.</returns>
		private ChatPacket ProcessLoginPacket(EndPoint senderEndPoint, ChatPacket receivedData)
		{
			Clients.Add(senderEndPoint, new ChatClientInfo(receivedData.ChatName));

			return new ChatPacket()
			{
				ChatDataIdentifier = receivedData.ChatDataIdentifier,
				ChatName = receivedData.ChatName,
				ChatMessage = $"-- {receivedData.ChatName} is online --"
			};
		}

		/// <returns>The packet to send out to all clients.</returns>
		private ChatPacket ProcessLogoutPacket(EndPoint senderEndPoint, ChatPacket receivedData)
		{
			Clients.Remove(senderEndPoint);

			return new ChatPacket()
			{
				ChatDataIdentifier = receivedData.ChatDataIdentifier,
				ChatName = receivedData.ChatName,
				ChatMessage = $"-- {receivedData.ChatName} has gone offline --"
			};
		}

		#endregion
	}
}
