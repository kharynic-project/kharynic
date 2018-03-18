using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Kharynic.Engine.Scripting
{
    // This is the only class allowed to work on filesystem-absolute paths.
    // All the others have to use this one in order to prevent accidental access
    // to files outside the project during build process.
    public static class GeneratorUtils
    {
        public static string GetHeaderComment([CallerFilePath] string callerFilePath = "<unknown>")
        {
            var relativePath = callerFilePath.Replace("\\", "/").Replace(BuildInfo.LocalProjectPath, "");
            return $"// Code generated on {DateTime.Now:yyyy.MM.dd} by {relativePath} - DO NOT EDIT.";
        }

        public static string ToAbsolutePath(string projectRelativePath)
        {
            var path = BuildInfo.LocalProjectPath + "/" + projectRelativePath;
            path = Regex.Replace(path, "[\\/]+", "/");
            return path;
        }

        public static void WriteFile(string code, string path, bool protectEditor = true)
        {
            if (!path.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase))
                protectEditor = false;
            // generated code cannot be used in editor-mode, or re-generation may fail
            if (protectEditor)
                code = "#if !UNITY_EDITOR\n\n" + code + "\n#endif\n";
            path = ToAbsolutePath(path);
            var directoryPath = Path.GetDirectoryName(path);
            System.Diagnostics.Debug.Assert(directoryPath != null);
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Writing file {path}");
            File.WriteAllText(path, code);
        }

        public static string ReadFile(string path)
        {
            path = ToAbsolutePath(path);
            return File.ReadAllText(Path.Combine(UnityEngine.Application.dataPath, path));
        }

        // returns path relative to project root (suitabe for WriteFile and ReadFile)
        public static string GetSourceFilePath(Type type, string extension, bool isGenerated = true)
        {
            var qualifiedName = $"{type.Namespace}.{type.Name}";
            var relativeName = new Regex($"^{BuildInfo.RootNamespace}\\.").Replace(qualifiedName, "");
            var path = $"/{relativeName.Replace(".", "/")}.{(isGenerated ? "generated." : "")}{extension}";
            if (extension.EndsWith(".cs"))
            {
                if (path.StartsWith("/Engine/"))
                    path = new Regex($"^/Engine/").Replace(path, BuildInfo.RelativeEngineAssetsPath + "/");
                else
                    path = BuildInfo.RelativeEngineAssetsPath + path;
            }
            if (extension.EndsWith(".jslib"))
            {
                if (path.StartsWith("/Engine/"))
                    path = new Regex($"^/Engine/").Replace(path, BuildInfo.RelativeEngineAssetsPath + "/Plugins/");
                else
                    path = BuildInfo.RelativeEngineAssetsPath + "/Plugins" + path;
            }
            return path;
        }
    }
}
