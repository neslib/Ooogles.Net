using Examples.Shared;
using Ooogles;
using OpenTK;
using System;

// Based on 020_cube_mapping.cpp example from oglplus (http://oglplus.org/)

namespace E18CubeMapping
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _normals;
        private Texture _texture;
        private SpiralSphereGeometry _shape;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;

        public override void Load()
        {
            Assets.Initialize(Examples.Assets.Assets.Type);

            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;

                attribute vec3 Position;
                attribute vec3 Normal;
                attribute vec2 TexCoord;

                varying vec3 vertNormal;
                varying vec3 vertLightDir;
                varying vec3 vertLightRefl;
                varying vec3 vertViewDir;
                varying vec3 vertViewRefl;

                uniform vec3 LightPos;

                void main(void)
                {
                  gl_Position = ModelMatrix * vec4(Position, 1.0);
                  vertNormal = mat3(ModelMatrix) * Normal;
                  vertLightDir = LightPos - gl_Position.xyz;

                  vertLightRefl = reflect(
                    -normalize(vertLightDir),
                    normalize(vertNormal));

                  vertViewDir = (
                    vec4(0.0, 0.0, 1.0, 1.0)*
                    CameraMatrix).xyz;

                  vertViewRefl = reflect(
                    normalize(vertViewDir),
                    normalize(vertNormal));

                  gl_Position = ProjectionMatrix * CameraMatrix * gl_Position;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform samplerCube TexUnit;

                varying vec3 vertNormal;
                varying vec3 vertLightDir;
                varying vec3 vertLightRefl;
                varying vec3 vertViewDir;
                varying vec3 vertViewRefl;

                void main(void)
                {
                  float l = length(vertLightDir);

                  float d = dot(
                    normalize(vertNormal), 
                    normalize(vertLightDir)) / l;

                  float s = dot(
                    normalize(vertLightRefl),
                    normalize(vertViewDir));

                  vec3 lt = vec3(1.0, 1.0, 1.0);
                  vec3 env = textureCube(TexUnit, vertViewRefl).rgb;

                  gl_FragColor = vec4(
                    env * 0.4 + 
                    (lt + env) * 1.5 * max(d, 0.0) + 
                    lt * pow(max(s, 0.0), 64.0), 
                    1.0);
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            _shape = new SpiralSphereGeometry();

            // Positions
            _verts = new DataBuffer(DataBuffer.Type.Vertex);
            _verts.Bind();
            _verts.Data(_shape.Positions);

            VertexAttribute attr = new VertexAttribute(_program, "Position");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Normals
            _normals = new DataBuffer(DataBuffer.Type.Vertex);
            _normals.Bind();
            _normals.Data(_shape.Normals);

            attr = new VertexAttribute(_program, "Normal");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Don't need data anymore
            _shape.Clear();

            // Texture
            TgaImage image = new TgaImage("Newton.tga");
            _texture = new Texture(Texture.Type.CubeMap);
            _texture.Bind();
            _texture.MinificationFilter = Texture.MinFilter.Linear;
            _texture.MagnificationFilter = Texture.MagFilter.Linear;
            _texture.WrapS = Texture.WrapMode.ClampToEdge;
            _texture.WrapT = Texture.WrapMode.ClampToEdge;
            for (int i = 0; i < 6; i++)
            {
                _texture.Upload(gl.PixelFormat.Rgba, image.Width, image.Height, image.Data, 0, gl.PixelDataType.UnsignedByte, i);
            }

            // Uniforms
            new Uniform(_program, "TexUnit").SetValue(0);
            new Uniform(_program, "LightPos").SetValue(3.0f, 5.0f, 4.0f);

            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");

            gl.ClearColor(0.2f, 0.05f, 0.1f, 0);
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
                Vector3.Zero, 
                (float)(4.5 - Math.Sin(totalTimeSec * Math.PI / 8) * 2),
                (float)(totalTimeSec * Math.PI / 6),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 15) * 90));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Update and render the sphere
            Matrix4 modelMatrix = Matrix4.CreateFromAxisAngle(new Vector3(1, 1, 1), (float)(totalTimeSec * Math.PI / 5));
            _uniModelMatrix.SetValue(ref modelMatrix);

            _shape.Draw();
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)newWidth / newHeight, 1, 100);
            _program.Use();
            _uniProjectionMatrix.SetValue(ref projectionMatrix);
        }

        public override void Unload()
        {
            _texture.Dispose();
            _normals.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}
