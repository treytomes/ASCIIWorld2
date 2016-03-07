using System.Collections.Generic;

namespace Client
{
	internal class StateObject
	{
		private const int BUFFER_SIZE = 1024;

		public byte[] Data;
		
		public List<byte> DataStream;

		public StateObject()
		{
			Data = new byte[BUFFER_SIZE];
			DataStream = new List<byte>();
		}
	}
}
