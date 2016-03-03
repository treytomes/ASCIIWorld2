using CommonCore;

namespace GameCore.Rendering.Text
{
	class PoolableTextExtents : TextExtents, IPoolable<PoolableTextExtents>
	{
		ObjectPool<PoolableTextExtents> _owner;

		#region Constructors

		public PoolableTextExtents()
		{
		}

		#endregion

		#region Properties

		ObjectPool<PoolableTextExtents> IPoolable<PoolableTextExtents>.Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}

		#endregion

		#region Methods

		void IPoolable.OnAcquire()
		{
			Clear();
		}

		void IPoolable.OnRelease()
		{
		}

		#endregion
	}
}
