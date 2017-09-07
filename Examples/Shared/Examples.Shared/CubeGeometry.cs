using System;
using OpenTK;
using Ooogles;

namespace Examples.Shared
{
    public class CubeGeometry
    {
        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector3[] Tangents { get; private set; }
        public Vector2[] TexCoords { get; private set; }
        public ushort[] Indices { get; }

        public CubeGeometry(float radius = 0.5f)
        {
            Positions = new Vector3[] {
                new Vector3(-radius, -radius, -radius),
                new Vector3(-radius, -radius,  radius),
                new Vector3( radius, -radius,  radius),
                new Vector3( radius, -radius, -radius),
                new Vector3(-radius,  radius, -radius),
                new Vector3(-radius,  radius,  radius),
                new Vector3( radius,  radius,  radius),
                new Vector3( radius,  radius, -radius),
                new Vector3(-radius, -radius, -radius),
                new Vector3(-radius,  radius, -radius),
                new Vector3( radius,  radius, -radius),
                new Vector3( radius, -radius, -radius),
                new Vector3(-radius, -radius,  radius),
                new Vector3(-radius,  radius,  radius),
                new Vector3( radius,  radius,  radius),
                new Vector3( radius, -radius,  radius),
                new Vector3(-radius, -radius, -radius),
                new Vector3(-radius, -radius,  radius),
                new Vector3(-radius,  radius,  radius),
                new Vector3(-radius,  radius, -radius),
                new Vector3( radius, -radius, -radius),
                new Vector3( radius, -radius,  radius),
                new Vector3( radius,  radius,  radius),
                new Vector3( radius,  radius, -radius)
            };

            Normals = new Vector3[]
            {
                new Vector3( 0, -1,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  0, -1),
                new Vector3( 0,  0, -1),
                new Vector3( 0,  0, -1),
                new Vector3( 0,  0, -1),
                new Vector3( 0,  0,  1),
                new Vector3( 0,  0,  1),
                new Vector3( 0,  0,  1),
                new Vector3( 0,  0,  1),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0)
            };

            Tangents = new Vector3[]
            {
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3(-1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 1,  0,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0, -1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  1,  0),
                new Vector3( 0,  1,  0)
            };

            TexCoords = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };

            Indices = new ushort[] {
                 0,  2,  1,
                 0,  3,  2,
                 4,  5,  6,
                 4,  6,  7,
                 8,  9, 10,
                 8, 10, 11,
                12, 15, 14,
                12, 14, 13,
                16, 17, 18,
                16, 18, 19,
                20, 23, 22,
                20, 22, 21
            };
        }

        public void Clear()
        {
            Positions = null;
            Normals = null;
            Tangents = null;
            TexCoords = null;
            // We keep Indices, since they may be used with Draw*
        }

        public void DrawWithBoundIndexBuffer()
        {
            gl.DrawElements(gl.PrimitiveType.Triangles, Indices.Length, gl.IndexType.UnsingedShort);
        }

        public void DrawWidthIndices()
        {
            gl.DrawElements(gl.PrimitiveType.Triangles, Indices);
        }
    }
}
