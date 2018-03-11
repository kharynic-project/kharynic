#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Kharynic.Engine
{
    // replaced during build by code generated by Kharynic.Engine.Editor.BuildInfo.Generator
    // used only for in-editor preview which doesn't run generators
    public partial class BuildInfo
    {
        public static readonly IEnumerable<string> DefinedSymbols = new[]{"UNITY_EDITOR"};
        public static readonly string Platform = "UnityEditor/" + EditorUserBuildSettings.activeBuildTarget;
        public static readonly string Runtime = ScriptingImplementation.Mono2x.ToString();
        public static readonly string TranspilationTarget = "CIL";
        public static readonly string BuildDate = DateTime.Now.ToString("MMM dd yyyy", CultureInfo.InvariantCulture);
        public static readonly string User = (Environment.UserName + "@" + Environment.UserDomainName).ToLowerInvariant();
        public static readonly string Version = Kharynic.Engine.Version.GetForPreview().ToString();
    }
}
#endif