using System;
using OpenTK;
using Ooogles;

namespace Examples.Shared
{
    public class SphereGeometry
    {
        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] TexCoords { get; private set; }
        public ushort[] Indices { get; }

        public SphereGeometry(int sliceCount = 18, float radius = 1)
        {
            int parallelCount = sliceCount / 2;
            int vertexCount = (parallelCount + 1) * (sliceCount + 1);
            int indexCount = parallelCount * sliceCount * 6;
            float angleStep = (float)(2 * Math.PI) / sliceCount;
            float uStep = 1.0f / parallelCount;
            float vStep = 1.0f / sliceCount;

            Positions = new Vector3[vertexCount];
            Normals = new Vector3[vertexCount];
            TexCoords = new Vector2[vertexCount];

            for (int i = 0; i <= parallelCount; i++)
            {
                float si = (float)Math.Sin(angleStep * i);
                float ci = (float)Math.Cos(angleStep * i);
                float uLat = 1.0f - (i * uStep);
                for (int j = 0; j <= sliceCount; j++)
                {
                    float sj = (float)Math.Sin(angleStep * j);
                    float cj = (float)Math.Cos(angleStep * j);

                    Vector3 direction = new Vector3(si * sj, ci, si * cj);
                    int vertexIndex = i * (sliceCount + 1) + j;

                    Positions[vertexIndex] = direction * radius;
                    Normals[vertexIndex] = direction;
                    TexCoords[vertexIndex] = new Vector2(j * vStep, uLat);
                }
            }

            Indices = new ushort[indexCount];
            int k = 0;
            for (int i = 0; i < parallelCount; i++)
            {
                for (int j = 0; j < sliceCount; j++)
                {
                    Indices[k++] = (ushort)((i + 0) * (sliceCount + 1) + (j + 0));
                    Indices[k++] = (ushort)((i + 1) * (sliceCount + 1) + (j + 0));
                    Indices[k++] = (ushort)((i + 1) * (sliceCount + 1) + (j + 1));
                    Indices[k++] = (ushort)((i + 0) * (sliceCount + 1) + (j + 0));
                    Indices[k++] = (ushort)((i + 1) * (sliceCount + 1) + (j + 1));
                    Indices[k++] = (ushort)((i + 0) * (sliceCount + 1) + (j + 1));
                }
            }
        }

        public void Clear()
        {
            Positions = null;
            Normals = null;
            TexCoords = null;
            // We keep Indices, since they may be used with Draw*
        }

        public void DrawWithBoundIndexBuffer()
        {
            gl.DrawElements(gl.PrimitiveType.Triangles, Indices.Length, gl.IndexType.UnsingedShort);
        }

        public void DrawWithIndices()
        {
            gl.DrawElements(gl.PrimitiveType.Triangles, Indices);
        }
    }
}
