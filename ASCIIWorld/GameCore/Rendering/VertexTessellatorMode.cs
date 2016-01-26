namespace GameCore.Rendering
{
	public enum VertexTessellatorMode
	{
		/// <summary>
		/// Render the VBO, then delete it.
		/// </summary>
		Render,

		/// <summary>
		/// Generate the VBO, but don't render it.
		/// </summary>
		Generate
	}
}