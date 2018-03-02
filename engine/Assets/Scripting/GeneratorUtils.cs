using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Kharynic.Engine.Scripting
{
    public static class GeneratorUtils
    {
        public static string GetHeaderComment([CallerFilePath] string callerFilePath = "<unknown>")
        {
            var relativePath = callerFilePath.Replace("\\", "/").Replace(BuildInfo.LocalProjectPath, "");
            return $"// Code generated on {DateTime.Now:yyyy.MM.dd} by {relativePath} - DO NOT EDIT.";
        }

        // TODO: assert that path starts with / (is project-absolute)
        public static void WriteFile(string code, string path, bool protectEditor = true)
        {
            if (!path.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase))
                protectEditor = false;
            // generated code cannot be used in editor-mode, or re-generation may fail
            if (protectEditor)
                code = "#if !UNITY_EDITOR\n\n" + code + "\n#endif\n";
            path = (BuildInfo.LocalProjectPath + "/" + path).Replace("//", "/");
            var directoryPath = Path.GetDirectoryName(path);
            System.Diagnostics.Debug.Assert(directoryPath != null);
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Writing file {path}");
            File.WriteAllText(path, code);
        }

        public static string ReadFile(string path)
        {
            path = (BuildInfo.LocalProjectPath + "/" + path).Replace("//", "/");
            return File.ReadAllText(Path.Combine(UnityEngine.Application.dataPath, path));
        }

        public static string GetSourceFilePath(Type type, string extension, bool isUnityAsset = false, bool isGenerated = true)
        {
            var qualifiedName = $"{type.Namespace}.{type.Name}";
            var relativeName = new Regex($"^{BuildInfo.RootNamespace}").Replace(qualifiedName, "");
            var path = $"/{relativeName.Replace(".", "/")}.{(isGenerated ? "generated." : "")}{extension}";
            if (isUnityAsset)
                path = new Regex($"^/Engine/").Replace(path, "/Engine/Assets/");
            return path;
        }
    }
}
