using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace org.kharynic
{
    public static class Debug
    {
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
                var infoStartColumn = logWidth - sourceInfo.Length;
                message = message.PadRight(infoStartColumn) + sourceInfo;
            }
            #if UNITY_EDITOR
                UnityEngine.Debug.Log(message);
            #else
                Scripts.Log(message);
            #endif
        }
    }
}
