using System;

namespace GameCore.Rendering
{
	/// <summary>
	/// Represents exceptions related to IGraphicsResources.
	/// </summary>
	public class GraphicsResourceException : Exception
	{
		/// <summary>Constructs a new GraphicsResourceException.</summary>
		public GraphicsResourceException()
			: base()
		{
		}

		/// <summary>Constructs a new string with the specified error message.</summary>
		public GraphicsResourceException(string message)
			: base(message)
		{
		}
	}
}