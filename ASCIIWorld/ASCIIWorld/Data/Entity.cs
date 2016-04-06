using ASCIIWorld.Rendering;
using GameCore.Rendering;
using OpenTK;
using System;

namespace ASCIIWorld.Data
{
	[Serializable]
	public class Entity : IUpdateable
	{
		#region Fields

		private Vector2 _position;
		private bool _isAlive;

		#endregion

		#region Constructors

		public Entity()
		{
			_isAlive = true;
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

		public bool IsAlive
		{
			get
			{
				return _isAlive;
			}
		}

		#endregion

		#region Methods

		public virtual void Update(TimeSpan elapsed)
		{
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

		public virtual bool ContainsPoint(Vector2 position)
		{
			return Math.Abs((_position - position).Length) <= 0.5;
		}

		public virtual void Touch(Entity touchedBy)
		{
		}

		/// <summary>
		/// Mark this entity to be removed from the world.
		/// </summary>
		public void Die()
		{
			_isAlive = false;
		}

		#endregion
	}
}
