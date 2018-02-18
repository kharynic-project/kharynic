using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEditor.Build;

namespace org.kharynic.Editor
{
    public static partial class EngineExternals
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 2;
            
            [MenuItem("Kharynic/Generate externals")]
            public static void Run()
            {
                var methods = typeof(kharynic.EngineExternals)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static);
                GeneratePInvokeMethodWrappers(methods);
                GenerateScriptWrappers(methods);
                Debug.Log($"{typeof(Generator).FullName} finished");
            }
            
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                Run();
            }

            private static void GeneratePInvokeMethodWrappers(IEnumerable<MethodInfo> methods)
            {
                var code = new StringBuilder(
                    "using System;\n\n" +
                    $"namespace {typeof(kharynic.EngineExternals).Namespace}\n" +
                    "{\n" +
                    $"    public static partial class {nameof(EngineExternals)}\n" +
                    "    {\n" +
                    $"        {GeneratorUtils.GetHeaderComment()}\n" +
                    $"        public class {nameof(kharynic.EngineExternals.Wrappers)}\n" +
                    "        {\n");
                foreach (var method in methods)
                {
                    var paramDeclarations = method.GetParameters().Select(p =>
                    {
                        var type = p.ParameterType;
                        var name = p.Name;
                        if (type == typeof(string))
                        {
                            type = typeof(IntPtr);
                            name += "Ptr";
                        }
                        return $"{type.Name} {name}";
                    });
                    var paramList = string.Join(", ", paramDeclarations);
                    var stringParams = method.GetParameters().Where(p => p.ParameterType == typeof(string));
                    var argsList = string.Join(", ", method.GetParameters().Select(p => p.Name));
                    var returnType = method.ReturnType.Name;
                    if (returnType == "Void")
                        returnType = "void";
                    var returnStatement = method.ReturnType != typeof(void) ? "return " : "";
                    code.Append(
                        $"            public delegate {returnType} {method.Name}Delegate({paramList});\n" +
                        "            \n" +
                        $"            [{typeof(AOT.MonoPInvokeCallbackAttribute).FullName}(typeof({method.Name}Delegate))]\n" +
                        $"            public static {returnType} {method.Name}({paramList})\n" +
                        "            {\n");
                    foreach (var stringParam in stringParams)
                    {
                        code.Append(
                            $"                var {stringParam.Name} = {typeof(Marshal).FullName}.{nameof(Marshal.PtrToStringAuto)}({stringParam.Name}Ptr);\n");
                    }
                    code.Append(
                        $"                {returnStatement}{nameof(kharynic.EngineExternals)}.{method.Name}({argsList});\n" +
                        "            }\n\n\n");
                }
                code.Append(
                    "        }\n" + 
                    "    }\n" + 
                    "}\n");
                GeneratorUtils.WriteFile(code.ToString(), $"{nameof(kharynic.EngineExternals)}.generated.cs");
            }
            
            // source: http://www.dyncall.org/docs/manual/manualse4.html
            // only some combinations are present in generated code, but probably all used ones
            private static readonly Dictionary<Type, string> DyncallSigTypeSymbols = new Dictionary<Type, string>
            {
                { typeof(double), "d" },
                { typeof(float), "f" },
                { typeof(int), "i" },
                { typeof(long), "j" },
                { typeof(void), "v" },
                { typeof(string), "i" } // is replaced by IntPtr and later by int
            };

            private static void GenerateScriptWrappers(IEnumerable<MethodInfo> methods)
            {
                var @namespace = $"window.{nameof(Engine)}";
                var code = new StringBuilder(
                    GeneratorUtils.GetHeaderComment() + "\n" +
                    "// this file is used by scripts which cannot regenerate it, so it has to be commited\n\n" +
                    "// *********************\n" +
                    "//       Externals\n" +    	
                    $"//  Phoenix V{kharynic.BuildInfo.Version}\n" +
                    "// *********************\n\n" +
                    $"{@namespace} = {@namespace} || {{}};\n\n");
                foreach (var method in methods)
                {
                    var @params = method.GetParameters();
                    var paramDeclarations = @params.Select(p => $"{p.Name} /*: {p.ParameterType.Name}*/");
                    var stringParams = @params.Where(p => p.ParameterType == typeof(string));
                    var returnTypeSymbol = DyncallSigTypeSymbols[method.ReturnType];
                    var paramTypeSymbols = @params.Select(p => DyncallSigTypeSymbols[p.ParameterType]);
                    var sig = returnTypeSymbol + string.Join("", paramTypeSymbols);
                    var paramNames = @params.Select(p => p.Name);
                    code.Append(
                        $"{@namespace}.{method.Name} = function({string.Join(", ", paramDeclarations)}) /*: {method.ReturnType.Name}*/\n" +
                        "{\n");
                    foreach (var stringParam in stringParams)
                        code.Append($"    {stringParam.Name} = this.GetPtrFromString({stringParam.Name});\n");
                    code.Append(
                        $"    var sig = \"{sig}\";\n" +
                        $"    var ptr = this.externals.{method.Name};\n" +
                        $"    var args = [ {string.Join(",", paramNames)} ];\n" + 
                        $"    var result = this.dynCall(sig, ptr, args);\n");
                    if (method.ReturnType == typeof(string))
                        code.Append("    return this.GetStringFromPtr(result);\n");
                    else if (method.ReturnType != typeof(void))
                        code.Append("    return result;\n");
                    code.Append("}\n\n");
                }

                var filename = $"{nameof(Engine)}.generated.js";
                code.Append($"console.log(\"{filename} loaded\");\n");
                GeneratorUtils.WriteFile(code.ToString(), $"../../scripts/{filename}", protectEditor: false);
            }
        }
    }
}
