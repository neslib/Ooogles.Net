using System;
using Ooogles;
using OpenTK;
using Examples.Shared;

// Based on 013_striped_cubes.cpp example from oglplus (http://oglplus.org/)

namespace E12StripedCubes
{
    public class App : Examples.Shared.Application
    {
        private Program _program;
        private DataBuffer _verts;
        private DataBuffer _texCoords;
        private DataBuffer _indices;
        private CubeGeometry _cube;
        private Uniform _uniProjectionMatrix;
        private Uniform _uniCameraMatrix;
        private Uniform _uniModelMatrix;

        public override void Load()
        {
            Shader vertexShader = new Shader(Shader.Type.Vertex, @"
                uniform mat4 ProjectionMatrix, CameraMatrix, ModelMatrix;

                attribute vec3 Position;
                attribute vec2 TexCoord;

                varying vec2 vertTexCoord;

                void main(void)
                {
                  vertTexCoord = TexCoord;
                  gl_Position = 
                    ProjectionMatrix *
                    CameraMatrix *
                    ModelMatrix *
                    vec4(Position, 1.0);
                }");
            vertexShader.Compile();

            Shader fragmentShader = new Shader(Shader.Type.Fragment, @"
                precision mediump float;

                varying vec2 vertTexCoord;

                void main(void)
                {
                  float i = floor(mod((vertTexCoord.x + vertTexCoord.y) * 8.0, 2.0));
                  gl_FragColor = mix(
                    vec4(0, 0, 0, 1),
                    vec4(1, 1, 0, 1),
                    i
                  );
                }");
            fragmentShader.Compile();

            _program = new Program(vertexShader, fragmentShader);
            _program.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();

            _program.Use();

            _cube = new CubeGeometry(0.5f);

            // Positions
            _verts = new DataBuffer(DataBuffer.Type.Vertex);
            _verts.Bind();
            _verts.Data(_cube.Positions);

            VertexAttribute attr = new VertexAttribute(_program, "Position");
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

            // Uniforms
            _uniProjectionMatrix = new Uniform(_program, "ProjectionMatrix");
            _uniCameraMatrix = new Uniform(_program, "CameraMatrix");
            _uniModelMatrix = new Uniform(_program, "ModelMatrix");

            gl.ClearColor(0.8f, 0.8f, 0.7f, 0);
            gl.ClearDepth(1);
            gl.Enable(gl.Capability.DepthTest);
        }

        public override void Render(float deltaTimeSec, double totalTimeSec)
        {
            // Clear the color and depth buffer
            gl.Clear(gl.ClearBuffers.Color | gl.ClearBuffers.Depth);

            // Use the program
            _program.Use();

            // Orbit camera around cubes
            Matrix4 cameraMatrix = Utils.OrbitCameraMatrix(
                Vector3.Zero, 3.5f, 
                (float)MathHelper.DegreesToRadians(totalTimeSec * 15), 
                (float)MathHelper.DegreesToRadians(Math.Sin(totalTimeSec) * 45));
            _uniCameraMatrix.SetValue(ref cameraMatrix);

            // Update and render first cube
            Matrix4 translation = Matrix4.CreateTranslation(-1, 0, 0);
            Matrix4 rotation = Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(totalTimeSec * 180));
            Matrix4 modelMatrix = rotation * translation;
            _uniModelMatrix.SetValue(ref modelMatrix);
            _cube.DrawWithBoundIndexBuffer();

            // Update and render second cube
            translation = Matrix4.CreateTranslation(1, 0, 0);
            rotation = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(totalTimeSec * 90));
            modelMatrix = rotation * translation;
            _uniModelMatrix.SetValue(ref modelMatrix);
            _cube.DrawWithBoundIndexBuffer();
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
            _indices.Dispose();
            _texCoords.Dispose();
            _verts.Dispose();
            _program.Dispose();
        }
    }
}
