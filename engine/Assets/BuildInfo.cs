﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace org.kharynic
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
        private static string Until(string a, string b)
        {
            var length = a.ToLowerInvariant()
                .IndexOf(b, StringComparison.Ordinal);
            return length > 0 ? a.Substring(0, length) : null;
        }
        public static string LocalProjectPath
        {
            get
            {
                const string engineAssetsPathSegment = "/engine/assets";
                return Until(GetSourceFilePath().Replace('\\', '/'), engineAssetsPathSegment) ??
                       Until(LocalAssetsPath, engineAssetsPathSegment);
            }
        }

        // trick using CompilerServices that works with IL2CPP+WebAssembly (while stacktraces not)
        private static string GetSourceFilePath ([CallerFilePath] string path = null) => path;
    }
}
