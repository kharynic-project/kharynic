using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace org.kharynic.Editor
{
    public static class GeneratorUtils
    {
        public static string GetHeaderComment()
        {
            var caller = new StackTrace(skipFrames: 1).GetFrame(0).GetMethod();
            return $"// generated on {DateTime.Now:yyyy.MM.dd HH:mm} " +
                   $"by {caller?.DeclaringType?.FullName}.{caller?.Name}";
        }

        public static void WriteFile(string code, string path, bool protectEditor = true)
        {
            // editor has to use stubs from repository instead or build won't start
            if (protectEditor)
                code = "#if !UNITY_EDITOR\n\n" + code + "\n#endif\n";
            File.WriteAllText(UnityEngine.Application.dataPath + "/" + path, code);
            UnityEditor.AssetDatabase.Refresh();
        }

        public static string ReadFile(string path)
        {
            return File.ReadAllText(Path.Combine(Application.dataPath, path));
        }
    }
}