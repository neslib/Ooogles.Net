using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.ES20;

#if __ANDROID__ || __IOS__
using TKUsage = OpenTK.Graphics.ES20.BufferUsage;
#else
using TKUsage = OpenTK.Graphics.ES20.BufferUsageHint;
#endif

namespace Ooogles
{
    /// <summary>
    /// A (vertex) array buffer or element array (index) buffer
    /// </summary>
    public sealed class DataBuffer : GLObject
    {
        /// <summary>
        /// Supported types of <see cref="DataBuffer"/>'s
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A vertex buffer (aka array buffer).
            /// Contains an array of vertices.
            /// </summary>
            Vertex = (int)BufferTarget.ArrayBuffer,

            /// <summary>
            /// An index buffer (aka element array buffer).
            /// Contains an array of indices to vertices.
            /// </summary>
            Index = (int)BufferTarget.ElementArrayBuffer
        }

        /// <summary>
        /// Hints how a <see cref="Data{T}(T[], IntPtr, Usage)"/>'s data is accessed.
        /// </summary>
        public enum Usage
        {
            /// <summary>
            /// The data stored in a buffer will be modified once and used many times.
            /// </summary>
            StaticDraw = (int)TKUsage.StaticDraw,

            /// <summary>
            /// The data stored in a buffer will be modified once and used at most a few times.
            /// </summary>
            StreamDraw = (int)TKUsage.StreamDraw,

            /// <summary>
            /// The data stored in a buffer will be modified repeatedly and used many times.
            /// </summary>
            DynamicDraw = (int)TKUsage.DynamicDraw
        }

        private BufferTarget _type;

