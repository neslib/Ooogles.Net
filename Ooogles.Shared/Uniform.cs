using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace Ooogles
{
    /// <summary>
    /// Represents a uniform in a <see cref="Program"/>.
    /// </summary>
    /// <remarks>
    /// These are variables marked with <c>uniform</c> in a vertex or fragment shader.
    /// </remarks>
    public struct Uniform
    {
        private int _location;
        private int _program;

        /// <summary>
        /// Get the maximum number of four-element floating-point, integer, 
        /// or boolean vectors that can be held in uniform variable storage for a vertex shader.
        /// </summary>
        /// <remarks>
        /// The value must be at least 128.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_VERTEX_UNIFORM_VECTORS)</para>
        /// </remarks>
        /// <seealso cref="SetValue(float)"/>
        /// <seealso cref="SetValues(float[])"/>
        /// <seealso cref="MaxFragmentUniformVectors"/>
        public int MaxVertexUniformVectors
        {
            get
            {
                GL.GetInteger(GetPName.MaxVertexUniformVectors, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// Get the maximum number of four-element floating-point, integer, 
        /// or boolean vectors that can be held in uniform variable storage for a fragment shader.
        /// </summary>
        /// <remarks>
        /// The value must be at least 16.
        ///
        /// <para><b>OpenGL API</b>: glGetIntegerv(GL_MAX_FRAGMENT_UNIFORM_VECTORS)</para>
        /// </remarks>
        /// <seealso cref="SetValue(float)"/>
        /// <seealso cref="SetValues(float[])"/>
        /// <seealso cref="MaxVertexUniformVectors"/>
        public int MaxFragmentUniformVectors
        {
            get
            {
                GL.GetInteger(GetPName.MaxFragmentUniformVectors, out int value);
                glUtils.Check(this);
                return value;
            }
        }

        /// <summary>
        /// The location of this uniform in the program.
        /// </summary>
        public int Location => _location;

        /// <summary>
        /// Creates a uniform.
        /// </summary>
        /// <remarks>
        /// <paramref name="uniformName"/> must be an active uniform variable name in program that is not a structure, 
        /// an array of structures, or a subcomponent of a vector or a matrix.
        ///
        /// <para>Uniform variables that are structures or arrays of structures may be queried by using separate <see cref="Uniform"/> classes for each field within the structure.
        /// The array element operator '[]' and the structure field operator '.' may be used in name in order to select elements within an array or fields within a structure.
        /// The result of using these operators is not allowed to be another structure, an array of structures, or a subcomponent of a vector or a matrix.
        /// Except if the last part of name indicates a uniform variable array, the location of the first element of an array can be retrieved by using the name of the array, 
        /// or by using the name appended by '[0]'.</para>
        ///
        /// <para>The actual locations assigned to uniform variables are not known until the program object is linked successfully.</para>
        ///
        /// <para><b>Note</b>: in DEBUG mode, a warning will be logged to the debug console if <paramref name="program"/> does not contain a uniform named <paramref name="uniformName"/>.</para>
        ///
        /// <para><b>OpenGL API</b>: glGetUniformLocation</para>
        /// </remarks>
        /// <param name="program">the program containing the uniform.</param>
        /// <param name="uniformName">the (case-sensitive) name of the uniform as used in the vertex or fragment shader of the program.</param>
        /// <exception cref="GLException">InvalidOperation if <paramref name="program"/> is not a valid program or has not been successfully linked.</exception>
        /// <seealso cref="SetValue(float)"/>
        public Uniform(Program program, string uniformName)
        {
            Debug.Assert(program != null);
            Debug.Assert(!String.IsNullOrWhiteSpace(uniformName));
            _program = program.Handle;
            _location = GL.GetUniformLocation(_program, uniformName);
            glUtils.Check(this);
#if DEBUG
            if (_location < 0)
                Debug.WriteLine($"Unable to get location of uniform {uniformName}");
#endif
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform1</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value)
        {
            GL.Uniform1(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the individual elements of a uniform or type Vector2.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para>This overload works identical to <see cref="SetValue(Vector2)"/>.</para>
        /// 
        /// <para><b>OpenGL API</b>: glUniform2</para>
        /// </remarks>
        /// <param name="value0">The first element (X) to set</param>
        /// <param name="value1">The second element (Y) to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value0, float value1)
        {
            GL.Uniform2(_location, value0, value1);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the individual elements of a uniform or type Vector3.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para>This overload works identical to <see cref="SetValue(Vector3)"/>.</para>
        /// 
        /// <para><b>OpenGL API</b>: glUniform3</para>
        /// </remarks>
        /// <param name="value0">The first element (X) to set</param>
        /// <param name="value1">The second element (Y) to set</param>
        /// <param name="value2">The third element (Z) to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value0, float value1, float value2)
        {
            GL.Uniform3(_location, value0, value1, value2);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the individual elements of a uniform or type Vector4.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para>This overload works identical to <see cref="SetValue(Vector4)"/>.</para>
        /// 
        /// <para><b>OpenGL API</b>: glUniform4</para>
        /// </remarks>
        /// <param name="value0">The first element (X) to set</param>
        /// <param name="value1">The second element (Y) to set</param>
        /// <param name="value2">The third element (Z) to set</param>
        /// <param name="value3">The fourth element (W) to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(float value0, float value1, float value2, float value3)
        {
            GL.Uniform4(_location, value0, value1, value2, value3);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform2</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Vector2 value)
        {
            GL.Uniform2(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform2</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Vector2 value)
        {
            GL.Uniform2(_location, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform3</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Vector3 value)
        {
            GL.Uniform3(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform3</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Vector3 value)
        {
            GL.Uniform3(_location, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform4</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(Vector4 value)
        {
            GL.Uniform4(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform4</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Vector4 value)
        {
            GL.Uniform4(_location, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: UniformMatrix2</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Matrix2 value)
        {
            GL.UniformMatrix2(_location, false, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: UniformMatrix3</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Matrix3 value)
        {
            GL.UniformMatrix3(_location, false, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: UniformMatrix4</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(ref Matrix4 value)
        {
            GL.UniformMatrix4(_location, false, ref value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the value of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform1</para>
        /// </remarks>
        /// <param name="value">The value to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if type of the parameter does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int value)
        {
            GL.Uniform1(_location, value);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the individual elements of a uniform containing two integer values.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        /// 
        /// <para><b>OpenGL API</b>: glUniform2</para>
        /// </remarks>
        /// <param name="value0">The first element to set</param>
        /// <param name="value1">The second element to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int value0, int value1)
        {
            GL.Uniform2(_location, value0, value1);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the individual elements of a uniform containing three integer values.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        /// 
        /// <para><b>OpenGL API</b>: glUniform3</para>
        /// </remarks>
        /// <param name="value0">The first element to set</param>
        /// <param name="value1">The second element to set</param>
        /// <param name="value2">The third element to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int value0, int value1, int value2)
        {
            GL.Uniform3(_location, value0, value1, value2);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets the individual elements of a uniform containing four integer values.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        /// 
        /// <para><b>OpenGL API</b>: glUniform4</para>
        /// </remarks>
        /// <param name="value0">The first element to set</param>
        /// <param name="value1">The second element to set</param>
        /// <param name="value2">The third element to set</param>
        /// <param name="value3">The fourth element to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int value0, int value1, int value2, int value3)
        {
            GL.Uniform4(_location, value0, value1, value2, value3);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform1</para>
        /// </remarks>
        /// <param name="values">array of floating-point values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(float[] values)
        {
            GL.Uniform1(_location, values.Length, values);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform2</para>
        /// </remarks>
        /// <param name="values">array of Vector2 values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(Vector2[] values)
        {
            GL.Uniform2(_location, values.Length, ref values[0].X);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform3</para>
        /// </remarks>
        /// <param name="values">array of Vector3 values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(Vector3[] values)
        {
            GL.Uniform3(_location, values.Length, ref values[0].X);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform4</para>
        /// </remarks>
        /// <param name="values">array of Vector4 values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(Vector4[] values)
        {
            GL.Uniform4(_location, values.Length, ref values[0].X);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: glUniform1</para>
        /// </remarks>
        /// <param name="values">array of integer values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(int[] values)
        {
            GL.Uniform1(_location, values.Length, values);
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: UniformMatrix2</para>
        /// </remarks>
        /// <param name="values">array of Matrix2 values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(Matrix2[] values)
        {
#if __ANDROID__ || __IOS__
            GL.UniformMatrix2(_location, values.Length, false, ref values[0].R0C0);
#else
            GL.UniformMatrix2(_location, values.Length, false, ref values[0].Row0.X);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: UniformMatrix3</para>
        /// </remarks>
        /// <param name="values">array of Matrix3 values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(Matrix3[] values)
        {
#if __ANDROID__ || __IOS__
            GL.UniformMatrix3(_location, values.Length, false, ref values[0].R0C0);
#else
            GL.UniformMatrix3(_location, values.Length, false, ref values[0].Row0.X);
#endif
            glUtils.Check(this);
        }

        /// <summary>
        /// Sets an array of values of the uniform.
        /// </summary>
        /// <remarks>
        /// This method modifies the value of a uniform variable.
        /// It operates on the program object that was made part of current state by calling <see cref="Program.Use"/>.
        ///
        /// <para>All active uniform variables defined in a program object are initialized to 0 when the program object is linked successfully.
        /// They retain the values assigned to them by a call to <c>SetValue</c> until the next successful link operation occurs on the program object, 
        /// when they are once again initialized to 0.</para>
        ///
        /// <para><b>OpenGL API</b>: UniformMatrix4</para>
        /// </remarks>
        /// <param name="values">array of Matrix4 values to set</param>
        /// <exception cref="GLException">InvalidOperation if there is no current program in use.</exception>
        /// <exception cref="GLException">InvalidOperation if the number and types of the parameters does not match the declaration in the shader.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="GetValue(out float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValues(Matrix4[] values)
        {
            GL.UniformMatrix4(_location, values.Length, false, ref values[0].Row0.X);
            glUtils.Check(this);
        }

        /// <summary>
        /// Return the value of a uniform variable.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of value that is returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out float value)
        {
            GL.GetUniform(_program, _location, out value);
        }

        /// <summary>
        /// Return the value of a uniform variable of type Vector2.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value0">is set to first element (X).</param>
        /// <param name="value1">is set to second element (Y).</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out float value0, out float value1)
        {
            float[] values = new float[2];
            GL.GetUniform(_program, _location, values);
            value0 = values[0];
            value1 = values[1];
        }

        /// <summary>
        /// Return the value of a uniform variable of type Vector3.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value0">is set to first element (X).</param>
        /// <param name="value1">is set to second element (Y).</param>
        /// <param name="value2">is set to third element (Z).</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out float value0, out float value1, out float value2)
        {
            float[] values = new float[3];
            GL.GetUniform(_program, _location, values);
            value0 = values[0];
            value1 = values[1];
            value2 = values[2];
        }

        /// <summary>
        /// Return the value of a uniform variable of type Vector4.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value0">is set to first element (X).</param>
        /// <param name="value1">is set to second element (Y).</param>
        /// <param name="value2">is set to third element (Z).</param>
        /// <param name="value3">is set to fourth element (W).</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out float value0, out float value1, out float value2, out float value3)
        {
            float[] values = new float[4];
            GL.GetUniform(_program, _location, values);
            value0 = values[0];
            value1 = values[1];
            value2 = values[2];
            value3 = values[3];
        }

        /// <summary>
        /// Return the value of a uniform variable of type Vector2.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out Vector2 value)
        {
            float[] values = new float[2];
            GL.GetUniform(_program, _location, values);
            value = new Vector2(values[0], values[1]);
        }

        /// <summary>
        /// Return the value of a uniform variable of type Vector3.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out Vector3 value)
        {
            float[] values = new float[3];
            GL.GetUniform(_program, _location, values);
            value = new Vector3(values[0], values[1], values[2]);
        }

        /// <summary>
        /// Return the value of a uniform variable of type Vector4.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out Vector4 value)
        {
            float[] values = new float[4];
            GL.GetUniform(_program, _location, values);
            value = new Vector4(values[0], values[1], values[2], values[3]);
        }

        /// <summary>
        /// Return the value of a uniform variable of type Matrix2.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out Matrix2 value)
        {
            float[] values = new float[4];
            GL.GetUniform(_program, _location, values);
            value = new Matrix2(values[0], values[1], values[2], values[3]);
        }

        /// <summary>
        /// Return the value of a uniform variable of type Matrix3.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out Matrix3 value)
        {
            float[] values = new float[9];
            GL.GetUniform(_program, _location, values);
            value = new Matrix3(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8]);
        }

        /// <summary>
        /// Return the value of a uniform variable of type Matrix4.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out Matrix4 value)
        {
            float[] values = new float[16];
            GL.GetUniform(_program, _location, values);
            value = new Matrix4(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7],
                values[8], values[9], values[10], values[11], values[12], values[13], values[14], values[15]);
        }

        /// <summary>
        /// Return the value of a uniform variable.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of value that is returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value">is set to the returned value.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out int value)
        {
            GL.GetUniform(_program, _location, out value);
        }

        /// <summary>
        /// Return the value of a uniform variable containing two integer elements.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value0">is set to first element.</param>
        /// <param name="value1">is set to second element.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out int value0, out int value1)
        {
            int[] values = new int[2];
            GL.GetUniform(_program, _location, values);
            value0 = values[0];
            value1 = values[1];
        }

        /// <summary>
        /// Return the value of a uniform variable containing three integer elements.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value0">is set to first element.</param>
        /// <param name="value1">is set to second element.</param>
        /// <param name="value2">is set to third element.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out int value0, out int value1, out int value2)
        {
            int[] values = new int[3];
            GL.GetUniform(_program, _location, values);
            value0 = values[0];
            value1 = values[1];
            value2 = values[2];
        }

        /// <summary>
        /// Return the value of a uniform variable containing four integer elements.
        /// </summary>
        /// <remarks>
        /// The type of the uniform variable determines the type of values that are returned.
        /// It is the responsibility of the caller to make sure that the type of the parameters matches the type of the uniform variable in the shader.
        ///
        /// <para>The uniform variable values can only be queried after a link if the link was successful.</para>
        ///
        /// <para> <b>OpenGL API</b>: glGetUniform</para>
        /// </remarks>
        /// <param name="value0">is set to first element.</param>
        /// <param name="value1">is set to second element.</param>
        /// <param name="value2">is set to third element.</param>
        /// <param name="value3">is set to fourth element.</param>
        /// <exception cref="GLException">InvalidOperation if the program is not successfully linked.</exception>
        /// <seealso cref="Program.Link"/>
        /// <seealso cref="Program.Use"/>
        /// <seealso cref="SetValue(float)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetValue(out int value0, out int value1, out int value2, out int value3)
        {
            int[] values = new int[4];
            GL.GetUniform(_program, _location, values);
            value0 = values[0];
            value1 = values[1];
            value2 = values[2];
            value3 = values[3];
        }
    }
}
