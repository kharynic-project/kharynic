using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace org.kharynic
{
    public static class Debug
    {
        private static readonly string LogFile = UnityEngine.Application.dataPath + "/debug.log";
        
        private static readonly char[] PathSeparators = {'/','\\'};
        
        public static void Log(
            string message,
            [CallerFilePath] string callerFilePath = "unknown.cs",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            // Path.GetFileName works only with same platforms path (with Windows/CallerLineNumber+WASM not)
            var fileNameStartIndex = callerFilePath.LastIndexOfAny(PathSeparators) + 1;
            var fileName = callerFilePath.Substring(fileNameStartIndex);
            var timestamp = (Engine.Instance.RunningTime.TotalSeconds * 100) % 1000;
            var annotation = $"    <{fileName},#{callerLineNumber}> {timestamp:000}";
            const int logWidth = 80;
            var lines = message.Split('\n').Select((s, i) => (i == 0) ? s : ("  " + s)).ToArray();
            var annotationStartColumn = Math.Max(logWidth - annotation.Length, lines.Select(l=>l.Length).Max());
            lines[0] = lines[0].PadRight(annotationStartColumn) + annotation;
            message = string.Join("\n", lines);
            
            #if UNITY_EDITOR
                UnityEngine.Debug.Log(message);
                File.AppendAllText(LogFile, $"{message}\n");
            #else
                Scripts.Log(message);
            #endif
        }


        public static void LogEngineObjects()
        {
            Func<UnityEngine.Object, string> getFullObjectName = (@object) =>
            {
                if (@object.GetType() != typeof(UnityEngine.Object))
                    return @object.GetType().FullName;
                var stringRep = @object.ToString();
                var typeNameStart = stringRep.IndexOf('(');
                var typeNameEnd = stringRep.IndexOf(')');
                if (typeNameStart > 0 && typeNameEnd > 0)
                    return stringRep.Substring(typeNameStart + 1, typeNameEnd - typeNameStart - 1);
                return @object.name;
            };
            
            var engineObjects = UnityEngine.Object
                .FindObjectsOfType(typeof(UnityEngine.Object))
                .Where(o => !(o is UnityEngine.GameObject) && !(o is UnityEngine.Component))
                .Select(o => getFullObjectName(o))
                .ToArray();
            Log($"{engineObjects.Length} engine objects found: \n{string.Join("\n", engineObjects)}");
        }

        [AssertionMethod]
        [Conditional("DEBUG")]
        public static void Assert(
            bool condition, 
            string message = null,
            [CallerFilePath] string callerFilePath = "unknown.cs",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!condition)
            {
                message = $"ERROR: assertion failed: {message}";
                Log(message, callerFilePath, callerLineNumber);
                #if UNITY_EDITOR
                    UnityEngine.Debug.Break();
                #endif
            }
        }
    }
}