        /// <summary>
        /// Checks if this buffer is currently bound.
        /// </summary>
        /// <value>True if this is the currently bound buffer, False otherwise.</value>
        public bool IsBound
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetInteger(glUtils.GetTargetBinding((All)_type), out int currentlyBoundBuffer);
                glUtils.Check(this);
                return (currentlyBoundBuffer == Handle);
            }
        }

        /// <summary>
        /// Get the size of the buffer in bytes.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this buffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetBufferParameteriv(GL_BUFFER_SIZE)</para>
        /// </remarks>
        /// <value>The size of the buffer in bytes.</value>
        /// <exception cref="GLException">InvalidOperation if no buffer is bound.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Data{T}(T[], Usage)"/>
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.GetBufferParameter(_type, BufferParameterName.BufferSize, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the buffer object's usage pattern.
        /// </summary>
        /// <remarks>
        /// <b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this buffer is not bound.
        ///
        /// <para><b>OpenGL API</b>: glGetBufferParameteriv(GL_BUFFER_USAGE)</para>
        /// </remarks>
        /// <value>The usage pattern.</value>
        /// <exception cref="GLException">InvalidOperation if no buffer is bound.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Data{T}(T[], Usage)"/>
        public Usage BufferUsage
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                glUtils.CheckBinding((All)_type, Handle, this);
                GL.GetBufferParameter(_type, BufferParameterName.BufferUsage, out int value);
                glUtils.Check(this);
                return (Usage)value;
            }
        }

        /// <summary>
        /// Gets the currently bound array buffer (aka Vertex Buffer).
        /// </summary>
        /// <remarks>
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_ARRAY_BUFFER_BINDING)</para>
        /// </remarks>
        /// <value>The currently bound array buffer or null if none is bound.</value>
        /// <seealso cref="CurrentIndexBuffer"/>
        public static DataBuffer CurrentVertexBuffer
        {
            get
            {
                GL.GetInteger(GetPName.ArrayBufferBinding, out int value);
                if (value == 0)
                    return null;
                return new DataBuffer(value, BufferTarget.ArrayBuffer);
            }
        }

        /// <summary>
        /// Gets the currently bound element array buffer (aka Index Buffer).
        /// </summary>
        /// <remarks>
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_ELEMENT_ARRAY_BUFFER_BINDING)</para>
        /// </remarks>
        /// <value>The currently bound element array buffer or null if none is bound.</value>
        /// <seealso cref="CurrentVertexBuffer"/>
        public static DataBuffer CurrentIndexBuffer
        {
            get
            {
                GL.GetInteger(GetPName.ElementArrayBufferBinding, out int value);
                if (value == 0)
                    return null;
                return new DataBuffer(value, BufferTarget.ElementArrayBuffer);
            }
        }

        internal DataBuffer(int handle, BufferTarget type)
        {
            Handle = handle;
            _type = type;
        }

        /// <summary>
        /// Creates a buffer.
        /// </summary>
        /// <remarks>
        /// No buffer objects are associated with the buffer until they are first bound by calling <see cref="Bind"/>.
        ///
        /// <para><b>OpenGL API</b>: glGenBuffers</para>
        /// </remarks>
        /// <param name="type">the type of buffer to create.</param>
        /// <seealso cref="Bind"/>
        public DataBuffer(Type type)
        {
            GL.GenBuffers(1, out _handle);
            glUtils.Check(this);
            _type = (BufferTarget)type;
        }

        /// <summary>
        /// Binds the buffer.
        /// </summary>
        /// <remarks>
        /// When a buffer object is bound to a target, the previous binding for that target is automatically broken.
        ///
        /// <para>The state of a buffer object immediately after it is first bound is a zero-sized memory buffer with 
        /// <see cref="BufferUsage.StaticDraw"/> usage.</para>
        ///
        /// <para>While a buffer object name is bound, GL operations on the target to which it is bound affect the bound buffer object, 
        /// and queries of the target to which it is bound return state from the bound buffer object.</para>
        ///
        /// <para>A buffer object binding created <c>Bind</c> remains active until a different buffer object name is bound to the same target, 
        /// or until the bound buffer object is deleted.</para>
        ///
        /// <para>Once created, a named buffer object may be re-bound to any target as often as needed.
        /// However, the GL implementation may make choices about how to optimize the storage of a buffer object based on its initial binding target.</para>
        ///
        /// <para><b>OpenGL API</b>: glBindBuffer</para>
        /// </remarks>
        /// <seealso cref="CurrentVertexBuffer"/>
        /// <seealso cref="CurrentIndexBuffer"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="IsBound"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind()
        {
            GL.BindBuffer(_type, Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Unbinds the buffer.
        /// </summary>
        /// <remarks>
        /// This effectively unbinds any buffer object previously bound, and restores client memory usage for that buffer object target.
        ///
        /// <para>While the buffer is unbound, as in the initial state, attempts to modify or query state on the target to which it is bound 
        /// generates an <see cref="GLException"/> with code <see cref="gl.Error.InvalidOperation"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glBindBuffer</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        /// <seealso cref="IsBound"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unbind()
        {
            GL.BindBuffer(_type, 0);
            glUtils.Check(this);
        }

        /// <summary>
        /// Create and initialize a buffer object's data store.
        /// </summary>
        /// <remarks>
        /// Any pre-existing data store is deleted.
        ///
        /// <para><paramref name="usage"/> is a hint to the GL implementation as to how a buffer object's data store will be accessed.
        /// This enables the GL implementation to make more intelligent decisions that may significantly impact buffer object performance.
        /// It does not, however, constrain the actual usage of the data store.</para>
        ///
        /// <para><b>Note</b>: clients must align data elements consistent with the requirements of the client platform, 
        /// with an additional base-level requirement that an offset within a buffer to a datum comprising N be a multiple of N.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this buffer is not bound.</para>
        ///
        /// <para><b>Note</b>: it is more efficient to use the overload <see cref="Data{T}(T[], IntPtr, Usage)"/> instead.</para>
        ///
        /// <para><b>OpenGL API</b>: glBufferData</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="data">data array to copy to the buffer.</param>
        /// <param name="usage">(optional) expected usage pattern of the data store. Defaults to <see cref="BufferUsage.StaticDraw"/>.</param>
        /// <exception cref="GLException">InvalidOperation if no buffer is bound.</exception>
        /// <exception cref="GLException">OutOfMemory if the GL is unable to create a data store.</exception>
        /// <seealso cref="Size"/>
        /// <seealso cref="BufferUsage"/>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="SubData{T}(IntPtr, T[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Data<T>(T[] data, Usage usage = Usage.StaticDraw) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
            GL.BufferData(_type, (IntPtr)(data.Length * Marshal.SizeOf<T>()), data, (TKUsage)usage);
            glUtils.Check(this);
        }

        /// <summary>
        /// Create and initialize a buffer object's data store.
        /// </summary>
        /// <remarks>
        /// Any pre-existing data store is deleted.
        ///
        /// <para><paramref name="usage"/> is a hint to the GL implementation as to how a buffer object's data store will be accessed.
        /// This enables the GL implementation to make more intelligent decisions that may significantly impact buffer object performance.
        /// It does not, however, constrain the actual usage of the data store.</para>
        ///
        /// <para><b>Note</b>: clients must align data elements consistent with the requirements of the client platform, 
        /// with an additional base-level requirement that an offset within a buffer to a datum comprising N be a multiple of N.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this buffer is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glBufferData</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="data">data array to copy to the buffer.</param>
        /// <param name="size">size of the data in bytes.</param>
        /// <param name="usage">(optional) expected usage pattern of the data store. Defaults to <see cref="BufferUsage.StaticDraw"/>.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="size"/> is negative.</exception>
        /// <exception cref="GLException">InvalidOperation if no buffer is bound.</exception>
        /// <exception cref="GLException">OutOfMemory if the GL is unable to create a data store with the specified <paramref name="size"/>.</exception>
        /// <seealso cref="Size"/>
        /// <seealso cref="BufferUsage"/>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="SubData{T}(IntPtr, T[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Data<T>(T[] data, IntPtr size, Usage usage = Usage.StaticDraw) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
            GL.BufferData(_type, size, data, (TKUsage)usage);
            glUtils.Check(this);
        }

        /// <summary>
        /// Update a subset of a buffer object's data store.
        /// </summary>
        /// <remarks>
        /// This method redefines some or all of the data store for the buffer object currently bound to target.
        /// data starting at byte offset <paramref name="offset"/> is copied to the data store from the memory pointed to by <paramref name="data"/>.
        /// An error is thrown if offset and size together define a range beyond the bounds of the buffer object's data store.
        ///
        /// <para><b>Note</b>: when replacing the entire data store, consider using SubData rather than completely recreating the data store with <see cref="Data{T}(T[], IntPtr, Usage)"/>.
        /// This avoids the cost of reallocating the data store.</para>
        ///
        /// <para><b>Note</b>: consider using multiple buffer objects to avoid stalling the rendering pipeline during data store updates.
        /// If any rendering in the pipeline makes reference to data in the buffer object being updated by SubData, 
        /// especially from the specific region being updated, that rendering must drain from the pipeline before the data store can be updated.</para>
        ///
        /// <para><b>Note</b>: clients must align data elements consistent with the requirements of the client platform, 
        /// with an additional base-level requirement that an offset within a buffer to a datum comprising N be a multiple of N.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this buffer is not bound.</para>
        ///
        /// <para><b>Note</b>: it is more efficient to use the overload <see cref="SubData{T}(IntPtr, T[], IntPtr)"/> instead.</para>
        ///
        /// <para><b>OpenGL API</b>: glBufferSubData</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="offset">the offset into the buffer object's data store where data replacement will begin, measured in bytes.</param>
        /// <param name="data">data array to copy to the buffer.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="offset"/> is negative, or if together with the size of the data,
        /// they define a region of memory that extends beyond the buffer object's allocated data store.</exception>
        /// <exception cref="GLException">InvalidOperation if no buffer is bound.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="Data{T}(T[], Usage)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubData<T>(IntPtr offset, T[] data) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
            GL.BufferSubData(_type, offset, (IntPtr)(data.Length * Marshal.SizeOf<T>()), data);
            glUtils.Check(this);
        }

        /// <summary>
        /// Update a subset of a buffer object's data store.
        /// </summary>
        /// <remarks>
        /// This method redefines some or all of the data store for the buffer object currently bound to target.
        /// data starting at byte offset <paramref name="offset"/> and extending for <paramref name="size"/> bytes 
        /// is copied to the data store from the memory pointed to by <paramref name="data"/>.
        /// An error is thrown if offset and size together define a range beyond the bounds of the buffer object's data store.
        ///
        /// <para><b>Note</b>: when replacing the entire data store, consider using SubData rather than completely recreating the data store with <see cref="Data{T}(T[], IntPtr, Usage)"/>.
        /// This avoids the cost of reallocating the data store.</para>
        ///
        /// <para><b>Note</b>: consider using multiple buffer objects to avoid stalling the rendering pipeline during data store updates.
        /// If any rendering in the pipeline makes reference to data in the buffer object being updated by SubData, 
        /// especially from the specific region being updated, that rendering must drain from the pipeline before the data store can be updated.</para>
        ///
        /// <para><b>Note</b>: clients must align data elements consistent with the requirements of the client platform, 
        /// with an additional base-level requirement that an offset within a buffer to a datum comprising N be a multiple of N.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if this buffer is not bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glBufferSubData</para>
        /// </remarks>
        /// <typeparam name="T">the type of data in the <paramref name="data"/> array</typeparam>
        /// <param name="offset">the offset into the buffer object's data store where data replacement will begin, measured in bytes.</param>
        /// <param name="data">data array to copy to the buffer.</param>
        /// <param name="size">the number of bytes in the data array.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="offset"/> or <paramref name="size"/> is negative, 
        /// or if together they define a region of memory that extends beyond the buffer object's allocated data store.</exception>
        /// <exception cref="GLException">InvalidOperation if no buffer is bound.</exception>
        /// <seealso cref="Bind"/>
        /// <seealso cref="Unbind"/>
        /// <seealso cref="Data{T}(T[], Usage)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubData<T>(IntPtr offset, T[] data, IntPtr size) where T : struct
        {
            glUtils.CheckBinding((All)_type, Handle, this);
            GL.BufferSubData(_type, offset, size, data);
            glUtils.Check(this);
        }

        /// <summary>
        /// Deletes the buffer.
        /// </summary>
        /// <remarks>
        /// If a buffer object that is currently bound is deleted, the binding reverts to 0 
        /// (the absence of any buffer object, which reverts to client memory usage).
        ///
        /// <para><b>OpenGL API</b>: glDeleteBuffers</para>
        /// </remarks>
        /// <seealso cref="Bind"/>
        protected override void DisposeHandle()
        {
            GL.DeleteBuffers(1, ref _handle);
            glUtils.Check(this);
        }
    }
}
