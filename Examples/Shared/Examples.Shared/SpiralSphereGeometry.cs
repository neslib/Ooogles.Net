using Ooogles;
using OpenTK;
using System;
using System.Diagnostics;

namespace Examples.Shared
{
    public class SpiralSphereGeometry
    {
        private int _stripSize;
        private int _stripCount;

        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
        public ushort[] Indices { get; private set; }

        public SpiralSphereGeometry(int bands = 4, int divisions = 8, int segments = 48, float radius = 1.0f, float thickness = 0.1f)
        {
            _stripSize = (segments + 1) * 2;
            _stripCount = (2 * bands * divisions) + (4 * bands);
            int vertexCount = ((bands * 2) * (divisions + 1) * (segments + 1)) + ((bands * 2) * (segments + 1));

            Positions = new Vector3[vertexCount];
            Normals = new Vector3[vertexCount];

            int k = 0;
            MakeVectors(Positions, ref k, 1.0f, radius, bands, divisions, segments);
            MakeVectors(Positions, ref k, 1.0f, radius + thickness, bands, divisions, segments);
            MakeSideVerts(Positions, ref k, bands, segments, radius, thickness);
            Debug.Assert(k == vertexCount);

            k = 0;
            MakeVectors(Normals, ref k, -1.0f, 1.0f, bands, divisions, segments);
            MakeVectors(Normals, ref k,  1.0f, 1.0f, bands, divisions, segments);
            MakeSideVerts(Normals, ref k, bands, segments, radius, thickness);
            Debug.Assert(k == vertexCount);

            GenerateIndices(bands, divisions, segments);
        }

        public void Clear()
        {
            Positions = null;
            Normals = null;
            // We keep Indices, since they may be used with Draw
        }

        public void Draw()
        {
            int first = 0;
            int count = _stripSize;
            for (int i = 0; i < _stripCount; i++)
            {
                gl.DrawElements(gl.PrimitiveType.TriangleStrip, Indices, first, count);
                first += count;
            }
        }

        private void GenerateIndices(int bands, int divisions, int segments)
        {
            int m = ((bands * 2) * (divisions * 2) * (segments + 1)) + ((bands * 8) * (segments + 1));
            Indices = new ushort[m];

            int k = 0;
            int offs = 0;
            int edge = segments + 1;
            int band = edge * (divisions + 1);
            int surface = bands * band;

            int edge1 = 0;
            int edge2 = edge;
            for (int n = 0; n < 2; n++)
            {
                for (int b = 0; b < bands; b++)
                {
                    for (int d = 0; d < divisions; d++)
                    {
                        for (int s = 0; s < edge; s++)
                        {
                            Indices[k++] = (ushort)(offs + s + edge1);
                            Indices[k++] = (ushort)(offs + s + edge2);
                        }
                        offs += edge;
                    }
                    offs += edge;
                }
                edge1 = edge;
                edge2 = 0;
            }

            offs = 0;
            int eOffs = 2 * surface;

            for (int b = 0; b < bands; b++)
            {
                for (int s = 0; s < edge; s++)
                {
                    Indices[k++] = (ushort)(offs + s);
                    Indices[k++] = (ushort)(eOffs + s);
                }
                offs += band;
                eOffs += (edge * 2);
            }

            offs = divisions * edge;
            eOffs = (2 * surface) + edge;

            for (int b = 0; b < bands; b++)
            {
                for (int s = 0; s < edge; s++)
                {
                    Indices[k++] = (ushort)(offs + s);
                    Indices[k++] = (ushort)(eOffs + s);
                }
                offs += band;
                eOffs += (edge * 2);
            }

            offs = surface;
            eOffs = 2 * surface;

            for (int b = 0; b < bands; b++)
            {
                for (int s = 0; s < edge; s++)
                {
                    Indices[k++] = (ushort)(offs + s);
                    Indices[k++] = (ushort)(eOffs + s);
                }
                offs += band;
                eOffs += (edge * 2);
            }

            offs = surface + (divisions * edge);
            eOffs = (2 * surface) + edge;

            for (int b = 0; b < bands; b++)
            {
                for (int s = 0; s < edge; s++)
                {
                    Indices[k++] = (ushort)(eOffs + s);
                    Indices[k++] = (ushort)(offs + s);
                }
                offs += band;
                eOffs += (edge * 2);
            }

            Debug.Assert(k == m);
        }

        private void MakeSideVerts(Vector3[] dst, ref int k, int bands, int segments, float radius, float thickness)
        {
            double bLeap = Math.PI / bands;
            double bSlip = bLeap * thickness * 0.5;
            double sStep = Math.PI / segments;

            float m = radius + (thickness * 0.5f);
            double g = -1;

            for (int b = 0; b < (bands * 2); b++)
            {
                double bOffs = 0;
                for (int s = 0; s <= segments; s++)
                {
                    double bAngle = (b * bLeap) + bOffs + (g * bSlip);
                    float sb = (float)Math.Sin(bAngle);
                    float cb = (float)Math.Cos(bAngle);

                    double sAngle = s * sStep;
                    float ss = (float)Math.Sin(sAngle);
                    float cs = (float)Math.Cos(sAngle);

                    dst[k++] = new Vector3(m * ss * cb, m * cs, m * ss * -sb);

                    bOffs += ss * sStep;
                }

                g *= -1;
            }
        }

        private void MakeVectors(Vector3[] dst, ref int k, float sign, float radius, int bands, int divisions, int segments)
        {
            double bLeap = Math.PI / bands;
            double bStep = bLeap / divisions;
            double sStep = Math.PI / segments;

            float m = sign * radius;

            for (int b = 0; b < bands; b++)
            {
                for (int d = 0; d <= divisions; d++)
                {
                    double bOffs = 0;
                    for (int s = 0; s <= segments; s++)
                    {
                        double bAngle = (2 * b * bLeap) + (d * bStep) + bOffs;
                        float sb = (float)Math.Sin(bAngle);
                        float cb = (float)Math.Cos(bAngle);

                        double sAngle = s * sStep;
                        float ss = (float)Math.Sin(sAngle);
                        float cs = (float)Math.Cos(sAngle);

                        dst[k++] = new Vector3(m * ss * cb, m * cs, m * ss * -sb);

                        bOffs += ss * sStep;
                    }
                }
            }
        }
    }
}
