#if (UNITY_WEBGL && !UNITY_EDITOR)
    #define ENABLE_SCRIPTS
#endif

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace org.kharynic
{
    public static class Scripts
    {
        public const bool Enabled 
        =
            #if ENABLE_SCRIPTS
                true
            #else
                false
            #endif
        ;
        
        /*
         * Conditional("ENABLE_SCRIPTS"), DllImport("__Internal") don't work together.
         * Because of this, separate copies wrapped in #if ... #else ... #endif are needed.
         * TODO: way to get rid of duplication or generate this from interface or JS.
         */
        
        #if ENABLE_SCRIPTS

            [DllImport("__Internal")] public static extern void OnLoad();
            
            [DllImport("__Internal")] public static extern void Execute(string code);
            
            [DllImport("__Internal")] public static extern void Log(string message);
        
        #else
        
            public static void OnLoad() { Debug.Log($"{nameof(OnLoad)}: unsupported platform"); }
            
            public static void Execute(string code) { Debug.Log($"{nameof(Execute)}: unsupported platform"); }
            
            public static void Log(string message) { Debug.Log($"{nameof(Log)}: unsupported platform"); }
        
        #endif
    }
}
