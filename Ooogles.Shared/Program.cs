using System;
using System.Text;
using System.Diagnostics;
using OpenTK.Graphics.ES20;
using System.Runtime.CompilerServices;

#if __ANDROID__ || __IOS__
using GetProgramParameterName = OpenTK.Graphics.ES20.ProgramParameter;
#endif

namespace Ooogles
{
    /// <summary>
    /// A program that combines a vertex shader and a fragment shader.
    /// </summary>
    public sealed class Program : GLObject
    {
        /// <summary>
        /// Supported data types for attributes
        /// </summary>
        public enum AttributeDataType
        {
            /// <summary>
            /// Single-precision floating-point type.
            /// Corresponds to C#'s float type.
            /// </summary>
            Float = (int)ActiveAttribType.Float,

            /// <summary>
            /// A vector of 2 floats.
            /// Corresponds to OpenTK's Vector2 type.
            /// </summary>
            Vector2 = (int)ActiveAttribType.FloatVec2,

            /// <summary>
            /// A vector of 3 floats.
            /// Corresponds to OpenTK's Vector3 type.
            /// </summary>
            Vector3 = (int)ActiveAttribType.FloatVec3,

            /// <summary>
            /// A vector of 4 floats.
            /// Corresponds to OpenTK's Vector4 type.
            /// </summary>
            Vector4 = (int)ActiveAttribType.FloatVec4,

            /// <summary>
            /// A 2x2 matrix of floats.
            /// Corresponds to OpenTK's Matrix2 type.
            /// </summary>
            Matrix2 = (int)ActiveAttribType.FloatMat2,

            /// <summary>
            /// A 3x3 matrix of floats.
            /// Corresponds to OpenTK's Matrix3 type.
            /// </summary>
            Matrix3 = (int)ActiveAttribType.FloatMat3,

            /// <summary>
            /// A 4x4 matrix of floats.
            /// Corresponds to OpenTK's Matrix4 type.
            /// </summary>
            Matrix4 = (int)ActiveAttribType.FloatMat4
        }

        /// <summary>
        /// Supported data types for uniforms
        /// </summary>
        public enum UniformDataType
        {
            /// <summary>
            /// Single-precision floating-point type.
            /// Corresponds to C#'s float type.
            /// </summary>
            Float = (int)ActiveUniformType.Float,

            /// <summary>
            /// A vector of 2 floats.
            /// Corresponds to OpenTK's Vector2 type.
            /// </summary>
            Vector2 = (int)ActiveUniformType.FloatVec2,

            /// <summary>
            /// A vector of 3 floats.
            /// Corresponds to OpenTK's Vector3 type.
            /// </summary>
            Vector3 = (int)ActiveUniformType.FloatVec3,

            /// <summary>
            /// A vector of 4 floats.
            /// Corresponds to OpenTK's Vector4 type.
            /// </summary>
            Vector4 = (int)ActiveUniformType.FloatVec4,

            /// <summary>
            /// 32-bit integer type.
            /// Corresponds to C#'s int type.
            /// </summary>
            Int = (int)ActiveUniformType.Int,

            /// <summary>
            /// A vector of 2 integers.
            /// </summary>
            IVector2 = (int)ActiveUniformType.IntVec2,

            /// <summary>
            /// A vector of 3 integers.
            /// </summary>
            IVector3 = (int)ActiveUniformType.IntVec3,

            /// <summary>
            /// A vector of 4 integers.
            /// </summary>
            IVector4 = (int)ActiveUniformType.IntVec4,

            /// <summary>
            /// Boolean type.
            /// Corresponds to C#'s bool type.
            /// </summary>
            Bool = (int)ActiveUniformType.Bool,

            /// <summary>
            /// A vector of 2 booleans.
            /// </summary>
            BVector2 = (int)ActiveUniformType.BoolVec2,

            /// <summary>
            /// A vector of 3 booleans.
            /// </summary>
            BVector3 = (int)ActiveUniformType.BoolVec3,

