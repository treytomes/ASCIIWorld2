using ASCIIWorld.Data;
using CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ASCIIWorld.Server
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWorldService" in both code and config file together.
	[ServiceContract]
	public interface IWorldService
	{
		[OperationContract]
		[FaultContract(typeof(WorldServiceFault))]
		Chunk GetChunk(int chunkId);

		[OperationContract]
		[FaultContract(typeof(WorldServiceFault))]
		Chunk GenerateChunk(Dictionary<int, string> blocks, string seed);
	}
}
