using Examples.Shared;
using Ooogles;
using OpenTK;
using System;

// Based on 021_morphing.cpp example from oglplus (http://oglplus.org/)

namespace E20Morphing
{
    public class App : Examples.Shared.Application
    {
        private const int PointCount = 4096;

        private Program _program;
        private DataBuffer[] _Vbos = new DataBuffer[4];
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;
        private Uniform _uniStatus;
        private float _status;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;
                uniform vec3 Color1, Color2;
                uniform float Status, ScreenScale;

                attribute vec4 Position1, Position2;
                attribute float Radiance1, Radiance2;

                varying vec3 vertColor;

                void main(void)
                {
                  gl_Position = 
                    ProjectionMatrix * 
                    CameraMatrix * 
                    ModelMatrix * 
                    mix(Position1, Position2, Status);

                  gl_PointSize = (2.0 + 3.0 * mix(
                    Radiance1, 
                    Radiance2, 
                    Status)) * ScreenScale;

                  vertColor = mix(
                    (0.2 + Radiance1) * Color1,
                    (0.2 + Radiance2) * Color2,
                    Status);
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec3 vertColor;

                void main(void)
                {
                  gl_FragColor = vec4(vertColor, 1.0);
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            _Vbos[0] = MakeShape1();
            _Vbos[1] = MakeShape2();
            _Vbos[2] = MakeRadiance("Radiance1");
            _Vbos[3] = MakeRadiance("Radiance2");

            // Uniforms
            new Uniform(_program, "Color1").SetValue(1.0f, 0.5f, 0.4f);
            new Uniform(_program, "Color2").SetValue(1.0f, 0.8f, 0.7f);

            // The gl_PointSize vertex shader output does not take screen scale into account.
            // So we scale it ourselves.
            new Uniform(_program, "ScreenScale").SetValue(Platform.ScreenScale);

            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");
            _uniStatus = new Uniform(_program, "Status");

            gl.ClearColor(0.2f, 0.2f, 0.2f, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
            gl.Enable(gl.Capability.Blend);
        }

        private DataBuffer MakeRadiance(string attrName)
        {
            Random random = new Random();
            float[] data = new float[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                data[i] = (float)(random.Next(100) * 0.01);
            }

            DataBuffer buffer = new DataBuffer(DataBuffer.Type.Vertex);
            buffer.Bind();
            buffer.Data(data);

            VertexAttribute attr = new VertexAttribute(_program, attrName);
            attr.SetConfig<float>();
            attr.Enable();

            return buffer;
        }

        private DataBuffer MakeShape1()
        {
            Random random = new Random();
            Vector3[] data = new Vector3[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                double phi = 2 * Math.PI * (random.Next(1000) * 0.001);
                double rho = 0.5 * Math.PI * ((random.Next(1000) * 0.002) - 1.0);

                float sPhi = (float)Math.Sin(phi);
                float cPhi = (float)Math.Cos(phi);
                float sRho = (float)Math.Sin(rho);
                float cRho = (float)Math.Cos(rho);

                data[i] = new Vector3(cPhi * cRho, sRho, sPhi * cRho);
            }

            DataBuffer buffer = new DataBuffer(DataBuffer.Type.Vertex);
            buffer.Bind();
            buffer.Data(data);

            VertexAttribute attr = new VertexAttribute(_program, "Position1");
            attr.SetConfig<Vector3>();
            attr.Enable();

            return buffer;
        }

        private DataBuffer MakeShape2()
        {
            Random random = new Random();
            Vector3[] data = new Vector3[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                double phi = 2 * Math.PI * (random.Next(1000) * 0.001);
                double rho = 2 * Math.PI * (random.Next(1000) * 0.001);

                float sPhi = (float)Math.Sin(phi);
                float cPhi = (float)Math.Cos(phi);
                float sRho = (float)Math.Sin(rho);
                float cRho = (float)Math.Cos(rho);

                data[i] = new Vector3(
                    cPhi * (0.5f + (0.5f * (1.0f + cRho))),
                    sRho * 0.5f,
                    sPhi * (0.5f + (0.5f * (1.0f + cRho))));
            }

            DataBuffer buffer = new DataBuffer(DataBuffer.Type.Vertex);
            buffer.Bind();
            buffer.Data(data);

            VertexAttribute attr = new VertexAttribute(_program, "Position2");
            attr.SetConfig<Vector3>();
            attr.Enable();

            return buffer;
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            if (((int)totalTimeSec & 3) == 0)
            {
                _status += deltaTimeSec;
            }
            else
            {
                float truncStatus = (float)Math.Truncate(_status);
                if (_status != truncStatus)
                {
                    float frac = _status - truncStatus;
                    if (frac < 0.5f)
                    {
                        _status = truncStatus;
                    }
                    else
                    {
                        _status = 1.0f + truncStatus;
                    }

                }
            }
            // Clear the color and depth buffer
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            // Use the program
            _program.Use();

            _uniStatus.SetValue(0.5f - (0.5f * (float)Math.Cos(Math.PI * _status)));

            // Set the matrix for camera orbiting the origin
            Matrix4 cameraMatrix = Utils.OrbitCameraMatrix(
                Vector3.Zero, 5.5f,
                (float)(totalTimeSec * Math.PI / 9.5),
                (float)MathHelper.DegreesToRadians(45 + Math.Sin(Math.PI * totalTimeSec / 7.5) * 40));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Render
            Matrix4 modelMatrix = Matrix4.CreateRotationX((float)(_status * Math.PI * 0.5));
            _uniModelMatrix.SetValue(ref modelMatrix);
            gl.DrawArrays(gl.PrimitiveType.Points, PointCount);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            base.Resize(newWidth, newHeight);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(48), (float)newWidth / newHeight, 1, 20);
            _program.Use();
            _uniProjectionMatrix.SetValue(ref projectionMatrix);
        }

        public override void Unload()
        {
            for (int i = 0; i < 3; i++)
            {
                _Vbos[i].Dispose();
            }
            _program.Dispose();
        }
    }
}
