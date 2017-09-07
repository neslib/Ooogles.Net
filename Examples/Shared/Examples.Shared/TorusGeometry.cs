using System;
using System.Diagnostics;
using OpenTK;
using Ooogles;

namespace Examples.Shared
{
    public class TorusGeometry
    {
        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] TexCoords { get; private set; }
        public ushort[] Indices { get; }

        public TorusGeometry(int sections = 16, int rings = 24, float outerRadius = 1, float innerRadius = 0.5f)
        {
            int count = (rings + 1) * (sections + 1);
            Positions = new Vector3[count];
            Normals = new Vector3[count];
            TexCoords = new Vector2[count];

            int k = 0;
            double rStep = (2 * Math.PI) / rings;
            double sStep = (2 * Math.PI) / sections;
            float r1 = innerRadius;
            float r2 = outerRadius - innerRadius;

            for (int r = 0; r <= rings; r++)
            {
                float vz = (float)-Math.Sin(r * rStep);
                float vx = (float) Math.Cos(r * rStep);

                for (int s = 0; s <= sections; s++)
                {
                    float vy = (float)Math.Sin(s * sStep);
                    float vr = (float)Math.Cos(s * sStep);

                    Positions[k] = new Vector3(
                        vx * (r1 + r2 * (1 + vr)),
                        vy * r2,
                        vz * (r1 + r2 * (1 + vr)));

                    Normals[k++] = new Vector3(vx * vr, vy, vz * vr);
                }
            }
            Debug.Assert(k == count);

            k = 0;
            rStep = 1.0 / rings;
            sStep = 1.0 / sections;

            for (int r = 0; r <= rings; r++)
            {
                float u = (float)(r * rStep);

                for (int s = 0; s <= sections; s++)
                {
                    float v = (float)(s * sStep);
                    TexCoords[k++] = new Vector2(u, v);
                }
            }
            Debug.Assert(k == count);

            count = rings * (2 * (sections + 1) + 2);
            Indices = new ushort[count];
            k = 0;
            int offs = 0;

            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s <= sections; s++)
                {
                    Indices[k++] = (ushort)(offs + s);
                    Indices[k++] = (ushort)(offs + s + (sections + 1));
                }

                offs += sections + 1;
                Indices[k++] = (ushort)(offs + sections);
                Indices[k++] = (ushort)(offs);
            }
            Debug.Assert(k == count);
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
            gl.DrawElements(gl.PrimitiveType.TriangleStrip, Indices.Length, gl.IndexType.UnsingedShort);
        }

        public void DrawWithIndices()
        {
            gl.DrawElements(gl.PrimitiveType.TriangleStrip, Indices);
        }
    }
}
