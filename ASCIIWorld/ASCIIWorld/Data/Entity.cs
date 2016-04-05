using ASCIIWorld.Rendering;
using GameCore.IO;
using GameCore.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Data
{
	public class Entity : IUpdateable, IRenderable
	{
		#region Fields

		private IRenderable _renderable;
		private Vector2 _position;

		#endregion

		#region Constructors

		public Entity(IRenderable renderable)
		{
			_renderable = renderable;
		}

		#endregion

		#region Properties

		public Vector2 Position
		{
			get
			{
				return _position;
			}
		}

		#endregion

		#region Methods

		public virtual void Update(TimeSpan elapsed)
		{
		}

		public virtual void Render(ITessellator tessellator)
		{
			tessellator.PushTransform();
			tessellator.Translate(Position);
			_renderable.Render(tessellator);
			tessellator.PopTransform();
		}

		public void MoveTo(Level level, Vector2 position)
		{
			var oldChunk = level.GetChunk((int)_position.X, (int)_position.Y);
			_position = position;
			var newChunk = level.GetChunk((int)_position.X, (int)_position.Y);

			if (oldChunk != newChunk)
			{
				oldChunk.RemoveEntity(this);
				newChunk.AddEntity(this);
			}
		}

		#endregion
	}
}
