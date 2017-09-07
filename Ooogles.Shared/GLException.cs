using System;

namespace Ooogles
{
    /// <summary>
    /// Exception type for OpenGL related errors.
    /// </summary>
    /// <remarks>
    /// When compiling with the DEBUG conditional define, and assertions enabled (the default configuration for Debug builds), 
    /// every OpenGL call is checked for errors, and this type of exception is raised when an error occurs.
    /// </remarks>
    public class GLException : Exception
    {
        /// <summary>
        /// The OpenGL error that happened.
        /// </summary>
        public gl.Error Error { get; }

        /// <summary>
        /// The method in which the error> occurred.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Creates an OpenGL exception
        /// </summary>
        /// <param name="error">The OpenGL error that happened.</param>
        /// <param name="method">The method in which the error> occurred.</param>
        public GLException(gl.Error error, string method) : base(
            $"OpenGL error {error} in method {method})")
        {
            Error = error;
            Method = method;
        }
    }
}
