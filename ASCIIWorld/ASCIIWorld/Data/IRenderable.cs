using GameCore.Rendering;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// Something that can be rendered to a tessellator.
	/// </summary>
	public interface IRenderable
	{
		void Render(ITessellator tessellator);
		void Render(ITessellator tessellator, float x, float y);
	}
}
