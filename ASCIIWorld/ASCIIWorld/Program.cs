using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var settings = new AppSettings();

			Console.WriteLine("Launching game window.");
			new ASCIIWorldGameWindow().Run(settings.UpdatesPerSecond, settings.FramesPerSecond);
		}
	}
}
