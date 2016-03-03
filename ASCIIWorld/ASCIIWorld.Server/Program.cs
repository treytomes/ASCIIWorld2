using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Server
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			ServiceHost worldServiceHost = null;
			try
			{
				// Base Address for StudentService
				Uri httpBaseAddress = new Uri("http://localhost:4321/WorldService");

				// Instantiate ServiceHost
				worldServiceHost = new ServiceHost(typeof(WorldService));

				// Open
				worldServiceHost.Open();
				Console.WriteLine("Service is live now at: {0}", worldServiceHost.BaseAddresses.First());
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"There is an issue with WorldService: {ex.Message}");
			}
			finally
			{
				if (worldServiceHost.State != CommunicationState.Closed)
				{
					worldServiceHost.Close();
				}
				worldServiceHost = null;
			}
		}
	}
}
