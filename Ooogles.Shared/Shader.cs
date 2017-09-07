using System;
using System.Text;
using System.Diagnostics;
using OpenTK.Graphics.ES20;

namespace Ooogles
{
    /// <summary>
    /// A vertex or fragment shader
    /// </summary>
    public sealed class Shader : GLObject
    {
        /// <summary>
        /// The type of a <see cref="Shader"/>
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A vertex shader
            /// </summary>
            Vertex = (int)OpenTK.Graphics.ES20.ShaderType.VertexShader,

            /// <summary>
            /// A fragment shader
            /// </summary>
            Fragment = (int)OpenTK.Graphics.ES20.ShaderType.FragmentShader
        }

        /// <summary>
        /// Get, set or replace the source code in a shader object.
        /// </summary>
        /// <remarks>
        /// When setting, any source code previously stored in the shader object is completely replaced.
        ///
        /// <para><b>OpenGL API</b>: glShaderSource, glGetShaderSource</para>
        /// </remarks>
        /// <exception cref="GLException">InvalidOperation if a shader compiler is not supported.</exception>
        /// <seealso cref="Compile"/>
        public string Source
        {
            get
            {
                GL.GetShader(Handle, ShaderParameter.ShaderSourceLength, out int length);
                glUtils.Check(this);
                if (length <= 1)
                    return string.Empty;

                StringBuilder sb = new StringBuilder(length);
                GL.GetShaderSource(Handle, length, out length, sb);
                glUtils.Check(this);
                return sb.ToString();
            }
            set
            {
                GL.ShaderSource(Handle, value);
                glUtils.Check(this);
            }
        }

        /// <summary>
        /// Gets the type of the shader.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetShaderiv(GL_SHADER_TYPE)
        /// </remarks>
        /// <value>The shader type.</value>
        public Type ShaderType
        {
            get
            {
                GL.GetShader(Handle, ShaderParameter.ShaderType, out int value);
                glUtils.Check(this);
                return (Type)value;
            }
        }

