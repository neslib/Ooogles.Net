using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.ES20;

namespace Ooogles
{
#pragma warning disable IDE1006 // Naming Styles
    internal static class glUtils
#pragma warning restore IDE1006 // Naming Styles
    {
        [Conditional("DEBUG")]
        internal static void Check(object instance, [CallerMemberName] string memberName = null)
        {
            Debug.Assert(instance != null);
#if __ANDROID__ || __IOS__
            ErrorCode error = GL.GetErrorCode();            
#else
            ErrorCode error = GL.GetError();
#endif
            if (error != ErrorCode.NoError)
                throw new GLException((gl.Error)error, instance.ToString() + "." + memberName);
        }

        [Conditional("DEBUG")]
        internal static void CheckBinding(All target, object instance, [CallerMemberName] string memberName = null)
        {
            Debug.Assert(instance != null);
            GL.GetInteger(GetTargetBinding(target), out int currentBinding);
            if (currentBinding == 0)
                Debug.WriteLine($"The method {instance.ToString()}.{memberName} requires an object bound to {GetTargetName(target)}, but no object is bound to that target");
        }

        internal static void CheckBinding(All target, int expectedBinding, object instance, [CallerMemberName] string memberName = null)
        {
            Debug.Assert(instance != null);
            GL.GetInteger(GetTargetBinding(target), out int currentBinding);
            if (currentBinding != expectedBinding)
                Debug.WriteLine($"The method {instance.ToString()}.{memberName} requires the current object bound to {GetTargetName(target)}, but another object is bound to that target");
        }

        internal static GetPName GetTargetBinding(All target)
        {
            switch (target)
            {
                case All.ArrayBuffer:
                    return GetPName.ArrayBufferBinding;

                case All.ElementArrayBuffer:
                    return GetPName.ElementArrayBufferBinding;

                case All.Texture2D:
                    return GetPName.TextureBinding2D;

                case All.TextureCubeMap:
                    return GetPName.TextureBindingCubeMap;

                case All.Renderbuffer:
                    return GetPName.RenderbufferBinding;

                case All.Framebuffer:
                    return GetPName.FramebufferBinding;

                default:
                    Debug.Assert(false);
                    return (GetPName)0;
            }
        }

        private static string GetTargetName(All target)
        {
            switch (target)
            {
                case All.ArrayBuffer:
                    return "Buffer.Type.Vertex";

                case All.ElementArrayBuffer:
                    return "Buffer.Type.Index";

                case All.Texture2D:
                    return "Texture.Type.TwoD";

                case All.TextureCubeMap:
                    return "Texture.Type.CubeMap";

                case All.Renderbuffer:
                    return "Renderbuffer";

                case All.Framebuffer:
                    return "Framebuffer";

                default:
                    Debug.Assert(false);
                    return $"(unknown {target})";
            }
        }
    }
}
