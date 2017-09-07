using Ooogles;
using OpenTK;
using System;
using System.Diagnostics;

namespace Examples.Shared
{
    public class TwistedTorusGeometry
    {
        private int _stripSize;
        private int _stripCount;

        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector3[] Tangents { get; private set; }

        public TwistedTorusGeometry(int sections = 12, int rings = 48, int twist = 12, float outerRadius = 1, float innerRadius = 0.5f, float thickness = 0.02f)
        {
            const double SSlipCoeff = 0.2;
            _stripSize = 2 * (rings + 1);
            _stripCount = sections * 4;

            int count = 2 * 2 * 2 * sections * (rings + 1);
            Positions = new Vector3[count];
            Normals = new Vector3[count];

            float t = thickness / innerRadius;
            float rTwist = (float)twist / rings;
            double rStep = 2.0 * Math.PI / rings;
            double sStep = 2.0 * Math.PI / sections;
            double sSlip = sStep * SSlipCoeff;
            float r1 = innerRadius;
            float r2 = outerRadius - innerRadius;

            int k = 0;
            float fSign = 1.0f;
            double[] sa = new double[2];

            for (int f = 0; f < 2; f++)
            {
                float fdt = t * fSign * 0.95f;

                for (int s = 0; s < sections; s++)
                {
                    double sAngle = (sStep * 0.5) + (s * sStep);
                    sa[0] = sAngle + (sSlip * fSign);
                    sa[1] = sAngle - (sSlip * fSign);

                    for (int r = 0; r <= rings; r++)
                    {
                        double rAngle = r * rStep;
                        float vz = (float)Math.Sin(rAngle);
                        float vx = (float)Math.Cos(rAngle);

                        double ta = sStep * r * rTwist;

                        for (int d = 0; d < 2; d++)
                        {
                            float vy = (float)Math.Sin(sa[d] + ta);
                            float vr = (float)Math.Cos(sa[d] + ta);

                            Positions[k] = new Vector3(
                                vx * (r1 + r2 * (1.0f + vr) + (fdt * vr)),
                                vy * (r2 + fdt),
                                vz * (r1 + r2 * (1.0f + vr) + (fdt * vr)));

                            Normals[k++] = new Vector3(
                                fSign * vx * vr,
                                fSign * vy,
                                fSign * vz * vr);
                        }
                    }
                }

                fSign = -1.0f;
            }

            float dSign = 1.0f;
            for (int d = 0; d < 2; d++)
            {
                for (int s = 0; s < sections; s++)
                {
                    double sAngle = (sStep * 0.5) + (s * sStep);
                    sa[0] = sAngle + (sSlip * dSign);

                    for (int r = 0; r <= rings; r++)
                    {
                        double rAngle = r * rStep;
                        double ta = sStep * r * rTwist;
                        float vy = (float)Math.Sin(sa[0] + ta);
                        float vr = (float)Math.Cos(sa[0] + ta);
                        float vz = (float)Math.Sin(rAngle);
                        float vx = (float)Math.Cos(rAngle);

                        fSign = 1.0f;
                        for (int f = 0; f < 2; f++)
                        {
                            float fdt = -t * dSign * fSign * 0.95f;

                            Positions[k] = new Vector3(
                                vx * (r1 + r2 * (1.0f + vr) + (fdt * vr)),
                                vy * (r2 + fdt),
                                vz * (r1 + r2 * (1.0f + vr) + (fdt * vr)));

                            Normals[k++] = new Vector3(
                                dSign * -vx * vr,
                                dSign * vy,
                                dSign * -vz * vr);

                            fSign = -1.0f;
                        }
                    }
                }
                dSign = -1.0f;
            }

            Debug.Assert(k == count);

            GenerateTangents(sections, rings, twist);
        }

        private void GenerateTangents(int sections, int rings, int twist)
        {
            int count = 2 * 2 * 2 * sections * (rings + 1);
            Tangents = new Vector3[count];
            int k = 0;

            for (int f = 0; f < 2; f++)
            {
                int fOff = f * sections * (rings + 1) * 2;
                for (int s = 0; s < sections; s++)
                {
                    int s0 = s * (rings + 1) * 2;
                    for (int r = 0; r <= rings; r++)
                    {
                        int s1 = s0;
                        int r0 = r;
                        int r1 = r + 1;
                        if (r == rings)
                        {
                            s1 = ((s + twist) % sections) * (rings + 1) * 2;
                            r1 = 1;
                        }

                        for (int d = 0; d < 2; d++)
                        {
                            int k0 = fOff + s0 + (r0 * 2) + d;
                            int k1 = fOff + s1 + (r1 * 2) + d;

                            float tx = Positions[k1].X - Positions[k0].X;
                            float ty = Positions[k1].Y - Positions[k0].Y;
                            float tz = Positions[k1].Z - Positions[k0].Z;
                            float tl = (float)Math.Sqrt((tx * tx) + (ty * ty) + (tz * tz));
                            Debug.Assert(tl > 0);

                            Tangents[k++] = new Vector3(tx / tl, ty / tl, tz / tl);
                        }
                    }
                }
            }

            for (int d = 0; d < 2; d++)
            {
                int dOff = d * sections * (rings + 1) * 2;
                for (int s = 0; s < sections; s++)
                {
                    int s0 = s * (rings + 1) * 2;
                    for (int r = 0; r <= rings; r++)
                    {
                        int s1 = s0;
                        int r0 = r;
                        int r1 = r + 1;
                        if (r == rings)
                        {
                            s1 = ((s + twist) % sections) * (rings + 1) * 2;
                            r1 = 1;
                        }

                        for (int f = 0; f < 2; f++)
                        {
                            int k0 = dOff + s0 + (r0 * 2) + f;
                            int k1 = dOff + s1 + (r1 * 2) + f;

                            float tx = Positions[k1].X - Positions[k0].X;
                            float ty = Positions[k1].Y - Positions[k0].Y;
                            float tz = Positions[k1].Z - Positions[k0].Z;
                            float tl = (float)Math.Sqrt((tx * tx) + (ty * ty) + (tz * tz));
                            Debug.Assert(tl > 0);

                            Tangents[k++] = new Vector3(tx / tl, ty / tl, tz / tl);
                        }
                    }
                }
            }

            Debug.Assert(k == count);
        }

        public void Clear()
        {
            Positions = null;
            Normals = null;
            Tangents = null;
            // We keep Indices, since they may be used with Draw*
        }

        public void Draw()
        {
            int first = 0;
            int count = _stripSize;
            for (int i = 0; i < _stripCount; i++)
            {
                gl.DrawArrays(gl.PrimitiveType.TriangleStrip, first, count);
                first += count;
            }
        }
    }
}
