using Examples.Shared;
using Ooogles;
using OpenTK;
using System;
using System.Diagnostics;

// Based on 025_recursive_texture.cpp example from oglplus (http://oglplus.org/)

namespace E22RecursiveTexture
{
    public class App : Examples.Shared.Application
    {
        private const int TextureSize = 512;

        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _normals;
        private DataBuffer _texCoords;
        private DataBuffer _indices;
        private Uniform _uniTexUnit;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;
        private Framebuffer _defaultFramebuffer;
        private Framebuffer[] _framebuffers = new Framebuffer[2];
        private Renderbuffer[] _renderbuffers = new Renderbuffer[2];
        private Texture[] _textures = new Texture[2];
        private int _currentTextureIndex;
        private CubeGeometry _cube;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;

                attribute vec3 Position;
                attribute vec3 Normal;
                attribute vec2 TexCoord;

                varying vec3 vertNormal;
                varying vec3 vertLight;
                varying vec2 vertTexCoord;

                uniform vec3 LightPos;

                void main(void)
                {
                  vertNormal = mat3(ModelMatrix) * Normal;
                  gl_Position = ModelMatrix * vec4(Position, 1.0);
                  vertLight = LightPos - gl_Position.xyz;
                  vertTexCoord = TexCoord;
                  gl_Position = ProjectionMatrix * CameraMatrix * gl_Position;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform sampler2D TexUnit;

                varying vec3 vertNormal;
                varying vec3 vertLight;
                varying vec2 vertTexCoord;

                void main(void)
                {
                  float l = sqrt(length(vertLight));
                  float d = (l > 0.0) ? dot(vertNormal, normalize(vertLight)) / l : 0.0;
                  float i = 0.6 + max(d, 0.0);
                  gl_FragColor = texture2D(TexUnit, vertTexCoord) * i;
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            _cube = new CubeGeometry();

            // Positions
            _verts = new DataBuffer(DataBuffer.Type.Vertex);
            _verts.Bind();
            _verts.Data(_cube.Positions);

            VertexAttribute attr = new VertexAttribute(_program, "Position");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Normals
            _normals = new DataBuffer(DataBuffer.Type.Vertex);
            _normals.Bind();
            _normals.Data(_cube.Normals);

            attr = new VertexAttribute(_program, "Normal");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Texture coordinates
            _texCoords = new DataBuffer(DataBuffer.Type.Vertex);
            _texCoords.Bind();
            _texCoords.Data(_cube.TexCoords);

            attr = new VertexAttribute(_program, "TexCoord");
            attr.SetConfig<Vector2>();
            attr.Enable();

            // Indices
            _indices = new DataBuffer(DataBuffer.Type.Index);
            _indices.Bind();
            _indices.Data(_cube.Indices);

            // Don't need data anymore
            _cube.Clear();

            // Textures, renderbuffers and framebuffers
            _defaultFramebuffer = Framebuffer.Current;
            for (int i = 0; i < 2; i++)
            {
                Texture texture = new Texture();
                texture.BindToTextureUnit(i);
                texture.MinificationFilter = Texture.MinFilter.Linear;
                texture.MagnificationFilter = Texture.MagFilter.Linear;
                texture.WrapS = Texture.WrapMode.Repeat;
                texture.WrapT = Texture.WrapMode.Repeat;
                texture.Reserve(gl.PixelFormat.Rgba, TextureSize, TextureSize);
                _textures[i] = texture;

                Renderbuffer renderbuffer = new Renderbuffer();
                renderbuffer.Bind();
                renderbuffer.Storage(TextureSize, TextureSize, Renderbuffer.Format.Depth16);
                _renderbuffers[i] = renderbuffer;

                Framebuffer framebuffer = new Framebuffer();
                framebuffer.Bind();
                framebuffer.AttachTexture(Framebuffer.Attachment.Color, _textures[i]);
                framebuffer.AttachRenderbuffer(Framebuffer.Attachment.Depth, _renderbuffers[i]);
                Debug.Assert(framebuffer.CompletenessStatus == Framebuffer.Status.Complete);

                _framebuffers[i] = framebuffer;
            }

            // Uniforms
            new Uniform(_program, "LightPos").SetValue(4.0f, 4.0f, -8.0f);

            _uniTexUnit = new Uniform(_program, "TexUnit");
            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");

            gl.ClearColor(1, 1, 1, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
            gl.Enable(gl.Capability.CullFace);
            gl.FrontFace(gl.FaceOrientation.CounterClockwise);
            gl.CullFace(gl.Face.Back);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            int frontIndex = _currentTextureIndex;
            int backIndex = 1 - _currentTextureIndex;
            _currentTextureIndex = backIndex;

            // Use the program
            _program.Use();

            // Render into texture
            _uniTexUnit.SetValue(frontIndex);

            // Set the matrix for camera orbiting the origin
            Matrix4 cameraMatrix = Utils.OrbitCameraMatrix(
                Vector3.Zero, 3.0f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * 35),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 10) * 60));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Set model matrix
            Matrix4 modelMatrix = Matrix4.CreateRotationX((float)(Math.PI * totalTimeSec * 0.5));
            _uniModelMatrix.SetValue(ref modelMatrix);

            // Set projection matrix
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(40), 1, 1, 40);
            _uniProjectionMatrix.SetValue(ref projectionMatrix);

            // Render to framebuffer
            _framebuffers[backIndex].Bind();
            gl.Viewport(TextureSize, TextureSize);
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);
            _cube.DrawWithBoundIndexBuffer();

            // Render textured cube to default framebuffer
            _defaultFramebuffer.Bind();
            gl.Viewport(Width, Height);
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            totalTimeSec *= 0.3;

            // Set the matrix for camera orbiting the origin
            cameraMatrix = Utils.OrbitCameraMatrix(
                Vector3.Zero, 3.0f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * 35),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 10) * 60));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Set projection matrix
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), (float)Width / Height, 1, 40);
            _uniProjectionMatrix.SetValue(ref projectionMatrix);

            // Render
            _cube.DrawWithBoundIndexBuffer();
        }

        public override void Unload()
        {
            for (int i = 0; i < 2; i++)
            {
                _framebuffers[i].Dispose();
                _renderbuffers[i].Dispose();
                _textures[i].Dispose();
            }
            _indices.Dispose();
            _texCoords.Dispose();
            _normals.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}
