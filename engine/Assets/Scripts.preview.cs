#if UNITY_EDITOR

using System;

namespace org.kharynic
{
    // generated on 2018.02.18 23:35 by org.kharynic.Editor.Scripts+Generator.GenerateEditorPreviewStubs
    // this file is used by editor preview which cannot regenerate it, so it has to be commited
    public static class Scripts
    {
        public const bool Enabled = false;

        public static void OnLoad() { Debug.Log("Scripts.OnLoad: unsupported platform"); }

        public static void Execute(string code) { Debug.Log("Scripts.Execute: unsupported platform"); }

        public static void Log(string message) { Debug.Log("Scripts.Log: unsupported platform"); }

        public static void RegisterExternal(string name, IntPtr functionPtr) { Debug.Log("Scripts.RegisterExternal: unsupported platform"); }

        public static string GetRootUrl() { Debug.Log("Scripts.GetRootUrl: unsupported platform"); return default(string); }
    }
}

#endif
