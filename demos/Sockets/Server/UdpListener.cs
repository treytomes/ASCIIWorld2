using CommonData;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	public class UdpListener<TClientInfo> : IServer, IDisposable
	{
		#region Fields

		private Socket _serverSocket;

		#endregion

		#region Constructors

		public UdpListener(TextWriter logger)
		{
			Logger = logger ?? Console.Out;

			_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			// Associate the socket with the IP address and port.
			_serverSocket.Bind(new IPEndPoint(IPAddress.Any, ServerInfo.PORT));

			Clients = new ClientManager<TClientInfo>(Logger);
			Packets = new PacketManager(Logger, this);
		}

		#endregion

		#region Properties

		protected TextWriter Logger { get; private set; }

		protected ClientManager<TClientInfo> Clients { get; private set; }

		protected PacketManager Packets { get; private set; }

		#endregion

		#region Methods

		public void Start()
		{
			BeginReceive();
			Logger.WriteLine("The server is listening...");
		}

		public void Broadcast(ChatPacket sendPacket)
		{
			BeginBroadcast(sendPacket);
		}

		public void Kick(EndPoint clientEndPoint)
		{
			Logger.WriteLine($"Kicking client: ${clientEndPoint}");
			Clients.Remove(clientEndPoint);
		}

		public void Dispose()
		{
			if (_serverSocket != null)
			{
				_serverSocket.Dispose();
				_serverSocket = null;
			}

			Logger.Flush();
		}

		private void BeginReceive(EndPoint senderEndPoint = null)
		{
			senderEndPoint = senderEndPoint ?? new IPEndPoint(IPAddress.Any, 0);

			// Start listening for incoming data.
			var state = new StateObject();
			state.EndPoint = senderEndPoint;
			_serverSocket.BeginReceiveFrom(state.Data, 0, state.Data.Length, SocketFlags.None, ref senderEndPoint, EndReceive, state);
		}

		private void EndReceive(IAsyncResult asyncResult)
		{
			try
			{
				// This will contain the client's endpoint.
				EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

				// Receive all data.
				var bytesReceived = _serverSocket.EndReceiveFrom(asyncResult, ref senderEndPoint);
				var state = asyncResult.AsyncState as StateObject;
				state.EndPoint = senderEndPoint;
				state.DataStream.AddRange(state.Data.Take(bytesReceived));
				Array.Clear(state.Data, 0, state.Data.Length);


				var packetData = state.DataStream.ToArray();
				var packetSize = BitConverter.ToInt32(packetData, 0);
				if (packetSize == packetData.Length)
				{
					var receivedData = Packet.Deserialize(packetData) as ChatPacket;
					Packets.HandlePacket(senderEndPoint, receivedData);
					state.DataStream.Clear();

					// Listen for more connections.
					BeginReceive(senderEndPoint);
				}
				else
				{
					// If the packet size doesn't equal the packet length, we will keep receiving more data until it does.
					_serverSocket.BeginReceiveFrom(state.Data, 0, state.Data.Length, SocketFlags.None, ref senderEndPoint, EndReceive, state);
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine(ex);
			}
		}

		/// <summary>
		/// Broadcast the packet to all clients.
		/// </summary>
		private void BeginBroadcast(ChatPacket sendPacket)
		{
			// Get packet as byte array.
			var data = sendPacket.Serialize();

			foreach (var client in Clients)
			{
				var state = new StateObject()
				{
					EndPoint = client.EndPoint,
					Data = data
				};

				_serverSocket.BeginSendTo(state.Data, 0, state.Data.Length, SocketFlags.None, client.EndPoint, EndBroadcast, state);
			}

			// Report the status.
			Logger.WriteLine(sendPacket.ChatMessage);
		}

		private void EndBroadcast(IAsyncResult asyncResult)
		{
			var numBytesSent = _serverSocket.EndSend(asyncResult);
			var state = asyncResult.AsyncState as StateObject;
			
			if (numBytesSent != state.Data.Length)
			{
				// Send the rest of the data.
				var remainingData = new byte[state.Data.Length - numBytesSent];
				Array.Copy(state.Data, remainingData, remainingData.Length);
				state.Data = remainingData;

				_serverSocket.BeginSendTo(state.Data, 0, state.Data.Length, SocketFlags.None, state.EndPoint, EndBroadcast, state);
			}
		}

		#endregion
	}
}
