using Examples.Shared;
using Ooogles;
using OpenTK;
using System;

// Based on 020_shaded_objects.cpp example from oglplus (http://oglplus.org/)

namespace E19ShadedObjects
{
    public class App : Examples.Shared.Application
    {
        private abstract class Shape : IDisposable
        {
            private Program _program;
            private DataBuffer _verts;
            private DataBuffer _normals;
            private DataBuffer _texCoords;
            private DataBuffer _indices;
            private VertexAttribute _attrVerts;
            private VertexAttribute _attrNormals;
            private VertexAttribute _attrTexCoords;
            private Uniform _uniProjectionMatrix;
            private Uniform _uniCameraMatrix;
            private Uniform _uniModelMatrix;
            private Uniform _uniLightPos;

            public Shape(Shader vertexShader, Shader fragmentShader)
            {
                _program = new Program(vertexShader, fragmentShader);
                _program.Link();
                _program.Use();

                // Fragment shader no longer needed. Vertex shader is shared though.
                fragmentShader.Dispose();
            }

            public void Init(Vector3[] verts, Vector3[] normals, Vector2[] texCoords, ushort[] indices)
            {
                // Positions
                _verts = new DataBuffer(DataBuffer.Type.Vertex);
                _verts.Bind();
                _verts.Data(verts);

                _attrVerts = new VertexAttribute(_program, "Position");

                // Normals
                _normals = new DataBuffer(DataBuffer.Type.Vertex);
                _normals.Bind();
                _normals.Data(normals);

                _attrNormals = new VertexAttribute(_program, "Normal");

                // Texture coordinates
                _texCoords = new DataBuffer(DataBuffer.Type.Vertex);
                _texCoords.Bind();
                _texCoords.Data(texCoords);

                _attrTexCoords = new VertexAttribute(_program, "TexCoord");

                // Indices
                _indices = new DataBuffer(DataBuffer.Type.Index);
                _indices.Bind();
                _indices.Data(indices);

                // Uniforms
                _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
                _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
                _uniModelMatrix = new Uniform(_program, "ModelMatrix");
                _uniLightPos = new Uniform(_program, "LightPos");
            }

            public void SetProjection(ref Matrix4 projection)
            {
                _program.Use();
                _uniProjectionMatrix.SetValue(ref projection);
            }

            public virtual void Render(ref Vector3 light, ref Matrix4 camera, ref Matrix4 model)
            {
                _program.Use();

                _uniLightPos.SetValue(ref light);
                _uniCameraMatrix.SetValue(ref camera);
                _uniModelMatrix.SetValue(ref model);

                _verts.Bind();
                _attrVerts.SetConfig(VertexAttribute.DataType.Float, 3);
                _attrVerts.Enable();

                _normals.Bind();
                _attrNormals.SetConfig(VertexAttribute.DataType.Float, 3);
                _attrNormals.Enable();

                _texCoords.Bind();
                _attrTexCoords.SetConfig(VertexAttribute.DataType.Float, 2);
                _attrTexCoords.Enable();

                _indices.Bind();
            }

            public void Dispose()
            {
                _program.Dispose();
                _verts.Dispose();
                _normals.Dispose();
                _texCoords.Dispose();
                _indices.Dispose();
            }
        }

        private class Sphere : Shape
        {
            private SphereGeometry _sphere;

            public Sphere(Shader vertexShader, Shader fragmentShader) : base(vertexShader, fragmentShader)
            {
                _sphere = new SphereGeometry();
                Init(_sphere.Positions, _sphere.Normals, _sphere.TexCoords, _sphere.Indices);
                _sphere.Clear();
            }

            public override void Render(ref Vector3 light, ref Matrix4 camera, ref Matrix4 model)
            {
                base.Render(ref light, ref camera, ref model);
                _sphere.DrawWithBoundIndexBuffer();
            }
        }

        private class Cube : Shape
        {
            private CubeGeometry _cube;

            public Cube(Shader vertexShader, Shader fragmentShader) : base(vertexShader, fragmentShader)
            {
                _cube = new CubeGeometry();
                Init(_cube.Positions, _cube.Normals, _cube.TexCoords, _cube.Indices);
                _cube.Clear();
            }

            public override void Render(ref Vector3 light, ref Matrix4 camera, ref Matrix4 model)
            {
                base.Render(ref light, ref camera, ref model);
                _cube.DrawWithBoundIndexBuffer();
            }
        }

        private class Torus : Shape
        {
            private TorusGeometry _torus;

            public Torus(Shader vertexShader, Shader fragmentShader) : base(vertexShader, fragmentShader)
            {
                _torus = new TorusGeometry();
                Init(_torus.Positions, _torus.Normals, _torus.TexCoords, _torus.Indices);
                _torus.Clear();
            }

            public override void Render(ref Vector3 light, ref Matrix4 camera, ref Matrix4 model)
            {
                base.Render(ref light, ref camera, ref model);
                _torus.DrawWithBoundIndexBuffer();
            }
        }

        private Sphere _sphere;
        private Cube _cubeX;
        private Cube _cubeY;
        private Cube _cubeZ;
        private Torus _torus;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;

                attribute vec3 Position;
                attribute vec3 Normal;
                attribute vec2 TexCoord;

