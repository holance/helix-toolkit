using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EntityComponent : IDisposable, INotifyPropertyChanged, IGUID
    {
        private sealed class DisposeHelper : DisposeObject
        {
        }

        private readonly Lazy<DisposeHelper> disposer = new Lazy<DisposeHelper>(() => new DisposeHelper(), false);

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public Entity Entity { get; internal set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is attached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is attached; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttached { private set; get; } = false;
        /// <summary>
        /// Attaches this instance.
        /// </summary>
        public void Attach()
        {
            if (!IsAttached)
            {
                OnAttach();
                IsAttached = true;
            }
        }

        protected abstract void OnAttach();

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            if (IsAttached)
            {
                IsAttached = false;
                OnDetach();
                DisposeAndClear();
            }
        }

        protected abstract void OnDetach();
        #region Disposible Helpers

        /// <summary>
        /// Adds a <see cref="IDisposable"/> object or a <see cref="IntPtr"/> allocated using <see cref="global::SharpDX.Utilities.AllocateMemory"/> to the list of the objects to dispose.
        /// </summary>
        /// <param name="toDispose">To dispose.</param>
        /// <exception cref="ArgumentException">If toDispose argument is not IDisposable or a valid memory pointer allocated by <see cref="global::SharpDX.Utilities.AllocateMemory"/></exception>
        public T Collect<T>(T toDispose) where T : IDisposable
        {
            return disposer.Value.Collect(toDispose);
        }

        /// <summary>
        /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
        /// </summary>
        /// <param name="objectToDispose">Object to dispose.</param>
        public void RemoveAndDispose<T>(ref T objectToDispose)
        {
            if (disposer.IsValueCreated)
            {
                disposer.Value.RemoveAndDispose(ref objectToDispose);
            }
        }

        /// <summary>
        /// Only disposes the and clear all internal resources.
        /// </summary>
        public void DisposeAndClear()
        {
            if (disposer.IsValueCreated)
            {
                disposer.Value.DisposeAndClear();
            }
        }

        #region IDisposible

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ReferenceCountDisposeObject"/> is reclaimed by garbage collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        ~EntityComponent()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        public void Dispose()
        {
            Dispose(true);
        }

        private int refCounter = 1;

        internal int IncRef()
        {
            return Interlocked.Increment(ref refCounter);
        }
        /// <summary>
        /// Forces the dispose.
        /// </summary>
        internal void ForceDispose()
        {
            Interlocked.Exchange(ref refCounter, 1);
            Dispose();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // TODO Should we throw an exception if this method is called more than once?
            if (Interlocked.Decrement(ref refCounter) == 0 && !IsDisposed)
            {
                Disposing?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);

                OnDispose(disposing);
                if (disposer.IsValueCreated)
                {
                    disposer.Value.Dispose();
                }
                GC.SuppressFinalize(this);

                IsDisposed = true;

                Disposed?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
                Disposing = null;
                Disposed = null;
            }
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void OnDispose(bool disposeManagedResources)
        {
        }

        /// <summary>
        /// Occurs when this instance is starting to be disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposing;

        /// <summary>
        /// Occurs when this instance is fully disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposed;
        #endregion
        #endregion Disposible Helpers
        #region INotifyPropertyChanged
        private bool disablePropertyChangedEvent = false;
        /// <summary>
        /// Disable property changed event calling
        /// </summary>
        public bool DisablePropertyChangedEvent
        {
            set
            {
                if (disablePropertyChangedEvent == value)
                {
                    return;
                }
                disablePropertyChangedEvent = value;
                RaisePropertyChanged();
            }
            get
            {
                return disablePropertyChangedEvent;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!DisablePropertyChangedEvent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="raisePropertyChanged"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            if (raisePropertyChanged)
            { this.RaisePropertyChanged(propertyName); }
            return true;
        }
        #endregion
    }
}
