using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Server
{
	public class ClientManager<TClientInfo> : IDisposable, IEnumerable<Client<TClientInfo>>
	{
		#region Fields

		/// <summary>
		/// The connected clients.
		/// </summary>
		private List<Client<TClientInfo>> _clients;

		#endregion

		#region Constructors

		public ClientManager(TextWriter logger)
		{
			Logger = logger ?? Console.Out;
			_clients = new List<Client<TClientInfo>>();
		}

		#endregion

		#region Properties

		protected TextWriter Logger { get; private set; }

		#endregion

		#region Methods

		public void Dispose()
		{
			Logger.Flush();
		}

		public void Add(EndPoint clientEndPoint, TClientInfo info)
		{
			if (_clients.Any(x => x.EndPoint == clientEndPoint))
			{
				Logger.WriteLine($"Cannot add client.  Client is already connected: {clientEndPoint}");
			}
			else
			{
				_clients.Add(new Client<TClientInfo>(clientEndPoint, info));
				Logger.WriteLine($"Client is connected: {clientEndPoint}");
			}
		}

		public void Remove(EndPoint clientEndPoint)
		{
			if (!_clients.Any(x => x.EndPoint.Equals(clientEndPoint)))
			{
				Logger.WriteLine($"Cannot remove client.  Client is not connected: {clientEndPoint}");
			}
			else
			{
				_clients.Remove(_clients.SingleOrDefault(c => c.EndPoint.Equals(clientEndPoint)));
				Logger.WriteLine($"Client is disconnected: {clientEndPoint}");
			}
		}

		public IEnumerator<Client<TClientInfo>> GetEnumerator()
		{
			foreach (var client in _clients)
			{
				yield return client;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}