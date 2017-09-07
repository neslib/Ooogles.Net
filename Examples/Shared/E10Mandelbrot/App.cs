using System;
using Ooogles;
using OpenTK;

// Based on 005_mandelbrot.cpp example from oglplus (http://oglplus.org/)

namespace E10Mandelbrot
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _coords;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                attribute vec2 Position;
                attribute vec2 Coord;

                varying vec2 vertCoord;

                void main(void)
                {
                  vertCoord = Coord;
                  gl_Position = vec4(Position, 0.0, 1.0);
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision highp float;

                varying vec2 vertCoord;

                const int nclr = 5;

                uniform vec4 clrs[5];

                void main(void)
                {
                  vec2 z = vec2(0.0, 0.0);
                  vec2 c = vertCoord;

                  int i = 0, max = 128;
                  while ((i != max) && (distance(z, c) < 2.0))
                  {
                    vec2 zn = vec2(
                      z.x * z.x - z.y * z.y + c.x,
                      2.0 * z.x * z.y + c.y);
                    z = zn;
                    ++i;
                  }

                  float a = sqrt(float(i) / float(max));

                  for (i = 0; i != (nclr - 1); ++i)
                  {
                    if((a > clrs[i].a) && (a <= clrs[i+1].a))
                    {
                      float m = (a - clrs[i].a) / (clrs[i+1].a - clrs[i].a);
                      gl_FragColor = vec4(
                        mix(clrs[i].rgb, clrs[i+1].rgb, m),
                        1.0
                      );
                      break;
                    }
                  }
                }");
            fragmentShader.Compile();

            _program = new Program();
            _program.AttachShader(vertexShader);
            _program.AttachShader(fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            // Positions
            _verts = new DataBuffer(DataBuffer.Type.Vertex);
            _verts.Bind();
            _verts.Data(new Vector2[] {
                new Vector2(-1, -1),
                new Vector2(-1,  1),
                new Vector2( 1, -1),
                new Vector2( 1,  1)});

            VertexAttribute attr = new VertexAttribute(_program, "Position");
            attr.SetConfig<Vector2>();
            attr.Enable();

            // Mandelbrot coordinates
            _coords = new DataBuffer(DataBuffer.Type.Vertex);
            _coords.Bind();
            _coords.Data(new Vector2[] {
                new Vector2(-1.5f, -0.5f),
                new Vector2(-1.5f,  1.0f),
                new Vector2( 0.5f, -0.5f),
                new Vector2( 0.5f,  1.0f)});

            attr = new VertexAttribute(_program, "Coord");
            attr.SetConfig<Vector2>();
            attr.Enable();

            // Color map
            Uniform uniform = new Uniform(_program, "clrs");
            uniform.SetValues(new Vector4[] {
                new Vector4(0.4f, 0.2f, 1.0f, 0.00f),
                new Vector4(1.0f, 0.2f, 0.2f, 0.30f),
                new Vector4(1.0f, 1.0f, 1.0f, 0.95f),
                new Vector4(1.0f, 1.0f, 1.0f, 0.98f),
                new Vector4(0.1f, 0.1f, 0.1f, 1.00f)});

            gl.Disable(gl.Capability.DepthTest);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color buffer
            gl.Clear(gl.ClearBuffers.Color);

            // Use the program
            _program.Use();
            
            // Draw the rectangle
            gl.DrawArrays(gl.PrimitiveType.TriangleStrip, 4);
        }

        public override void Unload()
        {
            _coords.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}
