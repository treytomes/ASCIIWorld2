using CommonData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Server
{
	public class PacketManager
	{
		#region Fields

		private Dictionary<DataIdentifier, Func<EndPoint, ChatPacket, ChatPacket>> _packetHandlers;

		#endregion

		#region Constructors

		public PacketManager(TextWriter logger, IServer server)
		{
			Logger = logger;
			Server = server;

			_packetHandlers = new Dictionary<DataIdentifier, Func<EndPoint, ChatPacket, ChatPacket>>();
		}

		#endregion

		#region Properties

		protected TextWriter Logger { get; private set; }

		protected IServer Server { get; private set; }

		#endregion

		#region Methods

		public void Register(DataIdentifier id, Func<EndPoint, ChatPacket, ChatPacket> handler)
		{
			_packetHandlers.Add(id, handler);
		}

		public void HandlePacket(EndPoint senderEndPoint, ChatPacket receivedData)
		{
			if (_packetHandlers.ContainsKey(receivedData.ChatDataIdentifier))
			{
				Server.Broadcast(_packetHandlers[receivedData.ChatDataIdentifier](senderEndPoint, receivedData));
			}
			else
			{
				Logger.WriteLine("Bad packet received.");
				Server.Kick(senderEndPoint);
			}
		}

		#endregion
	}
}