            /// <summary>
            /// A vector of 4 booleans.
            /// </summary>
            BVector4 = (int)ActiveUniformType.BoolVec4,

            /// <summary>
            /// A 2x2 matrix of floats.
            /// Corresponds to OpenTK's Matrix2 type.
            /// </summary>
            Matrix2 = (int)ActiveUniformType.FloatMat2,

            /// <summary>
            /// A 3x3 matrix of floats.
            /// Corresponds to OpenTK's Matrix3 type.
            /// </summary>
            Matrix3 = (int)ActiveUniformType.FloatMat3,

            /// <summary>
            /// A 4x4 matrix of floats.
            /// Corresponds to OpenTK's Matrix4 type.
            /// </summary>
            Matrix4 = (int)ActiveUniformType.FloatMat4,

            /// <summary>
            /// A 2D texture sampler.
            /// </summary>
            Sampler2D = (int)ActiveUniformType.Sampler2D,

            /// <summary>
            /// A cubetexture sampler.
            /// </summary>
            SamplerCube = (int)ActiveUniformType.SamplerCube
        }

        /// <summary>
        /// Information about an attribute as returned by <see cref="Program.GetAttributeInfo(int)"/>.
        /// </summary>
        public struct AttributeInfo
        {
            /// <summary>
            /// The data type of the attribute variable.
            /// </summary>
            public AttributeDataType DataType { get; }

            /// <summary>
            /// The size of the attribute variable, in units of type <see cref="DataType"/>.
            /// </summary>            
            public int Size { get; }

            /// <summary>
            /// The name of the attribute variable.
            /// </summary>
            public string Name { get; }

            internal AttributeInfo(AttributeDataType dataType, int size, string name)
            {
                DataType = dataType;
                Size = size;
                Name = name;
            }
        }

        /// <summary>
        /// Information about a uniform as returned by <see cref="Program.GetUniformInfo(int)"/>.
        /// </summary>
        public struct UniformInfo
        {
            /// <summary>
            /// The data type of the uniform variable.
            /// </summary>
            public UniformDataType DataType { get; }

            /// <summary>
            /// The size of the uniform variable. 
            /// For arrays, this is the length of the array.
            /// Otherwise, the value is 1.
            /// </summary>
            public int Size { get; }

            /// <summary>
            /// The name of the uniform variable.
            /// </summary>
            public string Name { get; }

            internal UniformInfo(UniformDataType dataType, int size, string name)
            {
                DataType = dataType;
                Size = size;
                Name = name;
            }
        }

