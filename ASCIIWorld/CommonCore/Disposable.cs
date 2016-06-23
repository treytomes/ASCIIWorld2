using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore
{
	/// <summary>
	/// Base class for members implementing <see cref="IDisposable"/>.
	/// </summary>
	public class Disposable : IDisposable
	{
		#region Fields

		private Subject<Unit> _whenDisposed;

		#endregion

		#region Constructors

		public Disposable()
		{
			IsDisposed = false;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="Disposable"/> class.
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Disposable"/> is reclaimed by garbage collection.
		/// Will run only if the Dispose method does not get called.
		/// 
		/// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
		/// </summary>
		/// <remarks>
		/// The finalizer is used primarily to free unmanaged resources.
		/// </remarks>
		~Disposable()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the when errors changed observable event.  Occurs when the disposed state of the object has changed.
		/// </summary>
		/// <value>
		/// The when disposed changed observable event.
		/// </value>
		public IObservable<Unit> WhenDisposed
		{
			get
			{
				if (IsDisposed)
				{
					return Observable.Return(Unit.Default);
				}
				else
				{
					if (_whenDisposed == null)
					{
						_whenDisposed = new Subject<Unit>();
					}
					return _whenDisposed.AsObservable();
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		/// <remarks>
		/// Also used to detect redundant calls.
		/// </remarks>
		/// <value>
		/// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposed { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Disposed all managed and unmanaged resources.
			Dispose(true);

			// Take this object off the finalization queue and prevent finalization code for this object from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!IsDisposed)
			{
				if (disposing)
				{
					DisposeManaged();
				}

				DisposeUnmanaged();
				IsDisposed = true;

				if (_whenDisposed != null)
				{
					_whenDisposed.OnNext(Unit.Default);
					_whenDisposed.OnCompleted();
					_whenDisposed.Dispose();
					_whenDisposed = null;
				}
			}
		}

		/// <summary>
		/// Disposes the managed resources implementing <see cref="IDisposable"/>.
		/// </summary>
		protected virtual void DisposeManaged()
		{
		}

		/// <summary>
		/// Disposes the unmanaged resources implementing <see cref="IDisposable"/>.
		/// </summary>
		protected virtual void DisposeUnmanaged()
		{

		}

		protected void ThrowIfDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion
	}
}
