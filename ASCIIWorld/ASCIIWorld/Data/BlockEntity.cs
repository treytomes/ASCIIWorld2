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
			Speed = 0.4f;
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

		public override void Update(Level level, TimeSpan elapsed)
		{
			base.Update(level, elapsed);
			_rotation += Speed;
		}

		public override void Touched(Entity touchedBy)
		{
			base.Touched(touchedBy);

			//if (!_isSelected)
			//{
			//	_isSelected = true;
			//}
			//else
			//{
			//	Die();
			//}

			if (touchedBy is PlayerEntity)
			{
				Console.WriteLine("touched!");
				(touchedBy as PlayerEntity).ReceiveItem(ItemRegistry.Instance.GetByBlockId(_blockId));
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
