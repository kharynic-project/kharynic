using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace org.kharynic.Editor
{
    public class GeneratorUtils
    {
        public static string GetHeaderComment([CallerMemberName] string callerMemberName = null)
        {
            var caller = new StackTrace(skipFrames: 1).GetFrame(0).GetMethod();
            return $"// generated on {DateTime.Now:yyyy.MM.dd HH:mm} " +
                   $"by {caller?.DeclaringType?.FullName}.{caller?.Name}";
        }

        public static void WriteFile(string code, string path)
        {
            // editor has to use stubs from repository instead or build won't start
            code = "#if !UNITY_EDITOR\n\n" + code + "\n#endif\n";
            File.WriteAllText(Application.dataPath + "/" + path, code);
            AssetDatabase.Refresh();
        }
    }
}