using System.Collections.Generic;
using System.Net;

namespace Server
{
	internal class StateObject
	{
		#region Constants

		private const int BUFFER_SIZE = 1024;

		#endregion

		#region Fields

		public byte[] Data;
		
		public EndPoint EndPoint;

		public List<byte> DataStream;

		#endregion

		#region Constructors

		public StateObject()
		{
			Data = new byte[BUFFER_SIZE];
			DataStream = new List<byte>();
		}

		#endregion
	}
}