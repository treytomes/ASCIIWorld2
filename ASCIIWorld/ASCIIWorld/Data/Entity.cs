using ASCIIWorld.Rendering;
using CommonCore.Math;
using OpenTK;
using System;

namespace ASCIIWorld.Data
{
	// TODO: Scale Entity by 0.8; calculate movement bounds accordingly.

	[Serializable]
	public class Entity
	{
		#region Fields

		private Vector2 _position;
		private bool _isAlive;
		private bool _isMoving;
		private float _speed;
		private Direction _orientation;
		private float _range;

		#endregion

		#region Constructors

		public Entity()
		{
			_isAlive = true;
			_speed = 1.0f;
			_orientation = Direction.North;
			_range = 4.0f;

			Size = 1.0f;
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

		public float Size { get; protected set; }

		public bool IsAlive
		{
			get
			{
				return _isAlive;
			}
		}

		public bool IsMoving
		{
			get
			{
				return _isMoving;
			}
			set
			{
				_isMoving = value;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
			protected set
			{
				_speed = value;
			}
		}

		public float Range
		{
			get
			{
				return _range;
			}
		}

		/// <summary>
		/// What direction is the entity facing?
		/// </summary>
		public Direction Orientation
		{
			get
			{
				return _orientation;
			}
			set
			{
				_orientation = value;
			}
		}

		#endregion

		#region Methods

		public virtual void Update(Level level, TimeSpan elapsed)
		{
			if (_isMoving)
			{
				Move(level);
			}
		}

		public void MoveTo(Level level, Vector2 newPosition)
		{
			var impactEntities = level.GetEntitiesAt(newPosition);
			if (impactEntities != null)
			{
				foreach (var impactEntity in impactEntities)
				{
					if (impactEntity != this)
					{
						impactEntity.Touched(this);
						//_isMoving = false; // TODO: Is it good to continue moving through an entity like this?  Should the Speed by modified at all?
					}
				}
			}

			if (CanMoveTo(level, newPosition))
			{
				var oldChunk = level.GetChunk(this); // (int)(_position.X + Size / 2), (int)(_position.Y + Size / 2));

				_position = newPosition;

				var newChunk = level.GetChunk(this);

				if (oldChunk != newChunk)
				{
					oldChunk.RemoveEntity(this);
					newChunk.AddEntity(this);
				}
			}
			else
			{
				_isMoving = false;
			}
		}

		public virtual bool ContainsPoint(Vector2 position)
		{
			return Math.Abs((_position - position).Length) <= 0.5;
		}

		public virtual void Touched(Entity touchedBy)
		{
		}

		/// <summary>
		/// Mark this entity to be removed from the world.
		/// </summary>
		public void Die()
		{
			_isAlive = false;
		}

		public bool CanReach(int blockX, int blockY)
		{
			var entityCenter = _position + new Vector2(0.5f, 0.5f);
			var targetCenter = new Vector2(blockX + 0.5f, blockY + 0.5f);
			var distance = Math.Abs((entityCenter - targetCenter).Length);
			return distance <= Range;
		}

		/// <summary>
		/// Is there a block blocking this position?
		/// </summary>
		protected bool CanMoveTo(Level level, Vector2 newPosition)
		{
			// This looks right, and visually it looks mostly right.
			var topLeft = new Vector2(newPosition.X, newPosition.Y);
			var bottomRight = new Vector2(topLeft.X + Size, topLeft.Y + Size);

			return
				!level.IsBlockedAt((int)Math.Floor(topLeft.X), (int)Math.Floor(topLeft.Y)) &&
				!level.IsBlockedAt((int)Math.Floor(topLeft.X), (int)Math.Floor(bottomRight.Y)) &&
				!level.IsBlockedAt((int)Math.Floor(bottomRight.X), (int)Math.Floor(bottomRight.Y)) &&
				!level.IsBlockedAt((int)Math.Floor(bottomRight.X), (int)Math.Floor(topLeft.Y));
		}

		private void MoveBy(Level level, Vector2 delta)
		{
			MoveTo(level, _position + delta);
		}

		private void MoveBy(Level level, Direction direction, float speed)
		{
			var vi = direction.ToVector2I();
			MoveBy(level, new Vector2(vi.X * speed, vi.Y * speed));
		}

		private void Move(Level level, Direction direction)
		{
			// TODO: Modify speed according to the friction value of the current block.
			// TODO: Move slower if traveling on the background layer.
			// TODO: Don't allow walking through a position where all layers, including the background, are empty.
			MoveBy(level, direction, _speed);
		}

		private void Move(Level level)
		{
			Move(level, _orientation);
		}

		#endregion
	}
}
