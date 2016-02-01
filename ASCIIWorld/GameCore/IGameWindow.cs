using System;

namespace GameCore
{
	public interface IGameWindow
	{
		event EventHandler<EventArgs> Resize;

		/// <summary>
		/// Does the game window have input focus?
		/// </summary>
		bool Focused { get; }

		/// <summary>
		/// The game window width in pixels.
		/// </summary>
		int Width { get; }


		/// <summary>
		/// The game window height in pixels.
		/// </summary>
		int Height { get; }
	}
}