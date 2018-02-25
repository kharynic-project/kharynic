using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace org.kharynic.Scripting
{
    public static class GeneratorUtils
    {
        public static string GetHeaderComment([CallerFilePath] string callerFilePath = "<unknown>")
        {
            var relativePath = callerFilePath.Replace("\\", "/").Replace(BuildInfo.LocalProjectPath, "");
            return $"// generated on {DateTime.Now:yyyy.MM.dd HH:mm} by {relativePath}";
        }

        public static void WriteFile(string code, string path, bool protectEditor = true)
        {
            // generated code cannot be used in editor-mode, or re-generation may fail
            if (protectEditor)
                code = "#if !UNITY_EDITOR\n\n" + code + "\n#endif\n";
            path = (UnityEngine.Application.dataPath + "/" + path).Replace("//", "/");
            var directoryPath = Path.GetDirectoryName(path);
            System.Diagnostics.Debug.Assert(directoryPath != null);
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Writing file {path}");
            File.WriteAllText(path, code);
        }

        public static string ReadFile(string path)
        {
            return File.ReadAllText(Path.Combine(UnityEngine.Application.dataPath, path));
        }
    }
}
