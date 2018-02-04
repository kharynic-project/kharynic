using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace org.kharynic.Editor
{
    public static partial class BuildInfo
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 1;

            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                var code = new StringBuilder(
                    "using System.Collections.Generic;\n" +
                    "namespace org.kharynic\n" +
                    "{\n" +
                    "    public static partial class BuildInfo\n" +
                    "    {\n");
                GenerateBuildInfo(code, target);
                code.Append("\n");
                GenerateCompilationSymbols(code);
                code.Append(
                    "    }\n" +
                    "}\n");
                WriteGeneratedCode(code.ToString(), "BuildInfo.generated.cs");
                //TODO: explore usages of EditorUtility.CompileCSharp()
            }

            private static string SetField(string code, string fieldName, string value)
            {
                const string prefix = "public const string ";
                var template = $"{prefix}{fieldName} *= *\"[^\"]*\" *;";
                var replacement = $"{prefix}{fieldName} = \"{value}\";";
                return new Regex(template).Replace(code, replacement);
            }

            private static string IncrementBuildNumberAndGetVersion()
            {
                var filePath = Application.dataPath + "/Editor/BuildInfo.Version.txt";

                // major.minor.build+hash
                var version = File.ReadAllText(filePath).Split('.', '+').ToArray();
                version[2] = (int.Parse(version[2]) + 1).ToString();
                version[3] = Guid.NewGuid().GetHashCode().ToString("x8").Substring(0, 3);
                var versionString = $"{version[0]}.{version[1]}.{version[2]}+{version[3]}";
                File.WriteAllText(filePath, versionString);
                return versionString;
            }

            private static string GetHeaderComment([CallerMemberName] string callerMemberName = null)
            {
                return
                    $"// generated on {DateTime.Now:yyyy.MM.dd HH:mm} by {typeof(Generator).FullName}" +
                    ((callerMemberName != null) ? $".{callerMemberName}" : "");
            }

            private static void WriteGeneratedCode(string code, string path)
            {
                // editor has to use stubs from repository instead or build won't start
                code = "#if !UNITY_EDITOR\n\n" + code + "\n#endif\n";
                File.WriteAllText(Application.dataPath + "/" + path, code);
                AssetDatabase.Refresh();
            }

            private static void GenerateBuildInfo(StringBuilder code, BuildTarget target)
            {
                var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                var buildNumber = IncrementBuildNumberAndGetVersion();
                var fields = new Dictionary<string, string>
                {
                    {"Platform", target.ToString()},
                    {"Runtime", PlayerSettings.GetScriptingBackend(buildTargetGroup).ToString()},
                    {"TranspilationTarget", PlayerSettings.WebGL.useWasm ? "WebAssembly" : "JavaScript"},
                    {"BuildDate", DateTime.Now.ToString("MMM dd yyyy", CultureInfo.InvariantCulture)},
                    {"User", (Environment.UserName + "@" + Environment.UserDomainName).ToLowerInvariant()},
                    {"LocalAssetsPath", Application.dataPath},
                    {"Version", buildNumber}
                };
                code.Append($"        {GetHeaderComment()}\n");
                foreach (var field in fields)
                    code.Append(
                        $"        public const string {field.Key} = \"{field.Value}\";\n");
            }

            private static void GenerateCompilationSymbols(StringBuilder code)
            {
                // source: csproj file + unity docs
                var listPath = Application.dataPath + "/Editor/BuildInfo.DefinedSymbols.txt";
                var symbols = File.ReadAllLines(listPath);
                code.Append(
                    $"        {GetHeaderComment()}\n" +
                    "        public static readonly IEnumerable<string> DefinedSymbols = new[]\n" +
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
