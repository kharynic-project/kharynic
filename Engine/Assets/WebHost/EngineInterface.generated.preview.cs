// Code generated on 2018.03.17 by /Engine/Assets/Editor/EngineInterface.Generator.cs - DO NOT EDIT.

#if UNITY_EDITOR

using System;

namespace Kharynic.WebHost
{
    // this file is used by editor preview which cannot regenerate it, so it has to be commited
    public static class EngineInterface
    {
        public const bool Enabled = false;

        public static void OnLoad() { Kharynic.Engine.Debug.Log("EngineInterface.OnLoad: unsupported platform"); }

        public static void Log(string message) { Kharynic.Engine.Debug.Log("EngineInterface.Log: unsupported platform"); }

        public static string GetRootUrl() { Kharynic.Engine.Debug.Log("EngineInterface.GetRootUrl: unsupported platform"); return default(string); }
    }
}

#endif
