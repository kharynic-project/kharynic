﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
#if !UNITY_EDITOR
    using System.Linq;
#endif

namespace Kharynic.Engine
{
    public static partial class BuildInfo
    {
        public static string Config 
        =>
            #if !UNITY_EDITOR
                '#' + DefinedSymbols.Select(s => s.GetHashCode())
                    .Aggregate(0, (a, b) => a^b).ToString("x8").Substring(0, 3)
            #else
                "preview"
            #endif
        ;
        public static string Toolset => "Unity v" + UnityEngine.Application.unityVersion;
        public const string Type
        =
            #if DEBUG
                "debug"
            #else
                "release"
            #endif
        ;
        public const bool IsWebApp
        =
            #if (UNITY_WEBGL && !UNITY_EDITOR)
                true
            #else
                false
            #endif
        ;
        // this cannot use Path.DirectorySeparatorChar as it may differ between build platform and runtime platform
        private static readonly Lazy<string> LazyLocalProjectPath = new Lazy<string>(() =>
            Until(GetSourceFilePath().Replace('\\', '/'), RelativeEngineAssetsPath));
        public static string LocalProjectPath => LazyLocalProjectPath.Value;

        public static string RelativeEngineAssetsPath = "/Engine/Assets";
        public static readonly string RootNamespace = "Kharynic";

        
        
        // trick using CompilerServices that works with IL2CPP+WebAssembly (while stacktraces not)
        private static string GetSourceFilePath ([CallerFilePath] string path = null) => path;
        
        private static string Until(string a, string b)
        {
            var length = a.IndexOf(b, StringComparison.OrdinalIgnoreCase);
            return length > 0 ? a.Substring(0, length) : null;
        }
    }
}
