using System;

namespace CommonCore.Math
{
	public static class MathHelper
	{
		public static bool IsInRange<T>(T value, T inclusiveMin, T exclusiveMax)
			where T : IComparable<T>
		{
			return (inclusiveMin.CompareTo(value) <= 0) && (value.CompareTo(exclusiveMax) < 0);
		}

		public static T Clamp<T>(T value, T inclusiveMin, T inclusiveMax)
			where T : IComparable<T>
		{
			if (value.CompareTo(inclusiveMin) < 0)
			{
				return inclusiveMin;
			}
			else if (inclusiveMax.CompareTo(value) < 0)
			{
				return inclusiveMax;
			}
			else
			{
				return value;
			}
		}

		public static double Modulo(double a, double b)
		{
			return a - b * System.Math.Floor(a / b);
		}

		// TODO: Move Unproject from GameplayState to MathHelper.
	}
}
