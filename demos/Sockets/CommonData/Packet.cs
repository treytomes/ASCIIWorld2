using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommonData
{
	/// <summary>
	/// All packets must inherit from this class.
	/// </summary>
	/// <remarks>
	/// All subclasses must be marked as serializable.
	/// </remarks>
	[Serializable]
	public abstract class Packet
	{
		public byte[] Serialize()
		{
			var stream = new MemoryStream();
			var formatter = new BinaryFormatter();

			formatter.Serialize(stream, this);

			var data = stream.GetBuffer();

			// The first 4 bytes of the data stream are the complete length of the data stream (including the size indicator).
			var size = BitConverter.GetBytes(sizeof(int) + data.Length);

			var sendData = new List<byte>();
			sendData.AddRange(size);
			sendData.AddRange(data);
			return sendData.ToArray();
		}

		public static Packet Deserialize(byte[] data)
		{
			var stream = new MemoryStream(data);

			// Read and skip over the object size.
			var length = BitConverter.ToInt32(data, 0);
			stream.Seek(sizeof(int), SeekOrigin.Begin);

			var formatter = new BinaryFormatter();
			var obj = formatter.Deserialize(stream);

			if (obj is Packet)
			{
				return obj as Packet;
			}
			else
			{
				throw new Exception("Attempting to deserialize an object that is not a packet!");
			}
		}
	}
}