                varying vec2 vertTexCoord;
                varying vec3 vertNormal;
                varying vec3 vertLight;

                uniform vec3 LightPos;

                void main(void)
                {
                  vertTexCoord = TexCoord;
                  gl_Position = ModelMatrix * vec4(Position, 1.0);
                  vertNormal = mat3(ModelMatrix) * Normal;
                  vertLight = LightPos - gl_Position.xyz;
                  gl_Position = ProjectionMatrix * CameraMatrix * gl_Position;
                }");
            vertexShader.Compile();

            _sphere = new Sphere(vertexShader, CreateFragmentShader(@"
                  float m = floor(mod((vertTexCoord.x + vertTexCoord.y) * 16.0, 2.0));
                  vec3 Color = mix(
                    vec3(0.0, 0.0, 0.0),
                    vec3(1.0, 1.0, 0.0),
                    m);
                "));

            _cubeX = new Cube(vertexShader, CreateFragmentShader(@"
                  float c = floor(mod(
                    1.0 +
                    floor(mod(vertTexCoord.x * 8.0, 2.0)) +
                    floor(mod(vertTexCoord.y * 8.0, 2.0)), 
                    2.0));
                  vec3 Color = vec3(c, c, c);   
                "));

            _cubeY = new Cube(vertexShader, CreateFragmentShader(@"
                  vec2 center = vertTexCoord - vec2(0.5, 0.5);
                  float m = floor(mod(sqrt(length(center)) * 16.0, 2.0));
                  vec3 Color = mix(
                    vec3(1.0, 0.0, 0.0),
                    vec3(0.0, 0.0, 1.0),
                    m);
                "));

            _cubeZ = new Cube(vertexShader, CreateFragmentShader(@"
                  vec2  center = (vertTexCoord - vec2(0.5, 0.5)) * 16.0;
                  float l = length(center);
                  float t = atan(center.y, center.x) / (2.0 * asin(1.0));
                  float m = floor(mod(l + t, 2.0));
                  vec3 Color = mix(
                    vec3(0.0, 1.0, 0.0),
                    vec3(1.0, 1.0, 1.0),
                    m);
                "));

            _torus = new Torus(vertexShader, CreateFragmentShader(@"
                  float m = floor(mod(vertTexCoord.x * 8.0, 2.0));
                  vec3 Color = mix(
                    vec3(1.0, 0.6, 0.1),
                    vec3(1.0, 0.9, 0.8),
                    m);
                "));

            // Vertex shader no longer needed
            vertexShader.Dispose();

            gl.ClearColor(0.5f, 0.5f, 0.5f, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
        }

        private Shader CreateFragmentShader(string source)
        {
            Shader shader = new Shader(Shader.Type.Fragment);
            shader.Source = @"
                precision mediump float;

                varying vec2 vertTexCoord;
                varying vec3 vertNormal;
                varying vec3 vertLight;

                void main(void)
                {
                  float Len = dot(vertLight, vertLight);
                  float Dot = Len > 0.0 ? dot(
                    vertNormal, 
                    normalize(vertLight)
                  ) / Len : 0.0;
                  float Intensity = 0.2 + max(Dot, 0.0) * 4.0;
                " + source + @"
                  gl_FragColor = vec4(Color * Intensity, 1.0);
                }";

            shader.Compile();
            return shader;
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color and depth buffer
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            Vector3 light = new Vector3(2, 2, 2);

            // Set the matrix for camera orbiting the origin
            Matrix4 camera = Utils.OrbitCameraMatrix(
                Vector3.Zero, 6.0f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * 15),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 3) * 45));

            // Render the shapes
            Matrix4 model = Matrix4.Identity;
            _sphere.Render(ref light, ref camera, ref model);

            model = Matrix4.CreateTranslation(2, 0, 0);
            Matrix4 rotate = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(totalTimeSec * 45));
            model = rotate * model;
            _cubeX.Render(ref light, ref camera, ref model);

            model = Matrix4.CreateTranslation(0, 2, 0);
            rotate = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(totalTimeSec * 90));
            model = rotate * model;
            _cubeY.Render(ref light, ref camera, ref model);

            model = Matrix4.CreateTranslation(0, 0, 2);
            rotate = Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(totalTimeSec * 135));
            model = rotate * model;
            _cubeZ.Render(ref light, ref camera, ref model);

            model = Matrix4.CreateTranslation(-1, -1, -1);
            rotate = Matrix4.CreateFromAxisAngle(new Vector3(1, 1, 1), (float)MathHelper.DegreesToRadians(totalTimeSec * 45));
            Matrix4 m1 = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(45));
            Matrix4 m2 = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(45));
            model = m2 * m1 * rotate * model;
            _torus.Render(ref light, ref camera, ref model);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), (float)newWidth / newHeight, 1, 50);
            _sphere.SetProjection(ref projectionMatrix);
            _cubeX.SetProjection(ref projectionMatrix);
            _cubeY.SetProjection(ref projectionMatrix);
            _cubeZ.SetProjection(ref projectionMatrix);
            _torus.SetProjection(ref projectionMatrix);
        }

        public override void Unload()
        {
            _sphere.Dispose();
            _cubeX.Dispose();
            _cubeY.Dispose();
            _cubeZ.Dispose();
            _torus.Dispose();
        }
    }
}
