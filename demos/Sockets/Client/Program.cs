using System;

namespace Client
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			new UdpClient(Console.Out).Run();
		}
	}
}
