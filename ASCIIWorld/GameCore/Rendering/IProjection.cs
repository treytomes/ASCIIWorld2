namespace GameCore.Rendering
{
	public interface IProjection
	{
		Viewport Viewport { get; }

		void Resize(Viewport viewport);
		void Apply();
	}
}
