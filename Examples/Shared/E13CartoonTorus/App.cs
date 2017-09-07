using System;
using Examples.Shared;
using Ooogles;
using OpenTK;

// Based on 016_cartoon_torus.cpp example from oglplus (http://oglplus.org/)

namespace E13CartoonTorus
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _normals;
        private DataBuffer _indices;
        private TorusGeometry _torus;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;

                attribute vec3 Position;
                attribute vec3 Normal;

                varying vec3 vertNormal;

                void main(void)
                {
                  vertNormal = mat3(ModelMatrix) * Normal;
                  gl_Position = 
                    ProjectionMatrix *
                    CameraMatrix *
                    ModelMatrix *
                    vec4(Position, 1.0);
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec3 vertNormal;

                uniform vec3 LightPos;

                void main(void)
                {
                  float intensity = 2.0 * max(
                    dot(vertNormal,  LightPos) /
                    length(LightPos),
                    0.0);
                  if (!gl_FrontFacing)
                  {
                    gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
                  }
                  else if (intensity > 0.9)
                  {
                    gl_FragColor = vec4(1.0, 0.9, 0.8, 1.0);
                  }
                  else if (intensity > 0.1)
                  {
                    gl_FragColor = vec4(0.7, 0.6, 0.4, 1.0);
                  }
                  else
                  {
                    gl_FragColor = vec4(0.3, 0.2, 0.1, 1.0);
                  }
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

            // Indices
            _indices = new DataBuffer(DataBuffer.Type.Index);
            _indices.Bind();
            _indices.Data(_torus.Indices);

            // Don't need data anymore
            _torus.Clear();

            // Uniforms
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
                Vector3.Zero, 3.5f,
                (float)MathHelper.DegreesToRadians(totalTimeSec * 35),
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 10) * 60));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Update and render the torus
            Matrix4 rotationY = Matrix4.CreateRotationY((float)(totalTimeSec * Math.PI * 0.5));
            Matrix4 rotationX = Matrix4.CreateRotationX((float)(Math.PI * 0.5));
            Matrix4 modelMatrix = rotationX * rotationY;
            _uniModelMatrix.SetValue(ref modelMatrix);
            _torus.DrawWithBoundIndexBuffer();
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), (float)newWidth / newHeight, 1, 30);
            _program.Use();
            _uniProjectionMatrix.SetValue(ref projectionMatrix);
        }

        public override void Unload()
        {
            _indices.Dispose();
            _normals.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}
