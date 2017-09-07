using OpenTK;
using System;

namespace Examples.Shared
{
    public static class Utils
    {
        public static Matrix4 OrbitCameraMatrix(Vector3 target, float radius, float azimuth, float elevation)
        {
            float sa = (float)Math.Sin(azimuth);
            float ca = (float)Math.Cos(azimuth);
            float se = (float)Math.Sin(elevation);
            float ce = (float)Math.Cos(elevation);

            Vector3 z = new Vector3(ce * ca, se, ce * -sa);
            Vector3 x = new Vector3(-sa, 0, -ca);
            Vector3 y = Vector3.Cross(z, x);

            return new Matrix4(
                x.X, y.X, z.X, 0,
                x.Y, y.Y, z.Y, 0,
                x.Z, y.Z, z.Z, 0,
                Vector3.Dot(x, z) * -radius - Vector3.Dot(x, target),
                Vector3.Dot(y, z) * -radius - Vector3.Dot(y, target),
                Vector3.Dot(z, z) * -radius - Vector3.Dot(z, target),
                1);
        }
    }
}
