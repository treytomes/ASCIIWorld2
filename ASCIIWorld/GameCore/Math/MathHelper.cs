using System;

namespace GameCore.Math
{
	public static class MathHelper
	{
		public static bool IsInRange<T>(T value, T inclusiveMin, T exclusiveMax)
			where T : IComparable<T>
		{
			return (inclusiveMin.CompareTo(value) <= 0) && (value.CompareTo(exclusiveMax) < 0);
		}
	}
}
