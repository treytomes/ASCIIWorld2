using CommonData;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Client
{
	public class UdpClient
	{
		#region Constants

		private const string SERVER_IP = "127.0.0.1";

		#endregion

		#region Fields

		private string _chatName;
		private Socket _clientSocket;
		private EndPoint _serverEndPoint;

		#endregion

		#region Constructors

		public UdpClient(TextWriter logger)
		{
			Logger = logger ?? Console.Out;
		}

		#endregion

		#region Properties

		protected TextWriter Logger { get; private set; }

		#endregion

		#region Methods

		public void Run()
		{
			var isDone = false;

			try
			{
				Console.Write("Enter your name: ");
				Connect(SERVER_IP, Console.ReadLine());

				while (!isDone)
				{
					Console.Write("> ");
					var text = Console.ReadLine();

					if (text == "exit")
					{
						isDone = true;
					}
					else
					{
						BeginSend(text);
					}
					Console.WriteLine();
				}

				Logout();
			}
			finally
			{
				if ((_clientSocket != null) && _clientSocket.Connected)
				{
					_clientSocket.Close();
					_clientSocket = null;
				}
			}
		}

		private void Connect(string serverIp, string name)
		{
			try
			{
				_chatName = name;

				// Initialize socket.
				_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				// Initialize server IP.
				var serverIP = IPAddress.Parse(serverIp);

				// Initialize the IPEndPoint for the server and use port SERVER_PORT.
				var server = new IPEndPoint(serverIP, ServerInfo.PORT);

				// Initialize the EndPoint for the server.
				_serverEndPoint = server as EndPoint;

				var state = new StateObject()
				{
					Data = new ChatPacket()
					{
						ChatName = _chatName,
						ChatMessage = null,
						ChatDataIdentifier = DataIdentifier.LogIn
					}.Serialize()
				};

				// Send data to server.
				_clientSocket.BeginSendTo(state.Data, 0, state.Data.Length, SocketFlags.None, _serverEndPoint, ar => _clientSocket.EndSend(ar), state);

				// Initialize data stream.
				state = new StateObject();

				// Begin listening for broadcasts from the server.
				_clientSocket.BeginReceiveFrom(state.Data, 0, state.Data.Length, SocketFlags.None, ref _serverEndPoint, EndReceive, state);
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Connection error!");
				Logger.WriteLine(ex.Message);
			}
		}

		private void BeginSend(string message)
		{
			try
			{
				var state = new StateObject()
				{
					Data = new ChatPacket()
					{
						ChatName = _chatName,
						ChatMessage = message.Trim(),
						ChatDataIdentifier = DataIdentifier.Message
					}.Serialize()
				};

				// Send packet to the server.
				_clientSocket.BeginSendTo(state.Data, 0, state.Data.Length, SocketFlags.None, _serverEndPoint, EndSend, state);
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Send error!");
				Logger.WriteLine(ex.Message);
			}
		}

		private void EndSend(IAsyncResult asyncResult)
		{
			var numBytesSent = _clientSocket.EndSend(asyncResult);
			var state = asyncResult.AsyncState as StateObject;

			if (numBytesSent != state.Data.Length)
			{
				// Send the rest of the data.
				var remainingData = new byte[state.Data.Length - numBytesSent];
				Array.Copy(state.Data, remainingData, remainingData.Length);
				state.Data = remainingData;

				_clientSocket.BeginSendTo(state.Data, numBytesSent, state.Data.Length, SocketFlags.None, _serverEndPoint, EndSend, state);
			}
		}

		private void Logout()
		{
			try
			{
				if (_clientSocket != null)
				{
					// Initialize a packet object to store the data to be sent.
					var sendData = new ChatPacket()
					{
						ChatDataIdentifier = DataIdentifier.LogOut,
						ChatName = _chatName,
						ChatMessage = null
					};

					// Get packet as byte array.
					var data = sendData.Serialize();

					// Send packet to the server.
					_clientSocket.SendTo(data, 0, data.Length, SocketFlags.None, _serverEndPoint);
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Logout error!");
				Logger.WriteLine(ex.Message);
			}
			finally
			{
				// Close the socket.
				_clientSocket.Close();
				_clientSocket = null;
			}
		}

		private void EndReceive(IAsyncResult asyncResult)
		{
			try
			{
				// Receive all data.
				var bytesReceived = _clientSocket.EndReceive(asyncResult);
				var state = asyncResult.AsyncState as StateObject;
				state.DataStream.AddRange(state.Data.Take(bytesReceived));
				Array.Clear(state.Data, 0, state.Data.Length); // reset data stream for the next receive

				var packetData = state.DataStream.ToArray();
				var packetSize = BitConverter.ToInt32(packetData, 0);
				if (packetSize == packetData.Length)
				{
					// Initialize a packet object to store the received data.
					var receivedData = Packet.Deserialize(packetData) as ChatPacket;

					if (receivedData.ChatMessage != null)
					{
						Console.WriteLine(receivedData.ChatMessage);
					}

					state.DataStream.Clear();
				}

				// If the packet size doesn't equal the packet length, we will keep receiving more data until it does.

				// Continue listening for broadcasts.
				_clientSocket.BeginReceiveFrom(state.Data, 0, state.Data.Length, SocketFlags.None, ref _serverEndPoint, EndReceive, state);
			}
			catch (ObjectDisposedException)
			{
				Logger.WriteLine("Object was disposed.");
			}
			catch (Exception ex)
			{
				Logger.WriteLine("ReceiveData error!");
				Logger.WriteLine(ex.Message);
			}
		}

		#endregion
	}
}
