using System;

namespace CommonData
{
	[Serializable]
	public class ChatPacket : Packet
	{
		#region Constructors

		public ChatPacket()
		{
			ChatDataIdentifier = DataIdentifier.Null;
			ChatName = null;
			ChatMessage = null;
		}

		#endregion

		#region Properties

		public DataIdentifier ChatDataIdentifier { get; set; }

		public string ChatName { get; set; }

		public string ChatMessage { get; set; }

		#endregion
	}
}
