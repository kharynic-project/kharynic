using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace org.kharynic
{
    public static class Debug
    {
        public static string LogFile = UnityEngine.Application.dataPath + "/debug.log";
        private static readonly char[] PathSeparators = {'/','\\'};
        
        public static void Log(
            string message,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int    callerLineNumber = -1)
        {
            if (callerLineNumber >= 0 && callerFilePath?.Length > 0)
            {
                // Path.GetFileName doesn't work properly on WebAssembly build (Win specific?)
                var fileNameStartIndex = callerFilePath.LastIndexOfAny(PathSeparators) + 1;
                var fileName = callerFilePath.Substring(fileNameStartIndex);
                var sourceInfo = $" <{fileName},#{callerLineNumber}>";
                const int logWidth = 80;
                const string timeFormat = "ss.fff ";
                var infoStartColumn = logWidth - sourceInfo.Length;
                message = DateTime.Now.ToString(timeFormat) + message;
                var lines = message.Split('\n');
                lines[lines.Length - 1] = lines[lines.Length - 1].PadRight(infoStartColumn);
                message = string.Join("\n", lines) + sourceInfo;
            }
            #if UNITY_EDITOR
                UnityEngine.Debug.Log(message);
                File.AppendAllText(LogFile, $"{message}\n");
            #else
                Scripts.Log(message);
            #endif
        }

        public static string GetFullObjectName(UnityEngine.Object @object)
        {
            if (@object.GetType() != typeof(UnityEngine.Object))
                return @object.GetType().FullName;
            var stringRep = @object.ToString();
            var typeNameStart = stringRep.IndexOf('(');
            var typeNameEnd = stringRep.IndexOf(')');
            if (typeNameStart > 0 && typeNameEnd > 0)
                return stringRep.Substring(typeNameStart + 1, typeNameEnd - typeNameStart - 1);
            return @object.name;
        }

        public static IEnumerable<UnityEngine.Object> GetEngineObjects()
        {
            return UnityEngine.Object.FindObjectsOfType(typeof(UnityEngine.Object))
                .Where(o => !(o is UnityEngine.GameObject) && !(o is UnityEngine.Component))
                .Where(o => GetFullObjectName(o)?.StartsWith($"{typeof(UnityEngine.Object).Namespace}.") == true);
        }
        
        public static void LogEngineObjects()
        {
            var engineObjects = GetEngineObjects().ToArray();
            Log($"{engineObjects.Length} engine objects found: \n{string.Join("\n", engineObjects.Select(GetFullObjectName))}");
        }
    }
}
