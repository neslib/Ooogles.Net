using Examples.Shared;
using Ooogles;
using OpenTK;
using System;

// Based on 017_phong_torus.cpp example from oglplus (http://oglplus.org/)

namespace E16PhongTorus
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _normals;
        private DataBuffer _colors;
        private TwistedTorusGeometry _torus;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix;

                attribute vec3 Position;
                attribute vec3 Normal;
                attribute vec3 Color;

                varying vec3 vertColor;
                varying vec3 vertNormal;
                varying vec3 vertViewDir;

                void main(void)
                {
                  vertColor = normalize(vec3(1.0, 1.0, 1.0) - Color);
                  vertNormal = Normal;
                  vertViewDir = (vec4(0.0, 0.0, 1.0, 1.0) * CameraMatrix).xyz;
                  gl_Position = ProjectionMatrix * CameraMatrix * vec4(Position, 1.0);
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec3 vertColor;
                varying vec3 vertNormal;
                varying vec3 vertViewDir;

                uniform vec3 LightPos[3];

                void main(void)
                {
                  float amb = 0.2;
                  float diff = 0.0;
                  float spec = 0.0;
                  for (int i=0; i != 3; ++i)
                  {
                    diff += max(
                      dot(vertNormal,  LightPos[i]) /
                      dot(LightPos[i], LightPos[i]), 
                      0.0);
                    float k = dot(vertNormal, LightPos[i]);
                    vec3 r = 2.0*k*vertNormal - LightPos[i];
                    spec += pow(max(
                      dot(normalize(r), vertViewDir),
                      0.0), 32.0 * dot(r, r));
                  }
                  gl_FragColor = 
                    vec4(vertColor, 1.0) * (amb + diff) +
                    vec4(1.0, 1.0, 1.0, 1.0) * spec;
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            _torus = new TwistedTorusGeometry();

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

            // Colors
            _colors = new DataBuffer(DataBuffer.Type.Vertex);
            _colors.Bind();
            _colors.Data(_torus.Tangents);

            attr = new VertexAttribute(_program, "Color");
            attr.SetConfig<Vector3>();
            attr.Enable();

            // Don't need data anymore
            _torus.Clear();

            // Uniforms
            new Uniform(_program, "LightPos").SetValues(new Vector3[] {
                new Vector3(2, -1,  0),
                new Vector3(0,  3, -1),
                new Vector3(0, -1,  4)});

            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");

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
                Vector3.Zero, 5.0f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * Math.PI * 2),
                (float)MathHelper.DegreesToRadians(Math.PI * totalTimeSec / 8) * 90);
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Render the torus
            _torus.Draw();
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)newWidth / newHeight, 1, 30);
            _program.Use();
            _uniProjectionMatrix.SetValue(ref projectionMatrix);
        }

        public override void Unload()
        {
            _colors.Dispose();
            _normals.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}
