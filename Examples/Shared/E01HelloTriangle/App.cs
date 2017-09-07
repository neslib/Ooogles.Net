using System;
using Examples.Shared;
using OpenTK;
using Ooogles;

/* Based on Hello_Triangle.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E01HelloTriangle
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private VertexAttribute _attrPosition;

        private readonly Vector3[] _vertices = {
            new Vector3( 0.0f,  0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3( 0.5f, -0.5f, 0.0f)
        };

        public override void Load()
        {
            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                attribute vec4 vPosition;

                void main(void)
                {
                  gl_Position = vPosition;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                void main(void)
                {
                  gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
                }");
            fragmentShader.Compile();

            // Link shaders into program
            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            // We don't need the shaders anymore. 
            // Note that the shaders won't actually be deleted until the program is deleted.
            vertexShader.Dispose();
            fragmentShader.Dispose();

            // Initialize vertex attribute
            _attrPosition = new VertexAttribute(_program, "vPosition");

            // Set clear color to black
            gl.ClearColor(0, 0, 0, 0);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color buffer
            gl.Clear(gl.ClearBuffers.Color);

            // Use the program
            _program.Use();

            // Set the data for the vertex attribute
            _attrPosition.SetData(_vertices);
            _attrPosition.Enable();

            // Draw the triangle
            gl.DrawArrays(gl.PrimitiveType.Triangles, 3);
        }

        public override void Unload()
        {
            _program?.Dispose();
        }
    }
}
