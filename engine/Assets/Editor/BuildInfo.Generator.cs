using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Kharynic.Engine.Scripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Kharynic.Engine.Editor
{
    public static class BuildInfo
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 1;

            public void OnPreprocessBuild(BuildTarget target, string buildPath)
            {
                var code = new StringBuilder(
                    $"namespace {typeof(Kharynic.Engine.BuildInfo).Namespace}\n" +
                    "{\n" +
                    $"    public static partial class {nameof(BuildInfo)}\n" +
                    "    {\n");
                Generate(code, target);
                code.Append("\n");
                GenerateCompilationSymbols(code);
                code.Append(
                    "    }\n" +
                    "}\n");
                var path = $"{Kharynic.Engine.BuildInfo.RelativeEngineAssetsPath}/{nameof(BuildInfo)}.generated.cs";
                GeneratorUtils.WriteFile(code.ToString(), path);
                Debug.Log($"{GetType().FullName} finished");
                //TODO: explore usages of EditorUtility.CompileCSharp()
            }




            private static void Generate(StringBuilder code, BuildTarget target)
            {
                var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                var buildNumber = Version.IncrementAndGet().ToString();
                var fields = new Dictionary<string, string>
                {
                    {"Platform", target.ToString()},
                    {"Runtime", PlayerSettings.GetScriptingBackend(buildTargetGroup).ToString()},
                    {"TranspilationTarget", PlayerSettings.WebGL.useWasm ? "WebAssembly" : "JavaScript"},
                    {"BuildDate", DateTime.Now.ToString("MMM dd yyyy", CultureInfo.InvariantCulture)},
                    {"User", (Environment.UserName + "@" + Environment.UserDomainName).ToLowerInvariant()},
                    {"Version", buildNumber}
                };
                code.Append($"        {GeneratorUtils.GetHeaderComment()}\n");
                foreach (var field in fields)
                    code.Append(
                        $"        public const string {field.Key} = \"{field.Value}\";\n");
            }

            private static void GenerateCompilationSymbols(StringBuilder code)
            {
                // source: csproj file + unity docs
                var listPath = Application.dataPath + $"/Editor/{nameof(BuildInfo)}.DefinedSymbols.txt";
                var symbols = File.ReadAllLines(listPath);
                code.Append(
                    $"        {GeneratorUtils.GetHeaderComment()}\n" +
                    $"        public static readonly {typeof(IEnumerable<>).Namespace}.{nameof(IEnumerable<string>)}<string> DefinedSymbols = new[]\n" +
                    "        {\n");
                foreach (var symbol in symbols)
                    code.Append(
                        $"            #if {symbol}\n" +
                        $"                \"{symbol}\",\n" +
                        "            #endif\n");
                code.Append(
                    "        };\n");
            }
        }
    }
}
