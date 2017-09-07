using System;
using Ooogles;
using OpenTK;

/* Based on Stencil_Test.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E08StencilOperations
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private VertexAttribute _attrPosition;
        private Uniform _uniColor;

        private readonly Vector3[] _vertices = {
            // Quad #0
            new Vector3(-0.75f,  0.25f, 0.50f),
            new Vector3(-0.25f,  0.25f, 0.50f),
            new Vector3(-0.25f,  0.75f, 0.50f),
            new Vector3(-0.75f,  0.75f, 0.50f),
            // Quad #1
            new Vector3( 0.25f,  0.25f, 0.90f),
            new Vector3( 0.75f,  0.25f, 0.90f),
            new Vector3( 0.75f,  0.75f, 0.90f),
            new Vector3( 0.25f,  0.75f, 0.90f),
            // Quad #2
            new Vector3(-0.75f, -0.75f, 0.50f),
            new Vector3(-0.25f, -0.75f, 0.50f),
            new Vector3(-0.25f, -0.25f, 0.50f),
            new Vector3(-0.75f, -0.25f, 0.50f),
            // Quad #3
            new Vector3( 0.25f, -0.75f, 0.50f),
            new Vector3( 0.75f, -0.75f, 0.50f),
            new Vector3( 0.75f, -0.25f, 0.50f),
            new Vector3( 0.25f, -0.25f, 0.50f),
            // Big Quad
            new Vector3(-1.00f, -1.00f, 0.00f),
            new Vector3( 1.00f, -1.00f, 0.00f),
            new Vector3( 1.00f,  1.00f, 0.00f),
            new Vector3(-1.00f,  1.00f, 0.00f)
        };

        private readonly byte[] _indices0 = {  0,  1,  2,  0,  2,  3 }; // Quad #0
        private readonly byte[] _indices1 = {  4,  5,  6,  4,  6,  7 }; // Quad #1
        private readonly byte[] _indices2 = {  8,  9, 10,  8, 10, 11 }; // Quad #2
        private readonly byte[] _indices3 = { 12, 13, 14, 12, 14, 15 }; // Quad #3
        private readonly byte[] _indices4 = { 16, 17, 18, 16, 18, 19 }; // Big Quad

        private const int _testCount = 4;

        private readonly Vector4[] _colors =
        {
            new Vector4(1, 0, 0, 1),
            new Vector4(0, 1, 0, 1),
            new Vector4(0, 0, 1, 1),
            new Vector4(1, 1, 0, 0)
        };

        public override bool NeedStencilBuffer()
        {
            // This application needs a stencil buffer
            return true;
        }

        public override void Load()
        {
            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                attribute vec4 a_position;

                void main()
                {
                  gl_Position = a_position;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform vec4 u_color;

                void main()
                {
                  gl_FragColor = u_color;
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
            _attrPosition = new VertexAttribute(_program, "a_position");

            // Initialize uniform
            _uniColor = new Uniform(_program, "u_color");

            // Set clear color to black
            gl.ClearColor(0, 0, 0, 0);

            // Set the stencil clear value
            gl.ClearStencil(0x01);

            // Set the depth clear value
            gl.ClearDepth(0.75f);

            // Enable the depth and stencil tests
            gl.Enable(gl.Capability.DepthTest);
            gl.Enable(gl.Capability.StencilTest);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Result of tests 0, 1 and 2
            byte[] stencilValues = { 0x07, 0x00, 0x02, 0x00 };

            // Clear the color, depth, and stencil buffers.
            // At this point, the stencil buffer will be 0x01 for all pixels.
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth | gl.ClearBuffers.Stencil);

            // Use the program
            _program.Use();

            // Set the data for the vertex attribute
            _attrPosition.SetData(_vertices);
            _attrPosition.Enable();

            // Test 0:
            //
            // Initialize upper-left region. In this case, the stencil-buffer values will
            // be replaced because the stencil test for the rendered pixels will fail the
            // stencil test, which is:
            //
            //    ref    mask   stencil  mask
            //   (0x07 & 0x03) < (0x01 & 0x07)
            //
            // The value in the stencil buffer for these pixels will be 0x07.
            gl.StencilFunc(gl.CompareFunc.Less, 0x07, 0x03);
            gl.StencilOperation(gl.StencilOp.Replace, gl.StencilOp.Decrement, gl.StencilOp.Decrement);
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices0);

            // Test 1:
            //
            // Initialize upper-right region. Here, we'll decrement the stencil-buffer
            // values where the stencil test passes but the depth test fails.
            // The stencil test is:
            //
            //    ref    mask   stencil  mask
            //   (0x03 & 0x03) > (0x01 & 0x03)
            //
            // But where the geometry fails the depth test. The stencil values for these
            // pixels will be 0x00.
            gl.StencilFunc(gl.CompareFunc.Greater, 0x03, 0x03);
            gl.StencilOperation(gl.StencilOp.Keep, gl.StencilOp.Decrement, gl.StencilOp.Keep);
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices1);

            // Test 2:
            //
            // Initialize the lower - left region. Here we'll increment (with saturation) the
            // stencil value where both the stencil and depth tests pass. The stencil test
            // for these pixels will be:
            //
            //    ref    mask   stencil  mask
            //   (0x01 & 0x03) = (0x01 & 0x03)
            //
            // The stencil values for these pixels will be 0x02.
            gl.StencilFunc(gl.CompareFunc.Equal, 0x01, 0x03);
            gl.StencilOperation(gl.StencilOp.Keep, gl.StencilOp.Increment, gl.StencilOp.Increment);
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices2);

            // Test 3:
            //
            // Finally, initialize the lower - right region. We'll invert the stencil value
            // where the stencil tests fails. The stencil test for these pixels will be:
            //
            //    ref    mask   stencil  mask
            //   (0x02 & 0x01) = (0x01 & 0x01)
            //
            // The stencil value here will be set to (~((2 ^ s - 1) & 0x01)), (with the 0x01
            // being from the stencil clear value), where 's' is the number of bits in the
            // stencil buffer.
            gl.StencilFunc(gl.CompareFunc.Equal, 0x02, 0x01);
            gl.StencilOperation(gl.StencilOp.Invert, gl.StencilOp.Keep, gl.StencilOp.Keep);
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices3);

            // Since we don't know at compile time how many stencil bits are present, we'll
            // query, and update the value correct value in the stencilValues arrays for
            // the fourth tests. We'll use this value later in rendering.
            stencilValues[3] = (byte)((~(((1 << Framebuffer.Current.StencilBits) - 1) & 0x01)) & 0xff);

            // Use the stencil buffer for controlling where rendering will occur. We
            // disable writing to the stencil buffer so we can test against them without
            // modifying the values we generated.
            gl.StencilMask(0x00);

            for (int i = 0; i < _testCount; i++)
            {
                gl.StencilFunc(gl.CompareFunc.Equal, stencilValues[i], 0xff);
                _uniColor.SetValue(_colors[i]);
                gl.DrawElements(gl.PrimitiveType.Triangles, _indices4);
            }

            // Reset the stencil mask
            gl.StencilMask(0xff);
        }

        public override void Unload()
        {
            _program.Dispose();
        }
    }
}
