using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LightMock.Generator
{
    internal static class Utils
    {
        public static string LoadResource(string resourceName, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            resourceName = assembly.GetManifestResourceNames().First(n => n.EndsWith("." + resourceName));
            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException("resource not found");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
