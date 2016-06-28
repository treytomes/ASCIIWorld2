using OpenTK.Input;
using System.Runtime.CompilerServices;

namespace GameCore
{
	/// <summary>
	/// Used on MouseEventArgs to determine whether the event has been processed.
	/// </summary>
	public static class MouseEventArgsExtensions
	{
		private sealed class Fields
		{
			internal bool IsProcessed { get; set; }
		}

		private static ConditionalWeakTable<MouseEventArgs, Fields> _table = new ConditionalWeakTable<MouseEventArgs, Fields>();

		public static bool IsProcessed(this MouseEventArgs @this)
		{
			Fields fields;

			if (!_table.TryGetValue(@this, out fields))
			{
				IsProcessed(@this, false);
				return false;
			}
			else
			{
				return _table.GetOrCreateValue(@this).IsProcessed;
			}
		}

		public static void IsProcessed(this MouseEventArgs @this, bool value)
		{
			_table.GetOrCreateValue(@this).IsProcessed = value;
		}
	}
}
