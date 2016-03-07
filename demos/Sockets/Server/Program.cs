using System;

namespace Server
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			new ChatServer(Console.Out).Start();
			Console.WriteLine("Press any key to end.");
			Console.ReadKey();
		}
	}
}
