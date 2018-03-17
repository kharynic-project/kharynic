// Code generated on 2018.03.17 by /Engine/Assets/Editor/EngineInterface.Generator.cs - DO NOT EDIT.

#if UNITY_EDITOR

using System;

namespace Kharynic.WebHost
{
    // this file is used by editor preview which cannot regenerate it, so it has to be commited
    public static class WebHost
    {
        public const bool Enabled = false;

        public static void OnEngineStart() { Kharynic.Engine.Debug.Log("WebHost.OnEngineStart: unsupported platform"); }

        public static void Log(string message) { Kharynic.Engine.Debug.Log("WebHost.Log: unsupported platform"); }

        public static string GetRootUrl() { Kharynic.Engine.Debug.Log("WebHost.GetRootUrl: unsupported platform"); return default(string); }
    }
}

#endif
