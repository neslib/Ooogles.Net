using System;

namespace Ooogles
{
    /// <summary>
    /// Exception type for OpenGL shader compilation and linking errors.
    /// </summary>
    /// <remarks>
    /// These types of errors are only checked when DEBUG is defined.
    /// </remarks>
    public class ShaderException : Exception
    {
        /// <summary>
        /// Creates a shader exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ShaderException(string message) : base(message) { }
    }
}
