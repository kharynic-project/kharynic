// Code generated on 2018.03.02 by /Engine/Assets/Editor/EngineInterface.Generator.cs - DO NOT EDIT.

#if UNITY_EDITOR

using System;
using Kharynic.Engine;

namespace Kharynic.WebHost
{
    // this file is used by editor preview which cannot regenerate it, so it has to be commited
    public static class EngineInterface
    {
        public const bool Enabled = false;

        public static void OnLoad() { Debug.Log("EngineInterface.OnLoad: unsupported platform"); }

        public static void Execute(string code) { Debug.Log("EngineInterface.Execute: unsupported platform"); }

        public static void Log(string message) { Debug.Log("EngineInterface.Log: unsupported platform"); }

        public static void RegisterExternal(string name, IntPtr functionPtr) { Debug.Log("EngineInterface.RegisterExternal: unsupported platform"); }

        public static string GetRootUrl() { Debug.Log("EngineInterface.GetRootUrl: unsupported platform"); return default(string); }
    }
}

#endif
