using GameCore.Rendering;
using OpenTK;
using System;

namespace ASCIIWorld.Data
{
	[Serializable]
	public class BlockEntity : Entity
	{
		#region Fields

		private int _blockId;
		private float _rotation;

		private bool _isSelected;

		#endregion

		#region Constructors

		public BlockEntity(int blockId)
			: base()
		{
			_blockId = blockId;
			_rotation = 0.0f;
			_isSelected = false;
		}

		#endregion

		#region Properties

		public int BlockID
		{
			get
			{
				return _blockId;
			}
		}

		public float Rotation
		{
			get
			{
				return _rotation;
			}
		}
		
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
		}

		#endregion

		#region Methods

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);
			_rotation += 0.4f;
		}

		public override void Touch(Entity touchedBy)
		{
			base.Touch(touchedBy);

			if (!_isSelected)
			{
				_isSelected = true;
			}
			else
			{
				Die();
			}
		}

		public override string ToString()
		{
			return $"BlockEntity(blockId:={_blockId})";
		}

		#endregion
	}
}
