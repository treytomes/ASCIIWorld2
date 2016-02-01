using System;

namespace GameCore
{
	public class FrameEventArgs : EventArgs
	{
		public FrameEventArgs(TimeSpan elapsed)
		{
			Elapsed = elapsed;
		}

		public TimeSpan Elapsed { get; private set; }
	}
}