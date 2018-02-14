using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;

namespace org.kharynic.Editor
{
    public static class Scripts
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 3;
            private const string ScriptInterfaceDefinitionPath = "../../scripts/Scripts.js";

            private class Function
            {
                public string Name;
                public Dictionary<string, string> Params;
            }
            
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                // this parser is quite basic lol
                // feel free to substitute it with real thing if you find it worth the effort
                var lineSeparators = new[] {'\n', '\r'};
                var scriptLines = GeneratorUtils.ReadFile(ScriptInterfaceDefinitionPath)
                    .Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);
                var signaturePattern = new Regex(
                    @"    (?<funcName>\w+) *: *function *\( *(?<param>(?<paramName>\w+) /\* *: *(?<paramType>\w+) *\*/ *,? *)*\) *");

                var functions = scriptLines
                    .Where(l => signaturePattern.IsMatch(l))
                    .Select(l =>
                    {
                        var match = signaturePattern.Match(l);
                        var paramNames = match.Groups["paramName"].Captures;
                        var paramTypes = match.Groups["paramType"].Captures;
                        return new Function
                        {
                            Name = match.Groups["funcName"]?.Value,
                            Params = Enumerable.Range(0, paramNames.Count).ToDictionary(
                                    i => paramNames[i].Value,
                                    i => paramTypes[i].Value)
                        };
                    }).ToArray();


                GenerateEmscriptenLib(functions);
                GenerateExternImportStubs(functions);
                GenerateEditorPreviewStubs(functions);
            }

            private void GenerateExternImportStubs(IEnumerable<Function> functions)
            {
                
            }

            private void GenerateEditorPreviewStubs(IEnumerable<Function> functions)
            {
                
            }

            private void GenerateEmscriptenLib(IEnumerable<Function> functions)
            {
                var code = new StringBuilder("mergeInto(LibraryManager.library, {\n");
                code.Append(string.Join(",\n", functions.Select(f =>
                {
                    var args = string.Join(", ",
                        f.Params.Keys.Select(paramName => 
                            f.Params[paramName] == "string" ? 
                                $"Pointer_stringify({paramName})" : 
                                paramName));
                    return
                        $"    {f.Name}: function({string.Join(", ", f.Params.Keys)}) {{\n" +
                        $"        window.WebHost.Scripts.{f.Name}({args});\n" +
                        $"    }}";
                })));
                code.Append("\n});\n");
                GeneratorUtils.WriteFile(code.ToString(), "Plugins/Scripts.generated.jslib", false);
            }
        }
    }
}