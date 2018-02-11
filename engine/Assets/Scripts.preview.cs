#if UNITY_EDITOR

using System;

namespace org.kharynic
{
    public static class Scripts
    {
        public const bool Enabled = false;
        
        public static void OnLoad() { Debug.Log("Scripts.OnLoad: unsupported platform"); }
        
        public static void Execute(string code) { Debug.Log("Scripts.Execute: unsupported platform"); }
        
        public static void Log(string message) { Debug.Log("Scripts.Log: unsupported platform"); }
        
        public static void RegisterExternal(string name, IntPtr functionPtr) { Debug.Log("Scripts.RegisterExternal: unsupported platform"); }
    }
}

#endif
