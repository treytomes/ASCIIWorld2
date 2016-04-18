using OpenTK.Input;
using System;

namespace GameCore
{
	public static class KeyExtensions
	{
		public static char ToChar(this Key @this)
		{
			switch (@this)
			{
				case Key.Number0:
					return '0';
				case Key.Number1:
					return '1';
				case Key.Number2:
					return '2';
				case Key.Number3:
					return '3';
				case Key.Number4:
					return '4';
				case Key.Number5:
					return '5';
				case Key.Number6:
					return '6';
				case Key.Number7:
					return '7';
				case Key.Number8:
					return '8';
				case Key.Number9:
					return '9';
				default:
					throw new NotImplementedException("This conversion is not defined.");
			}
		}
	}
}
