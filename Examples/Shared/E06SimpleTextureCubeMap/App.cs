using System;
using Ooogles;
using Examples.Shared;

/* Based on Simple_TextureCubemap.c from
   Book:      OpenGL(R) ES 2.0 Programming Guide
   Authors:   Aaftab Munshi, Dan Ginsburg, Dave Shreiner
   ISBN-10:   0321502795
   ISBN-13:   9780321502797
   Publisher: Addison-Wesley Professional
   URLs:      http://safari.informit.com/9780321563835
              http://www.opengles-book.com */

namespace E06SimpleTextureCubeMap
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private VertexAttribute _attrPosition;
        private VertexAttribute _attrNormal;
        private Uniform _uniSampler;
        private Texture _texture;
        private SphereGeometry _sphere;

        public override void Load()
        {
            // Compile vertex and fragment shaders
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                attribute vec4 a_position;
                attribute vec3 a_normal;

                varying vec3 v_normal;

                void main()
                {
                  gl_Position = a_position;
                  v_normal = a_normal;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec3 v_normal;

                uniform samplerCube s_texture;

                void main()
                {
                  gl_FragColor = textureCube(s_texture, v_normal);
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
            _attrNormal = new VertexAttribute(_program, "a_normal");

            // Initialize uniforms
            _uniSampler = new Uniform(_program, "s_texture");
            
            // Load the texture
            _texture = TextureUtils.CreateSimpleTextureCubeMap();

            // Generate the geometry data
            _sphere = new SphereGeometry(128, 0.75f);

            // Set clear color to black
            gl.ClearColor(0, 0, 0, 0);

            // Enable culling
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
            _attrPosition.SetData(_sphere.Positions);
            _attrPosition.Enable();
            _attrNormal.SetData(_sphere.Normals);
            _attrNormal.Enable();

            // Bind the texture
            _texture.BindToTextureUnit(0);

            // Set the texture sampler to texture unit to 0
            _uniSampler.SetValue(0);

            // Draw the sphere
            _sphere.DrawWithIndices();
        }

        public override void Unload()
        {
            // Release resources
            _texture.Dispose();
            _program.Dispose();
        }
    }
}
