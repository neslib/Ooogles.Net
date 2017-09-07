using System;
using System.Runtime.InteropServices;
using Examples.Shared;
using Ooogles;
using OpenTK;

/* Based on MipMap2D.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E04MipMap2D
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private VertexAttribute _attrPosition;
        private VertexAttribute _attrTexCoord;
        private Uniform _uniSampler;
        private Uniform _uniOffset;
        private Texture _texture;

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        private struct Vertex
        {
            public Vector4 pos;
            public Vector2 texCoord;

            public Vertex(Vector4 pos, Vector2 texCoord)
            {
                this.pos = pos;
                this.texCoord = texCoord;
            }
        }

        private readonly Vertex[] _vertices =
        {
            new Vertex(new Vector4(-0.5f,  0.5f, 0, 5), new Vector2(0, 0)),
            new Vertex(new Vector4(-0.5f, -0.5f, 0, 1), new Vector2(0, 1)),
            new Vertex(new Vector4( 0.5f, -0.5f, 0, 1), new Vector2(1, 1)),
            new Vertex(new Vector4( 0.5f,  0.5f, 0, 5), new Vector2(1, 0))
        };

        private readonly ushort[] _indices =
        {
            0, 1, 2,
            0, 2, 3
        };

        public override void Load()
        {
            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform float u_offset;

                attribute vec4 a_position;
                attribute vec2 a_texCoord;

                varying vec2 v_texCoord;

                void main()
                {
                  gl_Position = a_position;
                  gl_Position.x += u_offset;
                  v_texCoord = a_texCoord;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec2 v_texCoord;

                uniform sampler2D s_texture;

                void main()
                {
                  gl_FragColor = texture2D(s_texture, v_texCoord);
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
            _uniSampler = new Uniform(_program, "s_texture");
            _uniOffset = new Uniform(_program, "u_offset");

            // Load the texture
            _texture = TextureUtils.CreateMipmappedTexture2D();

            //
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
                    _attrPosition.SetData(VertexAttribute.DataType.Float, 4, pos, sizeof(Vertex));
                }
                fixed (float* texCoord = &_vertices[0].texCoord.X)
                {
                    _attrTexCoord.SetData(VertexAttribute.DataType.Float, 2, texCoord, sizeof(Vertex));
                }
            }
            _attrPosition.Enable();
            _attrTexCoord.Enable();

            // Bind the texture
            _texture.BindToTextureUnit(0);

            // Set the texture sampler to texture unit to 0
            _uniSampler.SetValue(0);

            // Draw quad with nearest sampling
            _texture.MagnificationFilter = Texture.MagFilter.Nearest;
            _texture.MinificationFilter = Texture.MinFilter.Nearest;
            _uniOffset.SetValue(-0.6f);
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices);

            // Draw quad with trilinear sampling
            _texture.MagnificationFilter = Texture.MagFilter.Linear;
            _texture.MinificationFilter = Texture.MinFilter.LinearMipmapLinear;
            _uniOffset.SetValue(0.6f);
            gl.DrawElements(gl.PrimitiveType.Triangles, _indices);
        }

        public override void Unload()
        {
            // Release resources
            _texture.Dispose();
            _program.Dispose();
        }
    }
}
