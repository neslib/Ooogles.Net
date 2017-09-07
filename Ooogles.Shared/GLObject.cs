using System;

namespace Ooogles
{
    /// <summary>
    /// Abstract base class for OpenGL objects
    /// </summary>
    public abstract class GLObject : IDisposable
    {
        /// <summary>
        /// Internal OpenGL handle
        /// </summary>
        protected int _handle;

        /// <summary>
        /// Internal OpenGL handle to the object
        /// </summary>
        public int Handle
        {
            get => _handle;
            protected set => _handle = value;
        }

        #region IDisposable Support

        /// <summary>
        /// Disposes of the OpenGL object
        /// </summary>
        /// <param name="disposing">Whether this method is called from the <see cref="Dispose()"/> method or not.</param>
        protected void Dispose(bool disposing)
        {
            /*if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }*/

            if (Handle != 0)
            {
                DisposeHandle();
                Handle = 0;
            }
        }

        /// <summary>
        /// Finalizes the object
        /// </summary>
        ~GLObject() {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the OpenGL object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Equality
        /// <summary>
        /// Checks if this object matches another object.
        /// </summary>
        /// <remarks>
        /// Two OpenGL objects are considered equal if their internal <see cref="Handle"/> properties are equal.
        /// </remarks>
        /// <param name="obj">The object to compare to</param>
        /// <returns>Whether this object matches <paramref name="obj"/></returns>
        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is GLObject))
            {
                return (Handle == ((GLObject)obj).Handle);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        /// <remarks>
        /// The hash code is equal to the internal <see cref="Handle"/> property of this object.
        /// </remarks>
        /// <returns>The hash code</returns>
        public override int GetHashCode() => Handle;
        #endregion

        /// <summary>
        /// Abstract method that must be overridden to dispose of the internal OpenGL <see cref="Handle"/>.
        /// </summary>
        protected abstract void DisposeHandle();
    }
}
