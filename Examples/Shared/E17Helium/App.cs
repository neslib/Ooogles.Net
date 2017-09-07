using Examples.Shared;
using Ooogles;
using OpenTK;
using System;

// Based on 019_helium.cpp example from oglplus (http://oglplus.org/)

namespace E17Helium
{
    public class App : Examples.Shared.Application
    {
        private class Particle : IDisposable
        {
            private Program _program;
            private DataBuffer _verts;
            private DataBuffer _normals;
            private DataBuffer _indices;
            private SphereGeometry _sphere;
            private Uniform _uniProjectionMatrix;
            private Uniform _uniCameraMatrix;
            private Uniform _uniModelMatrix;
            private Uniform _uniLightPos;

            public Particle(Shader vertexShader, Shader fragmentShader)
            {
                _program = new Program(vertexShader, fragmentShader);
                _program.Link();
                _program.Use();

                // Don't need fragment shader anymore (vertex shader is shared though)
                fragmentShader.Dispose();

                // Initialize uniforms
                _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
                _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
                _uniModelMatrix = new Uniform(_program, "ModelMatrix");
                _uniLightPos = new Uniform(_program, "LightPos");

                _sphere = new SphereGeometry(18, 1.0f);

                // Positions
                _verts = new DataBuffer(DataBuffer.Type.Vertex);
                _verts.Bind();
                _verts.Data(_sphere.Positions);

                VertexAttribute attr = new VertexAttribute(_program, "Position");
                attr.SetConfig<Vector3>();
                attr.Enable();

                // Normals
                _normals = new DataBuffer(DataBuffer.Type.Vertex);
                _normals.Bind();
                _normals.Data(_sphere.Normals);

                attr = new VertexAttribute(_program, "Normal");
                attr.SetConfig<Vector3>();
                attr.Enable();

                // Indices
                _indices = new DataBuffer(DataBuffer.Type.Index);
                _indices.Bind();
                _indices.Data(_sphere.Indices);

                // Don't need data anymore
                _sphere.Clear();
            }

            public void SetProjection(ref Matrix4 projection)
            {
                _program.Use();
                _uniProjectionMatrix.SetValue(ref projection);
            }

            public void SetLightAndCamera(ref Vector3 light, ref Matrix4 projection)
            {
                _program.Use();
                _uniLightPos.SetValue(ref light);
                _uniCameraMatrix.SetValue(ref projection);
            }

            public void Render(Matrix4 model)
            {
                _program.Use();
                _uniModelMatrix.SetValue(ref model);
                _verts.Bind();
                _normals.Bind();
                _sphere.DrawWithBoundIndexBuffer();
            }

            public void Dispose()
            {
                _program.Dispose();
                _verts.Dispose();
                _normals.Dispose();
                _indices.Dispose();
            }
        }

        private Particle _proton;
        private Particle _neutron;
        private Particle _electron;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;

                attribute vec3 Position;
                attribute vec3 Normal;

                varying vec3 vertNormal;
                varying vec3 vertLight;
                varying vec3 vertViewNormal;

                uniform vec3 LightPos;

                void main(void)
                {
                  gl_Position = ModelMatrix * vec4(Position, 1.0);
                  vertNormal = mat3(ModelMatrix) * Normal;
                  vertViewNormal = mat3(CameraMatrix) * vertNormal;
                  vertLight = LightPos - gl_Position.xyz;
                  gl_Position = ProjectionMatrix * CameraMatrix * gl_Position;
                }");
            vertexShader.Compile();

            _proton = new Particle(vertexShader, CreateFragmentShader(@"
                  bool sig = (
                    abs(vertViewNormal.x) < 0.5 &&
                    abs(vertViewNormal.y) < 0.2 
                  ) || (
                    abs(vertViewNormal.y) < 0.5 &&
                    abs(vertViewNormal.x) < 0.2 
                  );
                  vec3 color = vec3(1.0, 0.0, 0.0);
                "));

            _neutron = new Particle(vertexShader, CreateFragmentShader(@"
                  bool sig = false;
                  vec3 color = vec3(0.5, 0.5, 0.5);
                "));

            _electron = new Particle(vertexShader, CreateFragmentShader(@"
                  bool sig = (
                    abs(vertViewNormal.x) < 0.5 &&
                    abs(vertViewNormal.y) < 0.2
                  );
                  vec3 color = vec3(0.0, 0.0, 1.0);
                "));

            // Don't need vertex shader anymore
            vertexShader.Dispose();

            gl.ClearColor(0.3f, 0.3f, 0.3f, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
        }

        private Shader CreateFragmentShader(string source)
        {
            Shader shader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec3 vertNormal;
                varying vec3 vertLight;
                varying vec3 vertViewNormal;

                void main(void)
                {
                  float lighting = dot(
                    vertNormal, 
                    normalize(vertLight));

                  float intensity = clamp(
                    0.4 + lighting * 1.0,
                    0.0,
                    1.0);
                " + source + @"
                  gl_FragColor = sig ? 
                    vec4(1.0, 1.0, 1.0, 1.0):
                    vec4(color * intensity, 1.0);
                }");

            shader.Compile();
            return shader;
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color and depth buffer
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            Vector3 light = new Vector3(8, 8, 8);

            // Set the matrix for camera orbiting the origin
            Matrix4 camera = Utils.OrbitCameraMatrix(
                Vector3.Zero, 21.0f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * 15),
                (float)MathHelper.DegreesToRadians(Math.PI * totalTimeSec * 0.3) * 45);

            Matrix4 nucl = Matrix4.CreateFromAxisAngle(new Vector3(1, 1, 1), (float)(totalTimeSec * 2 * Math.PI));

            _proton.SetLightAndCamera(ref light, ref camera);
            Matrix4 model = Matrix4.CreateTranslation(1.4f, 0, 0);
            _proton.Render(model * nucl);
            model = Matrix4.CreateTranslation(-1.4f, 0, 0);
            _proton.Render(model * nucl);

            _neutron.SetLightAndCamera(ref light, ref camera);
            model = Matrix4.CreateTranslation(0, 0, 1);
            _neutron.Render(model * nucl);
            model = Matrix4.CreateTranslation(0, 0, -1);
            _neutron.Render(model * nucl);

            _electron.SetLightAndCamera(ref light, ref camera);

            Matrix4 rotate = Matrix4.CreateRotationY((float)(totalTimeSec * Math.PI * 1.4));
            model = Matrix4.CreateTranslation(10, 0, 0);
            _electron.Render(model * rotate);

            rotate = Matrix4.CreateRotationX((float)(totalTimeSec * Math.PI * 1.4));
            model = Matrix4.CreateTranslation(0, 0, 10);
            _electron.Render(model * rotate);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)newWidth / newHeight, 1, 50);
            _proton.SetProjection(ref projectionMatrix);
            _neutron.SetProjection(ref projectionMatrix);
            _electron.SetProjection(ref projectionMatrix);
        }

        public override void Unload()
        {
            _proton.Dispose();
            _neutron.Dispose();
            _electron.Dispose();
        }
    }
}
