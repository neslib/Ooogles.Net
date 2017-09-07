using System;
using System.Runtime.InteropServices;
using Ooogles;
using OpenTK;
using Examples.Shared;

/* Based on ParticleSystem.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E09ParticleSystem
{
    public class App : Examples.Shared.Application
    {
        private const int ParticleCount = 1024;

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct Particle
        {
            public float Lifetime;
            public Vector3 StartPosition;
            public Vector3 EndPosition;
        }

        private Program _program;
        private VertexAttribute _attrLifetime;
        private VertexAttribute _attrStartPos;
        private VertexAttribute _attrEndPos;
        private Uniform _uniTime;
        private Uniform _uniCenterPos;
        private Uniform _uniColor;
        private Uniform _uniSampler;
        private Texture _texture;
        private Particle[] _particles = new Particle[ParticleCount];
        private float _particleTime;
        private Random _random = new Random();

        public override void Load()
        {
            // Initialize the asset manager
            Assets.Initialize(Examples.Assets.Assets.Type);

            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform float u_time;
                uniform float u_pointScale;
                uniform vec3 u_centerPosition;

                attribute float a_lifetime;
                attribute vec3 a_startPosition;
                attribute vec3 a_endPosition;

                varying float v_lifetime;

                void main()
                {
                  if (u_time <= a_lifetime)
                  {
                    gl_Position.xyz = a_startPosition + (u_time * a_endPosition);
                    gl_Position.xyz += u_centerPosition;
                    gl_Position.w = 1.0;
                  }
                  else
                  {
                    gl_Position = vec4(-1000, -1000, 0, 0);
                  }

                  v_lifetime = 1.0 - (u_time / a_lifetime);
                  v_lifetime = clamp(v_lifetime, 0.0, 1.0);
                  gl_PointSize = (v_lifetime * v_lifetime) * u_pointScale;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform vec4 u_color;
                uniform sampler2D s_texture;

                varying float v_lifetime;

                void main()
                {
                  vec4 texColor;
                  texColor = texture2D(s_texture, gl_PointCoord);
                  gl_FragColor = vec4(u_color) * texColor;
                  gl_FragColor.a *= v_lifetime;
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
            _attrLifetime = new VertexAttribute(_program, "a_lifetime");
            _attrStartPos = new VertexAttribute(_program, "a_startPosition");
            _attrEndPos = new VertexAttribute(_program, "a_endPosition");

            // Initialize uniforms
            _uniTime = new Uniform(_program, "u_time");
            _uniCenterPos = new Uniform(_program, "u_centerPosition");
            _uniColor = new Uniform(_program, "u_color");
            _uniSampler = new Uniform(_program, "s_texture");

            // The gl_PointSize vertex shader output does not take screen scale into account.
            // So we scale it ourselves.
            _program.Use();
            new Uniform(_program, "u_pointScale").SetValue(40.0f * Platform.ScreenScale);

            // Set clear color to black
            gl.ClearColor(0, 0, 0, 0);

            // Fill in particle data array
            for (int i = 0; i < ParticleCount; i++)
            {
                Particle particle = new Particle();
                particle.Lifetime = (float)_random.NextDouble();

                double angle = _random.NextDouble() * 2.0 * Math.PI;
                double radius = _random.NextDouble() * 2.0;
                particle.EndPosition = new Vector3((float)(Math.Sin(angle) * radius), (float)(Math.Cos(angle) * radius), 0);

                angle = _random.NextDouble() * 2.0 * Math.PI;
                radius = _random.NextDouble() * 0.25;
                particle.StartPosition = new Vector3((float)(Math.Sin(angle) * radius), (float)(Math.Cos(angle) * radius), 0);

                _particles[i] = particle;
            }

            _particleTime = 1;

            _texture = TextureUtils.LoadTexture("smoke.tga");
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color buffer
            gl.Clear(gl.ClearBuffers.Color);

            // Use the program
            _program.Use();

            // Update uniforms
            _particleTime += deltaTimeSec;
            if (_particleTime >= 1)
            {
                _particleTime = 0;

                // Pick a new start location and color
                Vector3 centerPos = new Vector3((float)_random.NextDouble() - 0.5f, (float)_random.NextDouble() - 0.5f, (float)_random.NextDouble() - 0.5f);
                _uniCenterPos.SetValue(ref centerPos);

                Vector4 color = new Vector4((float)_random.NextDouble(), (float)_random.NextDouble(), (float)_random.NextDouble(), 1);
                _uniColor.SetValue(ref color);
            }

            // Load uniform time variable
            _uniTime.SetValue(_particleTime);

            // Load the vertex attributes
            unsafe
            {
                fixed (float* lifetime = &_particles[0].Lifetime)
                    _attrLifetime.SetData(VertexAttribute.DataType.Float, 1, lifetime, sizeof(Particle));

                fixed (float* startPos = &_particles[0].StartPosition.X)
                    _attrStartPos.SetData(VertexAttribute.DataType.Float, 3, startPos, sizeof(Particle));

                fixed (float* endPos = &_particles[0].EndPosition.X)
                    _attrEndPos.SetData(VertexAttribute.DataType.Float, 3, endPos, sizeof(Particle));
            }
            _attrLifetime.Enable();
            _attrStartPos.Enable();
            _attrEndPos.Enable();

            // Blend particles
            gl.Enable(gl.Capability.Blend);
            gl.BlendFunc(gl.BlendFactor.SrcAlpha, gl.BlendFactor.One);

            // Bind the texture
            _texture.BindToTextureUnit(0);

            // Set the sampler texture unit to 0
            _uniSampler.SetValue(0);

            // Draw the particle points
            gl.DrawArrays(gl.PrimitiveType.Points, ParticleCount);
        }

        public override void Unload()
        {
            _program.Dispose();
            _texture.Dispose();
        }
    }
}
