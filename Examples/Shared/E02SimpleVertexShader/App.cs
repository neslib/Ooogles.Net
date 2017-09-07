using System;
using Examples.Shared;
using Ooogles;
using OpenTK;

/* Based on Simple_VertexShader.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E02SimpleVertexShader
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private VertexAttribute _attrPosition;
        private VertexAttribute _attrTexCoord;
        private Uniform _uniMvpMatrix;
        private CubeGeometry _cube;
        private float _rotation;

        public override void Load()
        {
            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 u_mvpMatrix;

                attribute vec4 a_position;
                attribute vec2 a_texcoord;

                varying vec2 v_texcoord;

                void main()
                {
                  gl_Position = u_mvpMatrix * a_position;
                  v_texcoord = a_texcoord;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec2 v_texcoord;

                void main()
                {
                  gl_FragColor = vec4(v_texcoord.x, v_texcoord.y, 1.0, 1.0);
                }");
            fragmentShader.Compile();

            // Link shaders into program
            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            // We don't need the shaders anymore. 
            // Note that the shaders won't actually be deleted until the program is deleted.
            vertexShader.Dispose();
            fragmentShader.Dispose();

            // Initialize vertex attributes
            _attrPosition = new VertexAttribute(_program, "a_position");
            _attrTexCoord = new VertexAttribute(_program, "a_texcoord");

            // Initialize uniform
            _uniMvpMatrix = new Uniform(_program, "u_mvpMatrix");

            // Generate the geometry data
            _cube = new CubeGeometry(0.5f);

            // Set initial rotation
            _rotation = 45;

            // Set clear color to black
            gl.ClearColor(0, 0, 0, 0);

            // Enable culling of back-facing polygons
            gl.CullFace(gl.Face.Back);
            gl.Enable(gl.Capability.CullFace);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color buffer
            gl.Clear(gl.ClearBuffers.Color);

            // Use the program
            _program.Use();

            // Set the data for the vertex attributes
            _attrPosition.SetData(_cube.Positions);
            _attrPosition.Enable();

            _attrTexCoord.SetData(_cube.TexCoords);
            _attrTexCoord.Enable();

            // Calculate and set MVP matrix
            _rotation = (_rotation + (deltaTimeSec * 40.0f)) % 360.0f;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), (float)Width / (float)Height, 1, 20);
            Matrix4 translate = Matrix4.CreateTranslation(0, 0, -2);
            Matrix4 rotate = Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 1), MathHelper.DegreesToRadians(_rotation));

            Matrix4 model = rotate * translate;
            Matrix4 Mvp = model * perspective;
            _uniMvpMatrix.SetValue(ref Mvp);

            // Draw the cube
            gl.DrawElements(gl.PrimitiveType.Triangles, _cube.Indices);
        }

        public override void Unload()
        {
            _program.Dispose();
        }
    }
}