        /// <summary>
        /// Get the delete status.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_DELETE_STATUS)
        /// </remarks>
        /// <value>
        /// True if the program is currently flagged for deletion. False otherwise.
        /// </value>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Validate"/>
        /// <seealso cref="LinkStatus"/>
        /// <seealso cref="ValidateStatus"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="ActiveAttributes"/>
        /// <seealso cref="ActiveUniforms"/>
        public bool DeleteStatus
        {
            get
            {
                GL.GetProgram(Handle, GetProgramParameterName.DeleteStatus, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get the link status.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_LINK_STATUS)
        /// </remarks>
        /// <value>
        /// True if the last link operation was successful. False otherwise.
        /// </value>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Validate"/>
        /// <seealso cref="DeleteStatus"/>
        /// <seealso cref="ValidateStatus"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="ActiveAttributes"/>
        /// <seealso cref="ActiveUniforms"/>
        public bool LinkStatus
        {
            get
            {
                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get the validate status.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_VALIDATE_STATUS)
        /// </remarks>
        /// <value>
        /// True if the last validate operation was successful. False otherwise.
        /// </value>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Validate"/>
        /// <seealso cref="DeleteStatus"/>
        /// <seealso cref="LinkStatus"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="ActiveAttributes"/>
        /// <seealso cref="ActiveUniforms"/>
        public bool ValidateStatus
        {
            get
            {
                GL.GetProgram(Handle, GetProgramParameterName.ValidateStatus, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get the number of active attribute variables in the program.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_ACTIVE_ATTRIBUTES)
        /// </remarks>
        /// <value>
        /// The number of active attributes.
        /// </value>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Validate"/>
        /// <seealso cref="DeleteStatus"/>
        /// <seealso cref="LinkStatus"/>
        /// <seealso cref="ValidateStatus"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="ActiveUniforms"/>
        public int ActiveAttributes
        {
            get
            {
                GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the number of active uniform variables in the program.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_ACTIVE_UNIFORMS)
        /// </remarks>
        /// <value>
        /// The number of active uniforms.
        /// </value>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Validate"/>
        /// <seealso cref="DeleteStatus"/>
        /// <seealso cref="LinkStatus"/>
        /// <seealso cref="ValidateStatus"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="ActiveAttributes"/>
        public int ActiveUniforms
        {
            get
            {
                GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the program that is currently active.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetProgramiv(GL_CURRENT_PROGRAM)
        /// </remarks>
        /// <value>
        /// The currently active program or null if there is no program active.
        /// </value>
        /// <seealso cref="Use"/>
        public static Program Current
        {
            get
            {
                GL.GetInteger(GetPName.CurrentProgram, out int value);
                if (value == 0)
                    return null;
                return new Program(value);
            }
        }

        internal Program(int handle) => Handle = handle;

        /// <summary>
        /// Creates an empty program object.
        /// </summary>
        /// <remarks>
        /// A program object is an object to which shader objects can be attached.
        /// This provides a mechanism to specify the shader objects that will be linked to create a program.
        /// It also provides a means for checking the compatibility of the shaders that will be used to create a program
        /// (for instance, checking the compatibility between a vertex shader and a fragment shader). 
        /// When no longer needed as part of a program object, shader objects can be detached.
        /// 
        /// <para>One or more executables are created in a program object by successfully attaching shader objects to it with 
        /// <see cref="AttachShader(Shader)"/>, successfully compiling the shader objects with <see cref="Shader.Compile"/>, 
        /// and successfully linking the program object with <see cref="Link"/>.
        /// These executables are made part of current state when <see cref="Use"/>.
        /// Program objects can be deleted by calling <see cref="GLObject.Dispose()"/>. 
        /// The memory associated with the program object will be deleted when it is no longer part of current rendering state for any context.</para>
        /// 
        /// <para><b>Note</b>: like texture objects, the name space for program objects may be shared across a set of contexts, 
        /// as long as the server sides of the contexts share the same address space.
        /// If the name space is shared across contexts, any attached objects and the data associated with those attached objects are shared as well.</para>
        /// 
        /// <para><b>Note</b>: applications are responsible for providing the synchronization across API calls when objects are accessed from different execution threads.</para>
        /// 
        /// <para><b>OpenGL API</b>: glCreateProgram</para>
        /// </remarks>
        /// <seealso cref="Shader"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="Uniform"/>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="DetachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Use"/>
        /// <seealso cref="Validate"/>
        public Program()
        {
            Handle = GL.CreateProgram();
            glUtils.Check(this);
        }

        /// <summary>
        /// Creates an empty program object.
        /// </summary>
        /// <param name="vertexShader">Vertex shader to attach.</param>
        /// <param name="fragmentShader">Fragment shader to attach.</param>
        /// <remarks>
        /// A program object is an object to which shader objects can be attached.
        /// This provides a mechanism to specify the shader objects that will be linked to create a program.
        /// It also provides a means for checking the compatibility of the shaders that will be used to create a program
        /// (for instance, checking the compatibility between a vertex shader and a fragment shader). 
        /// When no longer needed as part of a program object, shader objects can be detached.
        /// 
        /// <para>One or more executables are created in a program object by successfully attaching shader objects to it with 
        /// <see cref="AttachShader(Shader)"/>, successfully compiling the shader objects with <see cref="Shader.Compile"/>, 
        /// and successfully linking the program object with <see cref="Link"/>.
        /// These executables are made part of current state when <see cref="Use"/>.
        /// Program objects can be deleted by calling <see cref="GLObject.Dispose()"/>. 
        /// The memory associated with the program object will be deleted when it is no longer part of current rendering state for any context.</para>
        /// 
        /// <para><b>Note</b>: like texture objects, the name space for program objects may be shared across a set of contexts, 
        /// as long as the server sides of the contexts share the same address space.
        /// If the name space is shared across contexts, any attached objects and the data associated with those attached objects are shared as well.</para>
        /// 
        /// <para><b>Note</b>: applications are responsible for providing the synchronization across API calls when objects are accessed from different execution threads.</para>
        /// 
        /// <para><b>OpenGL API</b>: glCreateProgram</para>
        /// </remarks>
        /// <seealso cref="Shader"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="Uniform"/>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="DetachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Use"/>
        /// <seealso cref="Validate"/>
        public Program(Shader vertexShader, Shader fragmentShader) : this()
        {
            AttachShaders(vertexShader, fragmentShader);
        }

        /// <summary>
        /// Attach a shader object to the program.
        /// </summary>
        /// <param name="shader">The shader object to be attached.</param>
        /// <exception cref="GLException">InvalidOperation if <paramref name="shader"/> is already attached to the program,
        /// or if another shader object of the same type as shader is already attached to the program.</exception>
        /// <remarks>
        /// In order to create an executable, there must be a way to specify the list of things that will be linked together.
        /// Program objects provide this mechanism. 
        /// Shaders that are to be linked together in a program object must first be attached to that program object. 
        /// This method attaches the shader object specified by <paramref name="shader"/> to the program.
        /// This indicates that shader will be included in link operations that will be performed on program.
        /// 
        /// <para> All operations that can be performed on a shader object are valid whether or not the shader object is attached to a program object. 
        /// It is permissible to attach a shader object to a program object before source code has been loaded into the shader object 
        /// or before the shader object has been compiled. 
        /// Multiple shader objects of the same type may not be attached to a single program object. 
        /// However, a single shader object may be attached to more than one program object. 
        /// If a shader object is deleted while it is attached to a program object, it will be flagged for deletion,
        /// and deletion will not occur until <see cref="DetachShader(Shader)"/>r is called to detach it from all program objects to which it is attached.</para>
        /// 
        /// <para><b>OpenGL API</b>: glAttachShader</para>
        /// </remarks>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="Shader.Compile"/>
        /// <seealso cref="DetachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Shader.Source"/>
        public void AttachShader(Shader shader)
        {
            Debug.Assert(shader != null);
            GL.AttachShader(Handle, shader.Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Attaches a vertex shader and fragment shader in a single call.
        /// </summary>
        /// <param name="vertexShader">The vertex shader to attach.</param>
        /// <param name="fragmentShader">The fragment shader to attach.</param>
        /// <remarks>
        /// This method is just a shortcut for calling <see cref="AttachShader(Shader)"/> twice.
        /// </remarks>
        public void AttachShaders(Shader vertexShader, Shader fragmentShader)
        {
            AttachShader(vertexShader);
            AttachShader(fragmentShader);
        }

        /// <summary>
        /// Detach a shader object from a program object.
        /// </summary>
        /// <param name="shader">The shader object to be detached.</param>
        /// <exception cref="GLException">InvalidOperation if <paramref name="shader"/> is not attached to this program.</exception>
        /// <remarks>
        /// This command can be used to undo the effect of the command AttachShader.
        /// 
        /// <para> If <paramref name="shader"/> has already been flagged for deletion and it is not attached to any other program object, 
        /// it will be deleted after it has been detached.</para>
        /// 
        /// <para><b>OpenGL API</b>: glDetachShader</para>
        /// </remarks>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="AttachShader(Shader)"/>
        public void DetachShader(Shader shader)
        {
            Debug.Assert(shader != null);
            GL.DetachShader(Handle, shader.Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Return the shader objects attached to the program.
        /// </summary>
        /// <returns>An array of attached shaders.</returns>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetAttachedShaders
        /// </remarks>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="DetachShader(Shader)"/>
        public Shader[] GetAttachedShaders()
        {
            int[] handles = new int[2];
#if __ANDROID__ || __IOS__
            int[] counts = new int[1];
            GL.GetAttachedShaders(Handle, 2, counts, handles);
            int count = counts[0];
#else
            GL.GetAttachedShaders(Handle, 2, out int count, handles);
#endif
            glUtils.Check(this);
            Shader[] shaders = new Shader[count];
            for (int i = 0; i < count; i++)
            {
                shaders[i] = new Shader(handles[i]);
            }
            return shaders;
        }

        /// <summary>
        /// Links the program.
        /// </summary>
        /// <returns>
        /// In Release builds: True on success, False on failure.
        /// In Debug builds: True on success or an <see cref="GLException"/> will be thrown on failure.
        /// </returns>
        /// <exception cref="ShaderException">When the shaders cannot be linked.</exception>
        /// <remarks>
        /// A shader object of type <see cref="Shader.Type.Vertex"/> attached to the program is used to create an executable that will run on the programmable vertex processor.
        /// A shader object of type <see cref="Shader.Type.Fragment"/> attached to the program is used to create an executable that will run on the programmable fragment processor.
        /// 
        /// <para>As a result of a successful link operation, all active user-defined uniform variables belonging to program will be initialized to 0, 
        /// and each of the program object's active uniform variables can be accessed using <see cref="Uniform"/>.
        /// Also, any active user-defined attribute variables that have not been bound to a generic vertex attribute index will be bound to one at this time.</para>
        /// 
        /// <para>Linking of a program object can fail for a number of reasons as specified in the OpenGL ES Shading Language Specification. 
        /// This will result in a <see cref="ShaderException"/> (in Debug builds). 
        /// The following lists some of the conditions that will cause a link error.</para>
        /// <list type="bullet">
        /// <item><description>A vertex shader and a fragment shader are not both present in the program object.</description></item>
        /// <item><description>The number of active attribute variables supported by the implementation has been exceeded.</description></item>
        /// <item><description>The storage limit for uniform variables has been exceeded.</description></item>
        /// <item><description>The number of active uniform variables supported by the implementation has been exceeded.</description></item>
        /// <item><description>The main function is missing for the vertex shader or the fragment shader.</description></item>
        /// <item><description>A varying variable actually used in the fragment shader is not declared in the same way (or is not declared at all) in the vertex shader.</description></item>
        /// <item><description>A reference to a function or variable name is unresolved.</description></item>
        /// <item><description>A shared global is declared with two different types or two different initial values.</description></item>
        /// <item><description>One or more of the attached shader objects has not been successfully compiled (via <see cref="Shader.Compile"/>).</description></item>
        /// <item><description>Binding a generic attribute matrix caused some rows of the matrix to fall outside the allowed maximum number of vertex attributes.</description></item>
        /// <item><description>Not enough contiguous vertex attribute slots could be found to bind attribute matrices.</description></item>
        /// </list>
        /// <para>When a program object has been successfully linked, 
        /// the program object can be made part of current state by calling <see cref="Use"/>.</para>
        /// 
        /// <para>This method will also install the generated executables as part of the current rendering state if the link operation was successful 
        /// and the specified program object is already currently in use as a result of a previous call to <see cref="Use"/>. 
        /// If the program object currently in use is relinked unsuccessfully, its link status will be set to False, 
        /// but the executables and associated state will remain part of the current state until a subsequent call to <see cref="Use"/> removes it from use.
        /// After it is removed from use, it cannot be made part of current state until it has been successfully relinked.</para>
        /// 
        /// <para>After the link operation, applications are free to modify attached shader objects, compile attached shader objects, 
        /// detach shader objects, delete shader objects, and attach additional shader objects.
        /// None of these operations affects the information log or the program that is part of the program object.</para>
        /// 
        /// <para><b>Note</b>: if the link operation is unsuccessful, any information about a previous link operation on program is lost
        /// (i.e., a failed link does not restore the old state of program). 
        /// Certain information can still be retrieved from program even after an unsuccessful link operation.</para>
        /// 
        /// <para><b>OpenGL API</b>: glLinkProgram, glGetProgramInfoLog, glGetProgramiv(GL_LINK_STATUS/GL_INFO_LOG_LENGTH)</para>
        /// </remarks>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="Uniform"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Shader.Compile"/>
        /// <seealso cref="DetachShader(Shader)"/>
        /// <seealso cref="Use"/>
        /// <seealso cref="Validate"/>
        public bool Link()
        {
            GL.LinkProgram(Handle);
            glUtils.Check(this);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
            glUtils.Check(this);

#if DEBUG
            string infoLog = GL.GetProgramInfoLog(Handle);
            glUtils.Check(this);
            if (!String.IsNullOrWhiteSpace(infoLog))
            {
                if (status == 1)
                {
                    Debug.WriteLine("Program link warning: " + infoLog);
                }
                else
                {
                    throw new ShaderException("Program link failure: " + infoLog);
                }
            }
#endif

            return (status == 1);
        }

        /// <summary>
        /// Validates the program.
        /// </summary>
        /// <returns>
        /// In Release builds: True on success, False on failure.
        /// In Debug builds: True on success or an <see cref="GLException"/> will be thrown on failure.
        /// </returns>
        /// <exception cref="ShaderException">With any validation errors.</exception>
        /// <remarks>
        /// This method checks to see whether the executables contained in program can execute given the current OpenGL state.
        /// The validation information (exception message) may consist of an empty string, 
        /// or it may be a string containing information about how the current program object interacts with the rest of current OpenGL state.
        /// This provides a way for OpenGL implementers to convey more information about why the current program is inefficient, suboptimal, failing to execute, and so on.
        /// 
        /// <para>If validation is successful, the program is guaranteed to execute given the current state.
        /// Otherwise, the program is guaranteed to not execute.</para>
        /// 
        /// <para>This function is typically useful only during application development.
        /// The informational string stored in the exception message is completely implementation dependent; 
        /// therefore, an application should not expect different OpenGL implementations to produce identical information strings.</para>
        /// 
        /// <para><b>Note</b>: this function mimics the validation operation that OpenGL implementations must perform when 
        /// rendering commands are issued while programmable shaders are part of current state.
        /// The error <see cref="ErrorCode.InvalidOperation"/> will be generated by <see cref="gl.DrawArrays(gl.PrimitiveType, int)"/>
        /// or <see cref="gl.DrawElements(gl.PrimitiveType, byte[])"/> if any two active samplers in the current program object are of different types, 
        /// but refer to the same texture image unit.</para>
        /// 
        /// <para><b>Note</b>: it may be difficult or cause a performance degradation for applications to catch these errors when rendering commands are issued.
        /// Therefore, applications are advised to make calls to <see cref="Validate"/> to detect these issues during application development.</para>
        /// 
        /// <para><b>OpenGL API</b>: glValidateProgram, glGetProgramInfoLog, glGetProgramiv(GL_VALIDATE_STATUS/GL_INFO_LOG_LENGTH)</para>
        /// </remarks>
        /// <seealso cref="Link"/>
        /// <seealso cref="Use"/>
        public bool Validate()
        {
            GL.ValidateProgram(Handle);
            glUtils.Check(this);

            GL.GetProgram(Handle, GetProgramParameterName.ValidateStatus, out int status);
            glUtils.Check(this);

#if DEBUG
            string infoLog = GL.GetProgramInfoLog(Handle);
            glUtils.Check(this);
            if (!String.IsNullOrWhiteSpace(infoLog))
            {
                if (status == 1)
                {
                    Debug.WriteLine("Program validation warning: " + infoLog);
                }
                else
                {
                    throw new ShaderException("Program validation failure: " + infoLog);
                }
            }
#endif

            return (status == 1);
        }

        /// <summary>
        /// Installs the program object as part of current rendering state.
        /// </summary>
        /// <exception cref="GLException">InvalidOperation if the program could not be made part of the current state.</exception>
        /// <remarks>
        /// Executables for each stage are created in a program object by successfully attaching shader objects to it with <see cref="AttachShader(Shader)"/>, 
        /// successfully compiling the shader objects with Compile, and successfully linking the program object with <see cref="Link"/>.
        /// 
        /// <para>A program object will contain executables that will run on the vertex and fragment processors 
        /// if it contains one shader object of type <see cref="Shader.Type.Vertex"/> and one shader object of type <see cref="Shader.Type.Fragment"/>
        /// that have both been successfully compiled and linked.</para>
        /// 
        /// <para>While a program object is in use, applications are free to modify attached shader objects, 
        /// compile attached shader objects, attach shader objects, and detach or delete shader objects. 
        /// None of these operations will affect the executables that are part of the current state.
        /// However, relinking the program object that is currently in use will install the program object as part of the current rendering state 
        /// if the link operation was successful (see <see cref="Link"/>). 
        /// If the program object currently in use is relinked unsuccessfully, its link status will be set to False, 
        /// but the executables and associated state will remain part of the current state 
        /// until a subsequent call to <see cref="Use"/> removes it from use.
        /// After it is removed from use, it cannot be made part of current state until it has been successfully relinked.</para>
        /// 
        /// <para><b>Note</b>: like texture objects and buffer objects, the name space for program objects may be shared across a set of contexts, 
        /// as long as the server sides of the contexts share the same address space.
        /// If the name space is shared across contexts, any attached objects and the data associated with those attached objects are shared as well.</para>
        /// 
        /// <para><b>Note</b>: applications are responsible for providing the synchronization across API calls when objects are accessed from different execution threads.</para>
        ///
        /// <para><b>OpenGL API</b>: glUseProgram</para>
        /// </remarks>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="Uniform"/>
        /// <seealso cref="GetAttachedShaders"/>
        /// <seealso cref="AttachShader(Shader)"/>
        /// <seealso cref="Shader.Compile"/>
        /// <seealso cref="DetachShader(Shader)"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Validate"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Use()
        {
            GL.UseProgram(Handle);
            glUtils.Check(this);
        }

        /// <summary>
        /// Return information about an active attribute variable.
        /// </summary>
        /// <param name="index">The index of the attribute variable to be queried.</param>
        /// <returns>Information about attribute at the given index.</returns>
        /// <exception cref="GLException">InvalidValue if <paramref name="index"/> is greater than or equal to the number of active attribute variables in program.</exception>
        /// <remarks>
        /// This method returns information about an active attribute variable in the program object specified by program.
        /// The number of active attributes can be obtained by calling <see cref="ActiveAttributes"/>. 
        /// A value of 0 for <paramref name="index"/> selects the first active attribute variable.
        /// Permissible values for <paramref name="index"/> range from 0 to the number of active attribute variables minus 1.
        /// 
        /// <para>Attribute variables have arbitrary names and obtain their values through numbered generic vertex attributes. 
        /// An attribute variable is considered active if it is determined during the link operation that it may be accessed during program execution.
        /// Therefore, the program should have previously been linked by calling <see cref="Link"/>, but it is not necessary for it to have been linked successfully.</para>
        /// 
        /// <para>This method will return as much information as it can about the specified active attribute variable. 
        /// If no information is available, the returned name will be an empty string. 
        /// This situation could occur if this function is called after a link operation that failed.</para>
        /// 
        /// <para><b>OpenGL API</b>: glGetActiveAttrib</para>
        /// </remarks>
        /// <seealso cref="ActiveAttributes"/>
        /// <seealso cref="Shader.MaxVertexAttribs"/>
        /// <seealso cref="VertexAttribute"/>
        /// <seealso cref="UniformInfo"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="VertexAttribute.SetValue(float)"/>
        /// <seealso cref="VertexAttribute.SetConfig(VertexAttribute.DataType, int, int, int)"/>
        /// <seealso cref="VertexAttribute.SetData(VertexAttribute.DataType, int, void*, int, bool)"/>
        public AttributeInfo GetAttributeInfo(int index)
        {
            StringBuilder sb = new StringBuilder(64);
            GL.GetActiveAttrib(Handle, index, 64, out int length, out int size, out ActiveAttribType type, sb);
            return new AttributeInfo((AttributeDataType)type, size, sb.ToString());
        }

        /// <summary>
        /// Return information about an active uniform variable.
        /// </summary>
        /// <param name="index">The index of the uniform variable to be queried.</param>
        /// <returns>Information about uniform at the given index.</returns>
        /// <exception cref="GLException">InvalidValue if <paramref name="index"/> is greater than or equal to the number of active uniforms in the program.</exception>
        /// <remarks>
        /// The number of active uniform variables can be obtained by calling <see cref="ActiveUniforms"/>.
        /// A value of 0 for <paramref name="index"/> selects the first active uniform variable.
        /// Permissible values for <paramref name="index"/> range from 0 to the number of active uniform variables minus 1.
        /// 
        /// <para>Shaders may use either built-in uniform variables, user-defined uniform variables, or both.
        /// Built-in uniform variables have a prefix of "gl_" and reference existing OpenGL state or values derived from such state (e.g., gl_DepthRange). 
        /// User-defined uniform variables have arbitrary names and obtain their values from the application through calls to <see cref="Uniform.SetValue(float)"/>.
        /// A uniform variable (either built-in or user-defined) is considered active if it is determined during the link operation that it may be accessed during program execution.
        /// Therefore, program should have previously been linked by calling <see cref="Link"/>, but it is not necessary for it to have been linked successfully.</para>
        ///
        /// <para>Only one active uniform variable will be reported for a uniform array.</para>
        /// 
        /// <para>Uniform variables that are declared as structures or arrays of structures will not be returned directly by this function.
        /// Instead, each of these uniform variables will be reduced to its fundamental components containing the "." and "[]" operators 
        /// such that each of the names is valid as an argument to <see cref="Uniform"/>.
        /// Each of these reduced uniform variables is counted as one active uniform variable and is assigned an index.
        /// A valid name cannot be a structure, an array of structures, or a subcomponent of a vector or matrix.</para>
        /// 
        /// <para>Uniform variables other than arrays will have a Size of 1. 
        /// Structures and arrays of structures will be reduced as described earlier, 
        /// such that each of the names returned will be a data type in the earlier list.
        /// If this reduction results in an array, the size returned will be as described for uniform arrays; 
        /// otherwise, the size returned will be 1.</para>
        /// 
        /// <para>The list of active uniform variables may include both built-in uniform variables
        /// (which begin with the prefix "gl_") as well as user-defined uniform variable names.</para>
        /// 
        /// <para>This function will return as much information as it can about the specified active uniform variable. 
        /// If no information is available, the returned name will be an empty string. 
        /// This situation could occur if this function is called after a link operation that failed.</para>
        /// 
        /// <para><b>OpenGL API</b>: glGetActiveUniform</para>
        /// </remarks>
        /// <seealso cref="Uniform.MaxVertexUniformVectors"/>
        /// <seealso cref="Uniform.MaxFragmentUniformVectors"/>
        /// <seealso cref="ActiveUniforms"/>
        /// <seealso cref="GetAttributeInfo(int)"/>
        /// <seealso cref="Uniform.GetValue(out float)"/>
        /// <seealso cref="Uniform"/>
        /// <seealso cref="Link"/>
        /// <seealso cref="Uniform.SetValue(float)"/>
        /// <seealso cref="Use"/>
        public UniformInfo GetUniformInfo(int index)
        {
            StringBuilder sb = new StringBuilder(64);
            GL.GetActiveUniform(Handle, index, 64, out int length, out int size, out ActiveUniformType type, sb);
            return new UniformInfo((UniformDataType)type, size, sb.ToString());
        }

        /// <summary>
        /// Deletes the program and its associated resources.
        /// </summary>
        /// <remarks>
        /// This method is called automatically as a result to calling <see cref="GLObject.Dispose()"/>
        /// </remarks>
        protected override void DisposeHandle()
        {
            GL.DeleteProgram(Handle);
            glUtils.Check(this);
        }
    }
}
