using System;
using Examples.Shared;
using Ooogles;
using OpenTK;

// Based on 016_noise_torus.cpp example from oglplus (http://oglplus.org/)

namespace E15NoiseTorus
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _normals;
        private DataBuffer _texCoords;
        private DataBuffer _indices;
        private TorusGeometry _torus;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;
        private Texture _texture;

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
                  gl_Position = ModelMatrix * vec4(Position, 1.0);
                  vertNormal = mat3(ModelMatrix)*Normal;
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
                  float d = (l > 0.0) ? dot(
                    vertNormal, 
                    normalize(vertLight)
                  ) / l : 0.0;
                  float i = 0.2 + 3.2 * max(d, 0.0);
                  gl_FragColor = texture2D(TexUnit, vertTexCoord) * i;
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            _torus = new TorusGeometry(72, 48, 1.0f, 0.5f);

            // Positions
            _verts = new DataBuffer(DataBuffer.Type.Vertex);
            _verts.Bind();
            _verts.Data(_torus.Positions);

            VertexAttribute attr = new VertexAttribute(_program, "Position");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Normals
            _normals = new DataBuffer(DataBuffer.Type.Vertex);
            _normals.Bind();
            _normals.Data(_torus.Normals);

            attr = new VertexAttribute(_program, "Normal");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Texture coordinates
            _texCoords = new DataBuffer(DataBuffer.Type.Vertex);
            _texCoords.Bind();
            _texCoords.Data(_torus.TexCoords);

            attr = new VertexAttribute(_program, "TexCoord");
            attr.SetConfig<Vector2>();
            attr.Enable();

            // Indices
            _indices = new DataBuffer(DataBuffer.Type.Index);
            _indices.Bind();
            _indices.Data(_torus.Indices);

            // Don't need data anymore
            _torus.Clear();

            // Texture
            Random random = new Random();
            byte[,] texData = new byte[256,256];
            for (int v = 0; v < 256; v++)
            {
                for (int u = 0; u < 256; u++)
                {
                    texData[v, u] = (byte)random.Next(255);
                }
            }

            _texture = new Texture();
            _texture.Bind();
            _texture.MinificationFilter = Texture.MinFilter.Linear;
            _texture.MagnificationFilter = Texture.MagFilter.Linear;
            _texture.WrapS = Texture.WrapMode.Repeat;
            _texture.WrapT = Texture.WrapMode.Repeat;
            _texture.Upload(gl.PixelFormat.Luminance, 256, 256, texData);

            // Uniforms
            new Uniform(_program, "TexUnit").SetValue(0);
            new Uniform(_program, "LightPos").SetValue(4.0f, 4.0f, -8.0f);

            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");

            gl.ClearColor(0.8f, 0.8f, 0.7f, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
            gl.Enable(gl.Capability.CullFace);
            gl.FrontFace(gl.FaceOrientation.CounterClockwise);
            gl.CullFace(gl.Face.Back);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color and depth buffer
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            // Use the program
            _program.Use();

            // Set the matrix for camera orbiting the origin
            Matrix4 cameraMatrix = Utils.OrbitCameraMatrix(
                Vector3.Zero, 4.5f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * 35),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 10) * 60));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Update and render the torus
            Matrix4 modelMatrix = Matrix4.CreateRotationX((float)(totalTimeSec * Math.PI * 0.5));
            _uniModelMatrix.SetValue(ref modelMatrix);
            _torus.DrawWithBoundIndexBuffer();
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)newWidth / newHeight, 1, 20);
            _program.Use();
            _uniProjectionMatrix.SetValue(ref projectionMatrix);
        }

        public override void Unload()
        {
            _indices.Dispose();
            _normals.Dispose();
            _texCoords.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}