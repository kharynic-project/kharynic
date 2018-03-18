// Code generated on 2018.03.18 by /Engine/Assets/Scripting/BindingsGenerator/ScriptExports/EditorPreviewStubsGenerator.cs - DO NOT EDIT.
// this file is used by editor preview which cannot regenerate it, so it has to be commited

#if UNITY_EDITOR

namespace Kharynic.WebHost
{
    public static class WebHost
    {
        public const bool Enabled = false;

        public static void OnEngineStart()
        {
            Kharynic.Engine.Debug.Log("WebHost.OnEngineStart: unsupported platform");
        }

        public static void OnEngineReady()
        {
            Kharynic.Engine.Debug.Log("WebHost.OnEngineReady: unsupported platform");
        }

        public static void Log(System.String message)
        {
            Kharynic.Engine.Debug.Log("WebHost.Log: unsupported platform");
        }

        public static System.String GetRootUrl()
        {
            Kharynic.Engine.Debug.Log("WebHost.GetRootUrl: unsupported platform");
            return default(System.String);
        }
    }
}

#endif
