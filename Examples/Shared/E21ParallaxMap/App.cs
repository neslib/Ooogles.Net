using Examples.Shared;
using Ooogles;
using OpenTK;
using System;

// Based on 022_parallax_map.cpp example from oglplus (http://oglplus.org/)

namespace E21ParallaxMap
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _normals;
        private DataBuffer _tangents;
        private DataBuffer _texCoords;
        private DataBuffer _indices;
        private Texture _texture;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;
        private Uniform _uniLightPos;
        private CubeGeometry _cube;

        public override void Load()
        {
            Assets.Initialize(Examples.Assets.Assets.Type);

            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;
                uniform vec3 LightPos;

                attribute vec3 Position;
                attribute vec3 Normal;
                attribute vec3 Tangent;
                attribute vec2 TexCoord;

                varying vec3 vertEye;
                varying vec3 vertLight;
                varying vec3 vertNormal;
                varying vec2 vertTexCoord;
                varying vec3 vertViewTangent;
                varying mat3 NormalMatrix;

                void main(void)
                {
                  vec4 EyePos = 
                    CameraMatrix *
                    ModelMatrix *
                    vec4(Position, 1.0);

                  vertEye = EyePos.xyz;

                  vec3 fragTangent = (
                    CameraMatrix *
                    ModelMatrix *
                    vec4(Tangent, 0.0)).xyz;

                  vertNormal = (
                    CameraMatrix *
                    ModelMatrix *
                    vec4(Normal, 0.0)).xyz;

                  vertLight = (
                    CameraMatrix *
                    vec4(LightPos - vertEye, 1.0)).xyz;

                  NormalMatrix = mat3(
                    fragTangent,
                    cross(vertNormal, fragTangent),
                    vertNormal);

                  vertViewTangent = vec3(
                    dot(NormalMatrix[0], vertEye),
                    dot(NormalMatrix[1], vertEye),
                    dot(NormalMatrix[2], vertEye));

                  vertTexCoord = TexCoord;

                  gl_Position = ProjectionMatrix * EyePos;
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform sampler2D BumpTex;
                uniform int BumpTexWidth;
                uniform int BumpTexHeight;

                float DepthMult = 0.1;

                varying vec3 vertEye;
                varying vec3 vertLight;
                varying vec3 vertNormal;
                varying vec2 vertTexCoord;
                varying vec3 vertViewTangent;
                varying mat3 NormalMatrix;

                void main(void)
                {
                  vec3 ViewTangent = normalize(vertViewTangent);
                  float perp = -dot(normalize(vertEye), vertNormal);

                  float sampleInterval = 1.0 / length(
                    vec2(BumpTexWidth, BumpTexHeight));

                  vec3 sampleStep = ViewTangent * sampleInterval;
                  float prevD = 0.0;
                  float depth = texture2D(BumpTex, vertTexCoord).w;
                  float maxOffs = min((depth * DepthMult) / -ViewTangent.z, 1.0);

                  vec3 viewOffs = vec3(0.0, 0.0, 0.0);
                  vec2 offsTexC = vertTexCoord + viewOffs.xy;

                  while (length(viewOffs) < maxOffs)
                  {
                    if ((offsTexC.x <= 0.0) || (offsTexC.x >= 1.0))
                      break;

                    if ((offsTexC.y <= 0.0) || (offsTexC.y >= 1.0))
                      break;

                    if ((depth * DepthMult * perp) <= -viewOffs.z)
                      break;

                    viewOffs += sampleStep;
                    offsTexC = vertTexCoord + viewOffs.xy;
                    prevD = depth;
                    depth = texture2D(BumpTex, offsTexC).w;
                  }

                  offsTexC = vec2(
                    clamp(offsTexC.x, 0.0, 1.0),
                    clamp(offsTexC.y, 0.0, 1.0));

                  float b = floor(mod(
                    1.0 +
                    floor(mod(offsTexC.x * 16.0, 2.0))+
                    floor(mod(offsTexC.y * 16.0, 2.0)), 2.0));

                  vec3 c = vec3(b, b, b);
                  vec3 n = texture2D(BumpTex, offsTexC).xyz;
                  vec3 finalNormal = NormalMatrix * n;
                  float l = length(vertLight);

                  float d = (l > 0.0) ? dot(
                    normalize(vertLight), 
                    finalNormal) / l : 0.0;

                  float i = 0.1 + 2.5 * max(d, 0.0);
                  gl_FragColor = vec4(c * i, 1.0);
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

            // Tangents
            _tangents = new DataBuffer(DataBuffer.Type.Vertex);
            _tangents.Bind();
            _tangents.Data(_cube.Tangents);

            attr = new VertexAttribute(_program, "Tangent");
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

            // Texture
            _texture = TextureUtils.LoadTexture("texture.tga");
            _texture.WrapS = Texture.WrapMode.Repeat;
            _texture.WrapT = Texture.WrapMode.Repeat;

            // Uniforms
            new Uniform(_program, "BumpTexWidth").SetValue(512);
            new Uniform(_program, "BumpTexHeight").SetValue(512);
            new Uniform(_program, "BumpTex").SetValue(0);

            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");
            _uniLightPos = new Uniform(_program, "LightPos");

            gl.ClearColor(0.1f, 0.1f, 0.1f, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
            gl.Enable(gl.Capability.CullFace);
            gl.FrontFace(gl.FaceOrientation.CounterClockwise);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color and depth buffer
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            // Use the program
            _program.Use();

            // Set light position
            double lightAzimuth = -Math.PI * totalTimeSec;
            float s = (float)Math.Sin(lightAzimuth);
            float c = (float)Math.Cos(lightAzimuth);
            _uniLightPos.SetValue(-c * 2, 2, -s * 2);

            // Set the matrix for camera orbiting the origin
            Matrix4 cameraMatrix = Utils.OrbitCameraMatrix(
                Vector3.Zero, 3.0f,
                MathHelper.DegreesToRadians(-45.0f),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 15) * 70));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Update and render the cube
            Matrix4 modelMatrix = Matrix4.CreateFromAxisAngle(new Vector3(1, 1, 1), (float)(-Math.PI * totalTimeSec * 0.025));
            _uniModelMatrix.SetValue(ref modelMatrix);
            gl.CullFace(gl.Face.Back);
            _cube.DrawWithBoundIndexBuffer();
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(54), (float)newWidth / newHeight, 1, 10);
            _program.Use();
            _uniProjectionMatrix.SetValue(ref projectionMatrix);
        }

        public override void Unload()
        {
            _indices.Dispose();
            _texCoords.Dispose();
            _tangents.Dispose();
            _normals.Dispose();
            _verts.Dispose();
            _program.Dispose();
            _texture.Dispose();
        }
    }
}
