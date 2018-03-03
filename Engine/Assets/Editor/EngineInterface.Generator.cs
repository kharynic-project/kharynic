﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Kharynic.Engine.Scripting;
using UnityEditor;
using UnityEditor.Build;

namespace Kharynic.Engine.Editor
{
    public static class EngineInterface
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 3;
            private static string DefinitionPath => 
                GeneratorUtils.GetSourceFilePath(typeof(WebHost.EngineInterface), "js", isUnityAsset: false, isGenerated: false);

            private class Function
            {
                public string Name;
                public string Type;
                public Dictionary<string, string> Params; // Name, Type
            }

            [MenuItem("Kharynic/Generate script imports")]
            public static void Run()
            {
                var functions = ParseScriptInterfaceDefinition();
                GenerateEmscriptenLib(functions);
                GenerateExternImportStubs(functions);
                GenerateEditorPreviewStubs(functions);
                Debug.Log($"{typeof(Generator).FullName} finished");
            }
            
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                Run();
            }

            private static Function[] ParseScriptInterfaceDefinition()
            {
                // this parser is quite basic
                // feel free to substitute it with real thing if you find it worth the effort
                var lineSeparators = new[] {'\n', '\r'};
                var scriptLines = GeneratorUtils.ReadFile(DefinitionPath)
                    .Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);
                var signaturePattern = new Regex(
                    @"^ *(?<funcName>\w+) *: *function *\( *(?<param>(?<paramName>\w+) */\* *: *(?<paramType>\w+) *\*/ *,? *)*\) */\* *: *(?<funcType>\w+) *\*/ *$");

                var functions = scriptLines
                    .Where(l => signaturePattern.IsMatch(l))
                    .Select(l =>
                    {
                        var match = signaturePattern.Match(l);
                        var paramNames = match.Groups["paramName"].Captures;
                        var paramTypes = match.Groups["paramType"].Captures;
                        return new Function
                        {
                            Name = match.Groups["funcName"].Value,
                            Type = match.Groups["funcType"].Value,
                            Params = Enumerable.Range(0, paramNames.Count).ToDictionary(
                                i => paramNames[i].Value,
                                i => paramTypes[i].Value)
                        };
                    }).ToArray();
                return functions;
            }

            private static void GenerateExternImportStubs(IEnumerable<Function> functions)
            {
                var code = new StringBuilder(
                    $"{GeneratorUtils.GetHeaderComment()}\n\n" +
                    $"using {typeof(IntPtr).Namespace};\n" +
                    $"using {typeof(DllImportAttribute).Namespace};\n\n" +
                    $"namespace {typeof(WebHost.EngineInterface).Namespace}\n" +
                    $"{{\n" +
                    $"    public static class {nameof(WebHost.EngineInterface)}\n" +
                    $"    {{\n" +
                    $"        public const bool Enabled = true;\n\n");
                code.Append(String.Join("\n\n", functions.Select(function =>
                {
                    var parameters = string.Join(", ", function.Params.Select(p => $"{p.Value} {p.Key}"));
                    return
                        $"        [{nameof(DllImportAttribute)}(\"__Internal\")] public static extern {function.Type} {function.Name}({parameters});";
                })));
                code.Append(
                    $"\n    }}\n" +
                    $"}}\n");
                var path = GeneratorUtils.GetSourceFilePath(typeof(WebHost.EngineInterface), "cs", isUnityAsset: true);
                GeneratorUtils.WriteFile(code.ToString(), path);
            }

            private static void GenerateEditorPreviewStubs(IEnumerable<Function> functions)
            {
                var code = new StringBuilder(
                    $"{GeneratorUtils.GetHeaderComment()}\n\n" +
                    $"#if UNITY_EDITOR\n\n" + 
                    $"using {typeof(IntPtr).Namespace};\n\n" +
                    $"namespace {typeof(WebHost.EngineInterface).Namespace}\n" +
                    $"{{\n" +
                    $"    // this file is used by editor preview which cannot regenerate it, so it has to be commited\n" +
                    $"    public static class {nameof(WebHost.EngineInterface)}\n" +
                    $"    {{\n" +
                    $"        public const bool Enabled = false;\n\n");
                code.Append(String.Join("\n\n", functions.Select(function =>
                {
                    var parameters = string.Join(", ", function.Params.Select(p => $"{p.Value} {p.Key}"));
                    var returnStatement = function.Type != "void" ? $" return default({function.Type});" : "";
                    return
                        $"        public static {function.Type} {function.Name}({parameters}) " +
                        $"{{ {typeof(Debug).FullName}.{nameof(Debug.Log)}(\"{nameof(WebHost.EngineInterface)}.{function.Name}: unsupported platform\");{returnStatement} }}";
                })));
                code.Append(
                    $"\n    }}\n" +
                    $"}}\n" + 
                    $"\n#endif\n");
                var path = GeneratorUtils.GetSourceFilePath(typeof(WebHost.EngineInterface), "preview.cs", isUnityAsset: true);
                GeneratorUtils.WriteFile(code.ToString(), path, protectEditor: false);
            }

            private static string GetScriptHostObject()
            {
                var lineSeparators = new[] {'\n', '\r'};
                var scriptLines = GeneratorUtils.ReadFile(DefinitionPath)
                    .Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);
                var objectDeclarationRegexp = new Regex(@"^ *(?<object>[\w\.]+) *= *$");
                var declarationLine = scriptLines.First(l => objectDeclarationRegexp.IsMatch(l));
                return objectDeclarationRegexp.Match(declarationLine).Groups["object"].Value;
            }

            private static void GenerateEmscriptenLib(IEnumerable<Function> functions)
            {
                var scriptHostObject = GetScriptHostObject();
                var code = new StringBuilder(
                    $"{GeneratorUtils.GetHeaderComment()}\n" +
                    "mergeInto(LibraryManager.library, \n" +
                    "{\n");
                code.Append(string.Join(",\n\n", functions.Select(f =>
                {
                    var args = string.Join(", ",
                        f.Params.Keys.Select(paramName => 
                            f.Params[paramName] == "string" ? 
                                $"Pointer_stringify({paramName})" : 
                                paramName));
                    var call = $"{scriptHostObject}.{f.Name}({args})";
                    string body;
                    if (string.Equals(f.Type, "Void", StringComparison.InvariantCultureIgnoreCase))
                        body = 
                            $"        {call};";
                    else if (string.Equals(f.Type, "String", StringComparison.InvariantCultureIgnoreCase))
                    {
                        body = 
                            $"        var result = {call};\n" +
                            $"        var bufferSize = lengthBytesUTF8(result) + 1;\n" +
                            $"        var buffer = _malloc(bufferSize);\n" +
                            $"        stringToUTF8(result, buffer, bufferSize);\n" +
                            $"        return buffer;";
                    }
                    else
                        body = 
                            $"        return {call};";
                    
                    return
                        $"    {f.Name}: function({string.Join(", ", f.Params.Keys)})\n" +
                        $"    {{\n" + 
                        body + "\n" +
                        $"    }}";
                })));
                code.Append("\n});\n");
                var path = $"{Kharynic.Engine.BuildInfo.RelativeEngineAssetsPath}/Plugins/" +
                           $"{typeof(WebHost.EngineInterface).FullName}.generated.jslib";
                GeneratorUtils.WriteFile(code.ToString(), path);
            }
        }
    }
}