        /// <summary>
        /// Get the delete status.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetShaderiv(GL_DELETE_STATUS)
        /// </remarks>
        /// <value>True if the shader is currently flagged for deletion. False otherwise.</value>
        /// <seealso cref="Compile"/>
        /// <seealso cref="Source"/>
        public bool DeleteStatus
        {
            get
            {
                GL.GetShader(Handle, ShaderParameter.DeleteStatus, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get the compile status.
        /// </summary>
        /// <remarks>
        /// <b>OpenGL API</b>: glGetShaderiv(GL_COMPILE_STATUS)
        /// </remarks>
        /// <returns>True if the last <see cref="Compile"/> operation was successful. False otherwise.</returns>
        /// <seealso cref="Compile"/>
        /// <seealso cref="Source"/>
        public bool CompileStatus
        {
            get
            {
                GL.GetShader(Handle, ShaderParameter.CompileStatus, out int value);
                glUtils.Check(this);
                return (value == 1);
            }
        }

        /// <summary>
        /// Get the maximum number four-element floating-point vectors available for interpolating varying variables used by vertex and fragment shaders.
        /// </summary>
        /// <remarks>
        /// Varying variables declared as matrices or arrays will consume multiple interpolators.
        /// The value must be at least 8.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_VARYING_VECTORS)</para>
        /// </remarks>
        public static int MaxVaryingVectors
        {
            get
            {
                GL.GetInteger(GetPName.MaxVaryingVectors, out int value);
                glUtils.Check("Shader");
                return value;
            }
        }

        /// <summary>
        /// Get the maximum number of 4-component generic vertex attributes accessible to a vertex shader.
        /// </summary>
        /// <remarks>
        /// The value must be at least 8.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_VERTEX_ATTRIBS)</para>
        /// </remarks>
        public static int MaxVertexAttribs
        {
            get
            {
                GL.GetInteger(GetPName.MaxVertexAttribs, out int value);
                glUtils.Check("Shader");
                return value;
            }
        }

        internal Shader(int handle) => Handle = handle;

        /// <summary>
        /// Creates a shader object.
        /// </summary>
        /// <remarks>
        /// A shader object is used to maintain the source code that define a shader.
        ///
        /// <para><paramref name="type"/> indicates the type of shader to be created.
        /// Two types of shaders are supported.
        /// A shader of type <see cref="Type.Vertex"/> is a shader that is intended to run on the programmable vertex processor.
        /// A shader of type <see cref="Type.Fragment"/> is a shader that is intended to run on the programmable fragment processor.</para>
        ///
        /// <para><b>Note</b>: like texture objects, the name space for shader objects may be shared across a set of contexts, 
        /// as long as the server sides of the contexts share the same address space.
        /// If the name space is shared across contexts, any attached objects and the data associated with those attached objects are shared as well.</para>
        ///
        /// <para><b>Note</b>: applications are responsible for providing the synchronization across API calls when objects are accessed from different execution threads.</para>
        ///
        /// <para><b>OpenGL API</b>: glCreateShader</para>
        /// </remarks>
        /// <param name="type">the type of shader to be created.</param>
        /// <seealso cref="Source"/>
        /// <seealso cref="Program.AttachShader"/>
        /// <seealso cref="Program.DetachShader"/>
        /// <seealso cref="Compile"/>
        public Shader(Type type)
        {
            Handle = GL.CreateShader((ShaderType)type);
            glUtils.Check(this);
        }

        /// <summary>
        /// Creates a shader object.
        /// </summary>
        /// <remarks>
        /// A shader object is used to maintain the source code that define a shader.
        ///
        /// <para><paramref name="type"/> indicates the type of shader to be created.
        /// Two types of shaders are supported.
        /// A shader of type <see cref="Type.Vertex"/> is a shader that is intended to run on the programmable vertex processor.
        /// A shader of type <see cref="Type.Fragment"/> is a shader that is intended to run on the programmable fragment processor.</para>
        ///
        /// <para><b>Note</b>: like texture objects, the name space for shader objects may be shared across a set of contexts, 
        /// as long as the server sides of the contexts share the same address space.
        /// If the name space is shared across contexts, any attached objects and the data associated with those attached objects are shared as well.</para>
        ///
        /// <para><b>Note</b>: applications are responsible for providing the synchronization across API calls when objects are accessed from different execution threads.</para>
        ///
        /// <para><b>OpenGL API</b>: glCreateShader</para>
        /// </remarks>
        /// <param name="type">the type of shader to be created.</param>
        /// <param name="source">GLSL-ES source code for the shader. When specified, the <see cref="Source"/> will be set.</param>
        /// <seealso cref="Source"/>
        /// <seealso cref="Program.AttachShader"/>
        /// <seealso cref="Program.DetachShader"/>
        /// <seealso cref="Compile"/>
        public Shader(Type type, string source) : this(type)
        {
            Source = source;
        }

        /// <summary>
        /// Compiles the shader;
        /// </summary>
        /// <remarks>
        /// <para><b>Note</b>: in DEBUG mode, any compiler warnings will be output to the debug console.</para>
        ///
        /// <para><b>OpenGL API</b>: glCompileShader, glGetShaderInfoLog, glGetShaderiv(GL_COMPILE_STATUS/GL_INFO_LOG_LENGTH)</para>
        /// </remarks>
        /// <returns>In RELEASE mode: True on success, False on failure. 
        /// In DEBUG mode: True on success or an <see cref="GLException"/> will be thrown on failure.</returns>
        /// <exception cref="GLException">InvalidOperation if a shader compiler is not supported.</exception>
        /// <exception cref="ShaderException">When the source code contains errors and cannot be compiled.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Source"/>
        /// <seealso cref="Shader.ReleaseCompiler"/>
        public bool Compile()
        {
            GL.CompileShader(Handle);
            glUtils.Check(this);

            GL.GetShader(Handle, ShaderParameter.CompileStatus, out int status);
            glUtils.Check(this);

#if DEBUG
            string infoLog = GL.GetShaderInfoLog(Handle);
            glUtils.Check(this);
            if (!String.IsNullOrWhiteSpace(infoLog))
            {
                if (status == 1)
                {
                    Debug.WriteLine("Shader compiler warning: " + infoLog);
                }
                else
                {
                    throw new ShaderException("Shader compiler failure: " + infoLog);
                }
            }
#endif

            return (status == 1);
        }

        /// <summary>
        /// Release resources allocated by the shader compiler.
        /// </summary>
        /// <remarks>
        /// For implementations that support a shader compiler, this method frees resources allocated by the shader compiler.
        /// This is a hint from the application that additional shader compilations are unlikely to occur, at least for some period of time, 
        /// and that the resources consumed by the shader compiler may be released and put to better use elsewhere.
        ///
        /// <para>However, if a call to <see cref="Compile"/> is made after a call to <c>ReleaseCompiler</c>, 
        /// the shader compiler must be restored to service the compilation request as if <c>ReleaseCompiler</c> had never been called.</para>
        ///
        /// <para><b>OpenGL API</b>: glReleaseShaderCompiler</para>
        /// </remarks>
        /// <seealso cref="Compile"/>
        public static void ReleaseCompiler()
        {
            GL.ReleaseShaderCompiler();
            glUtils.Check("Shader");
        }

        /// <summary>
        /// Deletes the shader.
        /// </summary>
        /// <remarks>
        /// This frees the memory and invalidates the name associated with the shader object specified by shader.
        ///
        /// <para>If a shader object to be deleted is attached to a program object, it will be flagged for deletion, 
        /// but it will not be deleted until it is no longer attached to any program object, 
        /// for any rendering context (i.e., it must be detached from wherever it was attached before it will be deleted).</para>
        ///
        /// <para>To determine whether an object has been flagged for deletion, use <see cref="DeleteStatus"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glDeleteShader</para>
        /// </remarks>
        /// <seealso cref="Program"/>
        /// <seealso cref="Program.DetachShader"/>
        /// <seealso cref="Program.Use"/>
        protected override void DisposeHandle()
        {
            GL.DeleteShader(Handle);
            glUtils.Check(this);
        }
    }
}
