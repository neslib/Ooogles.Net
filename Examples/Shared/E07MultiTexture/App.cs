using System;
using System.Runtime.InteropServices;
using Examples.Shared;
using Ooogles;
using OpenTK;

/* Based on MultiTexture.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E07MultiTexture
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private VertexAttribute _attrPosition;
        private VertexAttribute _attrTexCoord;
        private Uniform _uniBaseMap;
        private Uniform _uniLightMap;
        private Texture _texBaseMap;
        private Texture _texLightMap;

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct Vertex
        {
            public Vector3 pos;
            public Vector2 texCoord;

            public Vertex(Vector3 pos, Vector2 texCoord)
            {
                this.pos = pos;
                this.texCoord = texCoord;
            }
        }

        private readonly Vertex[] _vertices =
        {
            new Vertex(new Vector3(-0.5f,  0.5f, 0), new Vector2(0, 0)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
            new Vertex(new Vector3( 0.5f, -0.5f, 0), new Vector2(1, 1)),
            new Vertex(new Vector3( 0.5f,  0.5f, 0), new Vector2(1, 0))
        };

        private readonly ushort[] _indices =
        {
            0, 1, 2,
            0, 2, 3
        };

        public override void Load()
        {
            // Initialize the asset manager
            Assets.Initialize(Examples.Assets.Assets.Type);

            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                attribute vec4 a_position;
                attribute vec2 a_texCoord;

                varying vec2 v_texCoord;

                void main()
                {
                  gl_Position = a_position;
                  v_texCoord = a_texCoord;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec2 v_texCoord;

                uniform sampler2D s_baseMap;
                uniform sampler2D s_lightMap;

                void main()
                {
                  vec4 baseColor;
                  vec4 lightColor;

                  baseColor = texture2D(s_baseMap, v_texCoord);
                  lightColor = texture2D(s_lightMap, v_texCoord);

                  gl_FragColor = baseColor * (lightColor + 0.25);
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
            _attrTexCoord = new VertexAttribute(_program, "a_texCoord");

            // Initialize uniforms
            _uniBaseMap = new Uniform(_program, "s_baseMap");
            _uniLightMap = new Uniform(_program, "s_lightMap");

            // Load the textures
            _texBaseMap = TextureUtils.LoadTexture("basemap.tga");
            _texLightMap = TextureUtils.LoadTexture("lightmap.tga");

            // Set clear color to black
            gl.ClearColor(0, 0, 0, 0);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color buffer
            gl.Clear(gl.ClearBuffers.Color);

            // Use the program
            _program.Use();

            // Set the data for the vertex attributes
            unsafe
            {
                fixed (float* pos = &_vertices[0].pos.X)
                {
                    _attrPosition.SetData(VertexAttribute.DataType.Float, 3, pos, sizeof(Vertex));
                }
                fixed (float* texCoord = &_vertices[0].texCoord.X)
                {
                    _attrTexCoord.SetData(VertexAttribute.DataType.Float, 2, texCoord, sizeof(Vertex));
                }
            }
            _attrPosition.Enable();
            _attrTexCoord.Enable();

            // Bind the base map
            _texBaseMap.BindToTextureUnit(0);

            // Set the base map sampler to texture unit to 0
            _uniBaseMap.SetValue(0);

            // Bind the light map
            _texLightMap.BindToTextureUnit(1);

            // Set the light map sampler to texture unit to 1
            _uniLightMap.SetValue(1);

            // Draw the quad
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices);
        }

        public override void Unload()
        {
            // Release resources
            _texBaseMap.Dispose();
            _texLightMap.Dispose();
            _program.Dispose();
        }
    }
}
