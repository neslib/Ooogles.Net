using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Examples.Shared
{
    public static class Assets
    {
        private static Assembly _assembly = null;
        private static string _prefix;

        public static void Initialize(Type typeInAssetsAssembly)
        {
            Debug.Assert(_assembly == null);
            if (_assembly == null)
                _assembly = Assembly.GetAssembly(typeInAssetsAssembly);
            Debug.Assert(_assembly != null);
            _prefix = _assembly.GetName().Name + ".";
        }

        public static Stream Open(string filename)
        {
            Debug.Assert(_assembly != null, "Did you forget to call Initialize()?");
            var names = _assembly.GetManifestResourceNames();
            var modules = _assembly.GetLoadedModules();
            return _assembly.GetManifestResourceStream(_prefix + filename);
        }

        public static byte[] Load(string filename)
        {
            Stream stream = Open(filename);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
