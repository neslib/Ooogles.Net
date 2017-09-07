using System;
using Examples.Shared;
using Ooogles;
using OpenTK;

// Based on 016_metallic_torus.cpp example from oglplus (http://oglplus.org/)

namespace E14MetallicTorus
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
                  vertNormal = mat3(CameraMatrix) * mat3(ModelMatrix) * Normal;
                  gl_Position = 
                    ProjectionMatrix *
                    CameraMatrix *
                    ModelMatrix *
                    vec4(Position, 1.0);
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                uniform int ColorCount;
                uniform vec4 Color[8];

                varying vec3 vertNormal;

                vec3 ViewDir = vec3(0.0, 0.0, 1.0);
                vec3 TopDir = vec3(0.0, 1.0, 0.0);

                void main(void)
                {
                  float k = dot(vertNormal, ViewDir);
                  vec3 reflDir = 2.0 * k * vertNormal - ViewDir;
                  float a = dot(reflDir, TopDir);
                  vec3 reflColor = vec3(0.0);

                  for(int i = 0; i != (ColorCount - 1); ++i)
                  {
                    if ((a<Color[i].a) && (a >= Color[i+1].a))
                    {
                      float m = 
                        (a - Color[i].a) / 
                        (Color[i+1].a - Color[i].a);
                      reflColor = mix(
                        Color[i].rgb,
                        Color[i+1].rgb,
                        m
                      );
                      break;
                    }
                  }
                  float i = max(dot(vertNormal, TopDir), 0.0);
                  vec3 diffColor = vec3(i, i, i);
                  gl_FragColor = vec4(
                    mix(reflColor, diffColor, 0.3 + i * 0.7),
                    1.0
                  );
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
            new Uniform(_program, "ColorCount").SetValue(8);
            new Uniform(_program, "Color").SetValues(new Vector4[] {
                new Vector4(1.0f, 1.0f, 0.9f,  1.00f),
                new Vector4(1.0f, 0.9f, 0.8f,  0.97f),
                new Vector4(0.9f, 0.7f, 0.5f,  0.95f),
                new Vector4(0.5f, 0.5f, 1.0f,  0.95f),
                new Vector4(0.2f, 0.2f, 0.7f,  0.00f),
                new Vector4(0.1f, 0.1f, 0.1f,  0.00f),
                new Vector4(0.2f, 0.2f, 0.2f, -0.10f),
                new Vector4(0.5f, 0.5f, 0.5f, -1.00f)});

            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");

            gl.ClearColor(0.1f, 0.1f, 0.1f, 0);
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
                (float)MathHelper.DegreesToRadians(Math.Sin(Math.PI * totalTimeSec / 30) * 80));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Update and render the torus
            Matrix4 modelMatrix = Matrix4.CreateRotationX((float)(totalTimeSec * Math.PI * 0.5));
            _uniModelMatrix.SetValue(ref modelMatrix);
            _torus.DrawWithBoundIndexBuffer();
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), (float)newWidth / newHeight, 1, 20);
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
