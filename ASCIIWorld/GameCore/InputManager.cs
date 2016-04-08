using CommonCore;
using OpenTK;
using OpenTK.Input;
using System;

namespace GameCore
{
	public class InputManager
	{
		#region Constructors

		private InputManager(GameWindow game)
		{
			Keyboard = game.Keyboard;
			//Joysticks = game.Joysticks;
			Mouse = game.Mouse;

			Keyboard.KeyRepeat = false;

			ConvertEx.AddConversion<Key, char>(KeyExtensions.ToChar);
		}

		#endregion

		#region Properties

		public static InputManager Instance { get; private set; }

		//public IList<JoystickDevice> Joysticks { get; private set; }

		public KeyboardDevice Keyboard { get; private set; }

		public MouseDevice Mouse { get; private set; }

		#endregion

		#region Methods

		public static void Initialize(GameWindow game)
		{
			if (Instance != null)
			{
				throw new Exception("InputManager has already been initialized.");
			}
			else
			{
				Instance = new InputManager(game);
			}
		}

		#endregion
	}
}