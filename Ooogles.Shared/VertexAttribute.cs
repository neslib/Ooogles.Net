using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace Ooogles
{
    /// <summary>
    /// Represents a single vertex attribute in a <see cref="Program"/>.
    /// </summary>
    /// <remarks>
    /// These are variables marked with <c>attribute</c> in a vertex shader.
    /// </remarks>
    public struct VertexAttribute
    {
        /// <summary>
        /// Supported data types for vertex attributes
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// 8-bit signed integer.
            /// Corresponds to C#'s sbyte type.
            /// </summary>
            Byte = (int)VertexAttribPointerType.Byte,

            /// <summary>
            /// 8-bit unsigned integer.
            /// Corresponds to C#'s byte type.
            /// </summary>
            UnsignedByte = (int)VertexAttribPointerType.UnsignedByte,

            /// <summary>
            /// 16-bit signed integer.
            /// Corresponds to C#'s short type.
            /// </summary>
            Short = (int)VertexAttribPointerType.Short,

            /// <summary>
            /// 16-bit unsigned integer.
            /// Corresponds to C#'s ushort type.
            /// </summary>
            UnsignedShort = (int)VertexAttribPointerType.UnsignedShort,

            /// <summary>
            /// 32-bit floating-point value.
            /// Corresponds to C#'s float type.
            /// </summary>
            Float = (int)VertexAttribPointerType.Float,

            /// <summary>
            /// Fixed type
            /// </summary>
            Fixed = (int)VertexAttribPointerType.Fixed
        }

        private int _location;

        /// <summary>
        /// When the vertex attribute is used with client data (that is, when <see cref="SetData"/> is used), 
        /// then this method returns a pointer to the client data.
        /// </summary>
        /// <remarks>
        /// This equals the AData parameter passed to <see cref="SetData"/>.
        ///
        /// <para>When the vertex attribute is used with a data buffer (that is, when <see cref="SetConfig"/> is used), 
        /// then the returned value is undefined.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetVertexAttribPointerv</para>
        /// </remarks>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        public int Location => _location;

        /// <summary>
        /// Get whether the vertex attribute is enabled or not.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetVertexAttribiv(GL_VERTEX_ATTRIB_ARRAY_ENABLED)
        /// </remarks>
        /// <value>True if the vertex attribute is enabled. False otherwise.</value>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        public bool IsEnabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetVertexAttrib(_location, VertexAttribParameter.VertexAttribArrayEnabled, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get whether data for the vertex attribute is normalized when converted to floating-point.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetVertexAttribiv(GL_VERTEX_ATTRIB_ARRAY_NORMALIZED)
        /// </remarks>
        /// <returns>True if vertex data will be normalized. False otherwise.</returns>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="VertexDataType"/>
        public bool IsNormalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetVertexAttrib(_location, VertexAttribParameter.VertexAttribArrayNormalized, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get the size of the vertex attribute.
        /// </summary>
        /// <remarks>
        /// The size is the number of values for each element of the vertex attribute array, and it will be 1, 2, 3, or 4.
        /// The initial value is 4.
        ///
        /// <para><b>OpenGL API</b>: glGetVertexAttribiv(GL_VERTEX_ATTRIB_ARRAY_SIZE)</para>
        /// </remarks>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="IsNormalized"/>
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetVertexAttrib(_location, VertexAttribParameter.VertexAttribArraySize, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the array stride for (number of bytes between successive elements in) the vertex attribute.
        /// </summary>
        /// <remarks>
        /// A value of 0 indicates that the array elements are stored sequentially in memory.
        /// The initial value is 0.
        ///
        /// <para><b>OpenGL API</b>: glGetVertexAttribiv(GL_VERTEX_ATTRIB_ARRAY_STRIDE)</para>
        /// </remarks>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="IsNormalized"/>
        public int Stride
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetVertexAttrib(_location, VertexAttribParameter.VertexAttribArrayStride, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the current value for the generic vertex attribute.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetVertexAttribfv(GL_CURRENT_VERTEX_ATTRIB)
        /// </remarks>
        public Vector4 Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                float[] values = new float[4];
                GL.GetVertexAttrib(_location, VertexAttribParameter.CurrentVertexAttrib, values);
                glUtils.Check(this);
                return new Vector4(values[0], values[1], values[2], values[3]);
            }
        }

        /// <summary>
        /// Get the data type of the vertex attribute.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetVertexAttribiv(GL_VERTEX_ATTRIB_ARRAY_TYPE)
        /// </remarks>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="IsNormalized"/>
        public DataType VertexDataType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetVertexAttrib(_location, VertexAttribParameter.VertexAttribArrayType, out int value);
                glUtils.Check(this);
                return (DataType)value;
            }
        }

        /// <summary>
        /// Get the buffer object currently bound to the binding point corresponding to the generic vertex attribute.
        /// </summary>
        /// <remarks>
        /// If no buffer object is bound, then null is returned.
        ///
        /// <para><b>OpenGL API</b>: glGetVertexAttribiv(GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING)</para>
        /// </remarks>
        /// <seealso cref="SetConfig"/>
        public DataBuffer Buffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                GL.GetVertexAttrib(_location, VertexAttribParameter.VertexAttribArrayBufferBinding, out int bufferHandle);
                glUtils.Check(this);
                if (bufferHandle == 0)
                    return null;
                return new DataBuffer(bufferHandle, BufferTarget.ArrayBuffer);
            }
        }

        /// <summary>
        /// Creates a vertex attribute.
        /// </summary>
        /// <remarks>
        /// Queries the previously linked program object specified by <paramref name="program"/> for the attribute variable specified by 
        /// <paramref name="attributeName"/> and initializes the attribute with the index of the generic vertex attribute that is bound to that attribute variable.
        /// If <paramref name="attributeName"/> is a matrix attribute variable, the index of the first column of the matrix is used.
        ///
        /// <para>The association between an attribute variable name and a generic attribute index can be specified 
        /// by using the other constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// Attribute bindings do not go into effect until <see cref="Program.Link"/> is called.
        /// After a program object has been linked successfully, the index values for attribute variables remain fixed until the next link command occurs.
        /// The attribute values can only be queried after a link if the link was successful.</para>
        ///
        /// <para>This method uses the binding that actually went into effect the last time <see cref="Program.Link"/> was called for the specified program object.
        /// Attribute bindings that have been specified since the last link operation are not used.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode, a warning will be logged to the debug console if <paramref name="program"/> 
        /// does not contain an attribute named <paramref name="attributeName"/>, or <paramref name="attributeName"/> 
        /// starts with the reserved prefix 'gl_'.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetAttribLocation</para>
        /// </remarks>
        /// <param name="program">the program containing the attribute.</param>
        /// <param name="attributeName">the (case-sensitive) name of the attribute as used in the vertex shader of the program.</param>
        /// <exception cref="GLException">InvalidOperation if <paramref name="program"/> is not a valid program or has not been successfully linked.</exception>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="VertexAttribute(Program, int, string)"/>
        public VertexAttribute(Program program, string attributeName)
        {
            Debug.Assert(program != null);
            _location = GL.GetAttribLocation(program.Handle, attributeName);
            glUtils.Check(this);

#if DEBUG
            if (_location < 0)
                Debug.WriteLine($"Unable to get location for attribute {attributeName}");
#endif
        }

        /// <summary>
        /// Creates a vertex attribute by binding a user-defined attribute name to an attribute location.
        /// </summary>
        /// <remarks>
        /// This method is used to associate a user-defined attribute variable in the program object specified by <paramref name="program"/> 
        /// with a generic vertex attribute index.
        /// The name of the user-defined attribute variable is passed in <paramref name="attributeName"/>.
        /// The generic vertex attribute index to be bound to this variable is specified by <paramref name="location"/>.
        /// When <paramref name="program"/> is made part of current state, values provided via the generic vertex attribute index
        /// will modify the value of the user-defined attribute variable specified by <paramref name="attributeName"/>.
        ///
        /// <para>If <paramref name="attributeName"/> refers to a matrix attribute variable, 
        /// <paramref name="location"/> refers to the first column of the matrix.
        /// Other matrix columns are then automatically bound to locations <paramref name="location"/>+1 for a matrix of type Matrix2; 
        /// <paramref name="location"/>+1 and <paramref name="location"/>+2 for a matrix of type Matrix3; 
        /// and <paramref name="location"/>+1, <paramref name="location"/>+2, and <paramref name="location"/>+3 for a matrix of type Matrix4.</para>
        ///
        /// <para>This method makes it possible for vertex shaders to use descriptive names for attribute variables
        /// rather than generic numbered variables.
        /// The values sent to each generic attribute index are part of current state, 
        /// just like standard vertex attributes such as color, normal, and vertex position.
        /// If a different program object is made current by calling <see cref="Program.Use"/>, 
        /// the generic vertex attributes are tracked in such a way that the same values will be observed by attributes
        /// in the new program object that are also bound to <paramref name="location"/>.</para>
        ///
        /// <para>Attribute bindings do not go into effect until <see cref="Program.Link"/> is called.
        /// After a program object has been linked successfully, the index values for generic attributes remain fixed
        /// (and their values can be queried) until the next link command occurs.</para>
        ///
        /// <para>Applications are not allowed to bind any of the standard OpenGL vertex attributes using this command, 
        /// as they are bound automatically when needed.
        /// Any attribute binding that occurs after the program object has been linked will not take effect 
        /// until the next time the program object is linked.</para>
        ///
        /// <para><b>Note</b>: this constructor can be called before any vertex shader objects are bound to the specified program object.
        /// It is also permissible to bind a generic attribute index to an attribute variable name that is never used in a vertex shader.</para>
        ///
        /// <para><b>Note</b>: if <paramref name="attributeName"/> was bound previously, that information is lost.
        /// Thus you cannot bind one user-defined attribute variable to multiple indices, 
        /// but you can bind multiple user-defined attribute variables to the same index.</para>
        ///
        /// <para><b>Note</b>: applications are allowed to bind more than one user-defined attribute variable 
        /// to the same generic vertex attribute index.
        /// This is called aliasing, and it is allowed only if just one of the aliased attributes is active in the executable program, 
        /// or if no path through the shader consumes more than one attribute of a set of attributes aliased to the same location.
        /// The compiler and linker are allowed to assume that no aliasing is done and are free to employ optimizations 
        /// that work only in the absence of aliasing.
        /// OpenGL implementations are not required to do error checking to detect aliasing.
        /// Because there is no way to bind standard attributes, it is not possible to alias generic attributes with conventional ones
        /// (except for generic attribute 0).</para>
        ///
        /// <para><b>Note</b>: active attributes that are not explicitly bound will be bound by the linker when <see cref="Program.Link"/> is called.
        /// The locations assigned can be queried by using the other constructor <see cref="VertexAttribute(Program, string)"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glBindAttribLocation</para>
        /// </remarks>
        /// <param name="program">the program containing the attribute.</param>
        /// <param name="location">the location of the attribute to be bound</param>
        /// <param name="attributeName">the (case-sensitive) name of the vertex shader attribute to which <paramref name="location"/> is to be bound.</param>
        /// <exception cref="GLException">InvalidOperation if <paramref name="program"/> is not a valid program.</exception>
        /// <exception cref="GLException">InvalidOperation if <paramref name="attributeName"/> starts with the reserved prefix 'gl_'.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="location"/> is greater than or equal to the maximum number of supported vertex attributes.</exception>
        /// <seealso cref="Shader.MaxVertexAttribs"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="DataType"/>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        /// <seealso cref="VertexAttribute(Program, string)"/>
        public VertexAttribute(Program program, int location, string attributeName)
        {
            Debug.Assert(program != null);
            _location = location;
            GL.BindAttribLocation(program.Handle, location, attributeName);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets up the attribute for use with a <see cref="DataBuffer"/> (of type <see cref="DataBuffer.Type.Vertex"/>).
        /// </summary>
        /// <remarks>
        /// That is, this attribute will be part of a VBO (Vertex Buffer Object).
        /// When this attribute is <b>not</b> part of a VBO, then you should use <see cref="SetData"/> instead.
        ///
        /// <para>There is an overloaded version <see cref="SetConfig{T}(int, int)"/> that has a type parameter {T} 
        /// instead of the <paramref name="dataType"/> and <paramref name="numValuesPerVertex"/> parameters.
        /// That version is less efficient though.</para>
        /// 
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if no <see cref="DataBuffer"/> of type <see cref="DataBuffer.Type.Vertex"/> is bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttribPointer</para>
        /// </remarks>
        /// <param name="dataType">the data type of the each component of the vertex attribute. 
        /// For example, an attribute of type <c>vec3</c> contains components of type <see cref="DataType.Float"/>.</param>
        /// <param name="numValuesPerVertex">the number of values (components) of the vertex attribute. 
        /// For example, an attribute of type <c>vec3</c> contains 3 values. Valid values are 1, 2, 3 and 4.</param>
        /// <param name="stride">(optional) byte offset between consecutive vertex attributes. 
        /// If stride is 0 (the default), the generic vertex attributes are understood to be tightly packed in the array.</param>
        /// <param name="offset">(optional) byte offset into the buffer object's data store. Defaults to 0.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="numValuesPerVertex"/> is not 1, 2, 3 or 4.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="stride"/> is negative.</exception>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="IsNormalized"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="DataBuffer.Bind"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConfig(DataType dataType, int numValuesPerVertex, int stride = 0, int offset = 0)
        {
            glUtils.CheckBinding(All.ArrayBuffer, this);
            GL.VertexAttribPointer(_location, numValuesPerVertex, (VertexAttribPointerType)dataType, false, stride, (IntPtr)offset);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets up the attribute for use with a <see cref="DataBuffer"/> (of type <see cref="DataBuffer.Type.Vertex"/>).
        /// </summary>
        /// <remarks>
        /// That is, this attribute will be part of a VBO (Vertex Buffer Object).
        /// When this attribute is <b>not</b> part of a VBO, then you should use <see cref="SetData"/> instead.
        ///
        /// <para>Type parameter {T} must be of an integral or floating-point type, or of type Vector2, Vector3 or Vector4.</para>
        /// 
        /// <para>The overloaded version <see cref="SetConfig(DataType, int, int, int)"/> is more efficient.</para>
        /// 
        /// <para><b>Note</b>: in DEBUG mode with assertions enabled, an error will be logged to the debug console if no <see cref="DataBuffer"/> of type <see cref="DataBuffer.Type.Vertex"/> is bound.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttribPointer</para>
        /// </remarks>
        /// <typeparam name="T">The type to use for this vertex attribute.</typeparam>
        /// <param name="stride">(optional) byte offset between consecutive vertex attributes. 
        /// If stride is 0 (the default), the generic vertex attributes are understood to be tightly packed in the array.</param>
        /// <param name="offset">(optional) byte offset into the buffer object's data store. Defaults to 0.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="stride"/> is negative.</exception>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="IsNormalized"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="DataBuffer.Bind"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        /// <seealso cref="SetData"/>
        public void SetConfig<T>(int stride = 0, int offset = 0) where T : struct
        {
            GetTypeInfo<T>(out DataType dataType, out int numValuesPerVertex);
            SetConfig(dataType, numValuesPerVertex, stride, offset);
        }

        /// <summary>
        /// Sets up the attribute for use with client data.
        /// </summary>
        /// <remarks>
        /// When this attribute is part of a VBO, then you should use <see cref="SetConfig"/> instead
        /// (which is more efficient than using client data).
        ///
        /// <para>There is an overloaded version of this method <see cref="SetData{T}(T[], int, bool)"/> where <paramref name="data"/>
        /// is an array of values of generic type T.
        /// That version is less efficient though.</para>
        ///
        /// <para>  <b>OpenGL API</b>: glVertexAttribPointer</para>
        /// </remarks>
        /// <param name="dataType">the data type of the each component of the vertex attribute. For example, an attribute of type Vector3 contains components of type <see cref="DataType.Float"/>.</param>
        /// <param name="numValuesPerVertex">the number of values (components) of the vertex attribute. For example, an attribute of type Vector3 contains 3 values. Valid values are 1, 2, 3 and 4.</param>
        /// <param name="data">pointer to the client data containing the vertices.</param>
        /// <param name="stride">(optional) byte offset between consecutive vertex attributes. 
        /// If stride is 0 (the default), the generic vertex attributes are understood to be tightly packed in the array.</param>
        /// <param name="normalized">(optional) flag that specifies whether integer values will be normalized. 
        /// If set to True, integer values are be mapped to the range [-1,1] (for signed values) or [0,1] (for unsigned values) when they are accessed and converted to floating point.
        /// Otherwise (default), values will be converted to floats directly without normalization.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="numValuesPerVertex"/> is not 1, 2, 3 or 4.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="stride"/> is negative.</exception>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="IsNormalized"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        /// <seealso cref="SetConfig"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetData(DataType dataType, int numValuesPerVertex, void* data, int stride = 0, bool normalized = false) 
        {
            GL.VertexAttribPointer(_location, numValuesPerVertex, (VertexAttribPointerType)dataType, normalized, stride, (IntPtr)data);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets up the attribute for use with client data.
        /// </summary>
        /// <remarks>
        /// When this attribute is part of a VBO, then you should use <see cref="SetConfig"/> instead
        /// (which is more efficient than using client data).
        ///
        /// <para>There is an overloaded version of this method <see cref="SetData{T}(T[], int, bool)"/> where <paramref name="data"/>
        /// is an array of values of generic type T.
        /// That version is less efficient though.</para>
        ///
        /// <para>  <b>OpenGL API</b>: glVertexAttribPointer</para>
        /// </remarks>
        /// <typeparam name="T">The type of elements in the <paramref name="data"/> array.</typeparam>
        /// <param name="dataType">the data type of the each component of the vertex attribute. For example, an attribute of type Vector3 contains components of type <see cref="DataType.Float"/>.</param>
        /// <param name="numValuesPerVertex">the number of values (components) of the vertex attribute. For example, an attribute of type Vector3 contains 3 values. Valid values are 1, 2, 3 and 4.</param>
        /// <param name="data">client data array containing the vertices.</param>
        /// <param name="stride">(optional) byte offset between consecutive vertex attributes. 
        /// If stride is 0 (the default), the generic vertex attributes are understood to be tightly packed in the array.</param>
        /// <param name="normalized">(optional) flag that specifies whether integer values will be normalized. 
        /// If set to True, integer values are be mapped to the range [-1,1] (for signed values) or [0,1] (for unsigned values) when they are accessed and converted to floating point.
        /// Otherwise (default), values will be converted to floats directly without normalization.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="numValuesPerVertex"/> is not 1, 2, 3 or 4.</exception>
        /// <exception cref="GLException">InvalidValue if <paramref name="stride"/> is negative.</exception>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="IsNormalized"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        /// <seealso cref="SetConfig"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetData<T>(DataType dataType, int numValuesPerVertex, T[] data, int stride = 0, bool normalized = false) where T : struct
        {
            GL.VertexAttribPointer(_location, numValuesPerVertex, (VertexAttribPointerType)dataType, normalized, stride, data);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets up the attribute for use with client data.
        /// </summary>
        /// <remarks>
        /// When this attribute is part of a VBO, then you should use <see cref="SetConfig"/> instead
        /// (which is more efficient than using client data).
        ///
        /// <para>Type parameter {T} must be of an integral or floating-point type, or of type Vector2, Vector3 or Vector4.</para>
        ///
        /// <para>The other overloaded versions (<see cref="SetConfig(DataType, int, int, int)"/> and <see cref="SetData{T}(DataType, int, T[], int, bool)"/>) are more efficient.</para>
        /// 
        /// <para><b>OpenGL API</b>: glVertexAttribPointer</para>
        /// </remarks>
        /// <typeparam name="T">The type of elements in the <paramref name="data"/> array.</typeparam>
        /// <param name="data">client data array containing the vertices.</param>
        /// <param name="stride">(optional) byte offset between consecutive vertex attributes. 
        /// If stride is 0 (the default), the generic vertex attributes are understood to be tightly packed in the array.</param>
        /// <param name="normalized">(optional) flag that specifies whether integer values will be normalized. 
        /// If set to True, integer values are be mapped to the range [-1,1] (for signed values) or [0,1] (for unsigned values) when they are accessed and converted to floating point.
        /// Otherwise (default), values will be converted to floats directly without normalization.</param>
        /// <exception cref="GLException">InvalidValue if <paramref name="stride"/> is negative.</exception>
        /// <seealso cref="Enable"/>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="VertexDataType"/>
        /// <seealso cref="Size"/>
        /// <seealso cref="IsNormalized"/>
        /// <seealso cref="Stride"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        /// <seealso cref="SetConfig"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetData<T>(T[] data, int stride = 0, bool normalized = false) where T : struct
        {
            GetTypeInfo<T>(out DataType dataType, out int numValuesPerVertex);
            GL.VertexAttribPointer(_location, numValuesPerVertex, (VertexAttribPointerType)dataType, normalized, stride, data);
            glUtils.Check(this);
        }

        /// <summary>
        /// Enables this vertex attribute.
        /// </summary>
        /// <remarks>
        /// If enabled, the values in the generic vertex attribute array will be accessed and used for rendering when calls are made to 
        /// vertex array commands such as <see cref="gl.DrawArrays(gl.PrimitiveType, int)"/> or <see cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>.
        ///
        /// <para><b>OpenGL API</b>: glEnableVertexAttribArray</para>
        /// </remarks>
        /// <seealso cref="Disable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enable()
        {
            GL.EnableVertexAttribArray(_location);
            glUtils.Check(this);
        }

        /// <summary>
        /// Disables this vertex attribute.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glDisableVertexAttribArray
        /// </remarks>
        /// <seealso cref="Enable"/>
        /// <seealso cref="IsEnabled"/>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        /// <seealso cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// <seealso cref="gl.DrawElements(gl.PrimitiveType, byte[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disable()
        {
            GL.DisableVertexAttribArray(_location);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib1</para>
        /// </remarks>
        /// <param name="value">The value to set. The 2nd and 3rd components will be set to 0, and the 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value)
        {
            GL.VertexAttrib1(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib2</para>
        /// </remarks>
        /// <param name="value0">The first component value to set.</param>
        /// <param name="value1">The second component value to set. The 3rd component will be set to 0, and the 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value0, float value1)
        {
            GL.VertexAttrib2(_location, value0, value1);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib3</para>
        /// </remarks>
        /// <param name="value0">The first component value to set.</param>
        /// <param name="value1">The second component value to set.</param>
        /// <param name="value2">The third component value to set. The 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value0, float value1, float value2)
        {
            GL.VertexAttrib3(_location, value0, value1, value2);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib4</para>
        /// </remarks>
        /// <param name="value0">The first component value to set.</param>
        /// <param name="value1">The second component value to set.</param>
        /// <param name="value2">The third component value to set.</param>
        /// <param name="value3">The fourth component value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value0, float value1, float value2, float value3)
        {
            GL.VertexAttrib4(_location, value0, value1, value2, value3);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Vector2
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib2</para>
        /// </remarks>
        /// <param name="value">The value to set. The 3rd component will be set to 0, and the 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Vector2 value)
        {
            GL.VertexAttrib2(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Vector2
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib2</para>
        /// </remarks>
        /// <param name="value">The value to set. The 3rd component will be set to 0, and the 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Vector2 value)
        {
            GL.VertexAttrib2(_location, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Vector3
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib3</para>
        /// </remarks>
        /// <param name="value">The value to set. The 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Vector3 value)
        {
            GL.VertexAttrib3(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Vector3
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib3</para>
        /// </remarks>
        /// <param name="value">The value to set. The 4th component will be set to 1.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Vector3 value)
        {
            GL.VertexAttrib3(_location, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Vector4
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib4</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Vector4 value)
        {
            GL.VertexAttrib4(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Vector4
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib4</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Vector4 value)
        {
            GL.VertexAttrib4(_location, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Matrix2
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib2</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Matrix2 value)
        {
#if __ANDROID__ || __IOS__
            GL.VertexAttrib2(_location + 0, ref value.R0C0);
            GL.VertexAttrib2(_location + 1, ref value.R1C0);
#else
            GL.VertexAttrib2(_location + 0, ref value.Row0);
            GL.VertexAttrib2(_location + 1, ref value.Row1);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Matrix2
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib2</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Matrix2 value)
        {
#if __ANDROID__ || __IOS__
            GL.VertexAttrib2(_location + 0, ref value.R0C0);
            GL.VertexAttrib2(_location + 1, ref value.R1C0);
#else
            GL.VertexAttrib2(_location + 0, ref value.Row0);
            GL.VertexAttrib2(_location + 1, ref value.Row1);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Matrix3
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib3</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Matrix3 value)
        {
#if __ANDROID__ || __IOS__
            GL.VertexAttrib3(_location + 0, ref value.R0C0);
            GL.VertexAttrib3(_location + 1, ref value.R1C0);
            GL.VertexAttrib3(_location + 2, ref value.R2C0);
#else
            GL.VertexAttrib3(_location + 0, ref value.Row0);
            GL.VertexAttrib3(_location + 1, ref value.Row1);
            GL.VertexAttrib3(_location + 2, ref value.Row2);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Matrix3
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib3</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Matrix3 value)
        {
#if __ANDROID__ || __IOS__
            GL.VertexAttrib3(_location + 0, ref value.R0C0);
            GL.VertexAttrib3(_location + 1, ref value.R1C0);
            GL.VertexAttrib3(_location + 2, ref value.R2C0);
#else
            GL.VertexAttrib3(_location + 0, ref value.Row0);
            GL.VertexAttrib3(_location + 1, ref value.Row1);
            GL.VertexAttrib3(_location + 2, ref value.Row2);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Matrix4
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib4</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Matrix4 value)
        {
            GL.VertexAttrib4(_location + 0, ref value.Row0);
            GL.VertexAttrib4(_location + 1, ref value.Row1);
            GL.VertexAttrib4(_location + 2, ref value.Row2);
            GL.VertexAttrib4(_location + 3, ref value.Row3);
            glUtils.Check(this);
        }

        /// <summary>
        /// Specify the value of a generic vertex attribute or type Matrix4
        /// </summary>
        /// <remarks>
        /// Generic attributes are defined as four-component values that are organized into an array.
        /// The first entry of this array is numbered 0, and the size of the array is implementation-dependent and 
        /// can be queried with <see cref="Shader.MaxVertexAttribs"/>.
        ///
        /// <para>When less than 4 float-type values are passed, the remaining components will be set to 0, 
        /// except for the 4th component, which will be set to 1.</para>
        ///
        /// <para>A user-defined attribute variable declared in a vertex shader can be bound to a generic attribute index using the constructor <see cref="VertexAttribute(Program, int, string)"/>.
        /// This allows an application to use descriptive variable names in a vertex shader.
        /// A subsequent change to the specified generic vertex attribute will be immediately reflected as a change to the corresponding attribute variable in the vertex shader.</para>
        ///
        /// <para>The binding between a generic vertex attribute index and a user-defined attribute variable in a vertex shader is part of the state of a program object, 
        /// but the current value of the generic vertex attribute is not.
        /// The value of each generic vertex attribute is part of current state and it is maintained even if a different program object is used.</para>
        ///
        /// <para>An application may freely modify generic vertex attributes that are not bound to a named vertex shader attribute variable.
        /// These values are simply maintained as part of current state and will not be accessed by the vertex shader.
        /// If a generic vertex attribute bound to an attribute variable in a vertex shader is not updated while the vertex shader is executing, 
        /// the vertex shader will repeatedly use the current value for the generic vertex attribute.</para>
        ///
        /// <para><b>Note</b>: it is possible for an application to bind more than one attribute name to the same generic vertex attribute index.
        /// This is referred to as aliasing, and it is allowed only if just one of the aliased attribute variables is active in the vertex shader, 
        /// or if no path through the vertex shader consumes more than one of the attributes aliased to the same location.
        /// OpenGL implementations are not required to do error checking to detect aliasing, they are allowed to assume that aliasing will not occur, 
        /// and they are allowed to employ optimizations that work only in the absence of aliasing.</para>
        ///
        /// <para><b>OpenGL API</b>: glVertexAttrib4</para>
        /// </remarks>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="SetConfig"/>
        /// <seealso cref="SetData"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Matrix4 value)
        {
            GL.VertexAttrib4(_location + 0, ref value.Row0);
            GL.VertexAttrib4(_location + 1, ref value.Row1);
            GL.VertexAttrib4(_location + 2, ref value.Row2);
            GL.VertexAttrib4(_location + 3, ref value.Row3);
            glUtils.Check(this);
        }

        private static void GetTypeInfo<T>(out DataType dataType, out int numValuesPerVertex) where T : struct
        {
            numValuesPerVertex = 1;
            dataType = DataType.Float;

            Type type = typeof(T);
            if (type == typeof(byte))
            {
                dataType = DataType.UnsignedByte;
            }
            else if (type == typeof(sbyte))
            {
                dataType = DataType.Byte;
            }
            else if (type == typeof(ushort))
            {
                dataType = DataType.UnsignedShort;
            }
            else if (type == typeof(short))
            {
                dataType = DataType.Short;
            }
            else if (type == typeof(float))
            {
                dataType = DataType.Float;
            }
            else if (type == typeof(Vector2))
            {
                numValuesPerVertex = 2;
            }
            else if (type == typeof(Vector3))
            {
                numValuesPerVertex = 3;
            }
            else if (type == typeof(Vector4))
            {
                numValuesPerVertex = 4;
            }
            else
            {
                Debug.Assert(false, "Invalid type parameter");
            }
        }
    }
}
