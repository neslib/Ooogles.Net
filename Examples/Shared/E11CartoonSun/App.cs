using System;
using Ooogles;
using OpenTK;

// Based on 006_cartoon_sun.cpp example from oglplus (http://oglplus.org/)

namespace E11CartoonSun
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private Uniform _uniTime;
        private Uniform _uniSunPos;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                attribute vec2 Position;

                varying vec2 vertPos;

                void main(void)
                {
                  gl_Position = vec4(Position, 0.0, 1.0);
                  vertPos = gl_Position.xy;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform float Time;

                uniform vec2 SunPos;
                uniform vec3 Sun1, Sun2, Sky1, Sky2;

                varying vec2 vertPos;

                void main(void)
                {
                  vec2 v = vertPos - SunPos;
                  float l = length(v);
                  float a = (sin(l) + atan(v.y, v.x)) / 3.1415;
                  if (l < 0.1)
                  {
                    gl_FragColor = vec4(Sun1, 1.0);
                  }
                  else if (floor(mod(18.0 * (Time * 0.1 + 1.0 + a), 2.0)) == 0.0)
                  {
                    gl_FragColor = vec4(mix(Sun1, Sun2, l), 1.0);
                  }
                  else
                  {
                    gl_FragColor = vec4(mix(Sky1, Sky2, l), 1.0);
                  }
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
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

            // Uniforms
            _uniTime = new Uniform(_program, "Time");
            _uniSunPos = new Uniform(_program, "SunPos");

            new Uniform(_program, "Sun1").SetValue(new Vector3(0.95f, 0.85f, 0.60f));
            new Uniform(_program, "Sun2").SetValue(new Vector3(0.90f, 0.80f, 0.20f));
            new Uniform(_program, "Sky1").SetValue(new Vector3(0.90f, 0.80f, 0.50f));
            new Uniform(_program, "Sky2").SetValue(new Vector3(0.80f, 0.60f, 0.40f));

            gl.Disable(gl.Capability.DepthTest);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color buffer
            gl.Clear(gl.ClearBuffers.Color);

            // Use the program
            _program.Use();

            // Update uniforms
            _uniTime.SetValue((float)totalTimeSec);

            double angle = totalTimeSec * Math.PI * 0.1;
            _uniSunPos.SetValue((float)-Math.Cos(angle), (float)Math.Sin(angle));

            // Draw the rectangle
            gl.DrawArrays(gl.PrimitiveType.TriangleStrip, 4);
        }

        public override void Unload()
        {
            _verts.Dispose();
            _program.Dispose();
        }
    }
}