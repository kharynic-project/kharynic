using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace org.kharynic.Scripting
{
    // Generates C# and EcmaScript glue code for calling .net methods from scripts.
    // Scripting side needs to be initialized at runtime with data gathered by Registration.GetInterfaceData.
    // TODO: support for reference parameters and return types
    public class Generator
    {
        private const string ThisPtrVar = "thisPtr";
        private const string PtrSuffix = "Ptr";
        private const string GeneratedInterfaceSuffix = "_generated";
        
        // source: http://www.dyncall.org/docs/manual/manualse4.html
        // only some combinations are present in generated code, but probably all used ones
        private static readonly Dictionary<Type, string> DyncallSigTypeSymbols = new Dictionary<Type, string>
        {
            {typeof(bool), "B"},
            {typeof(char), "c"},
            {typeof(double), "d"},
            {typeof(float), "f"},
            {typeof(int), "i"},
            {typeof(IntPtr), "i"},
            {typeof(long), "j"},
            {typeof(object), "i"}, // replaced by IntPtr replaced by int (in 32bit emscripten)
            {typeof(void), "v"}
        };
        
        private readonly Type _targetType;
        private readonly string _rootNamespace;
        private readonly string _scriptHeader;
        private readonly IEnumerable<MethodInfo> _methods;

        public Generator(
            Type targetType, 
            string rootNamespace,
            string scriptHeader = null)
        {
            _targetType = targetType;
            _rootNamespace = rootNamespace;
            _scriptHeader = scriptHeader;

            _methods = targetType
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ScriptableAttribute>() != null);
        }

        public void Run()
        {
            GenerateCSharpLayer();
            GenerateScriptLayer();
        }

        private void GenerateCSharpLayer()
        {
            var header =
                $"namespace {_targetType.Namespace}\n" +
                $"{{\n" +
                $"    {GeneratorUtils.GetHeaderComment()}\n" +
                $"    [{typeof(GeneratedInterfaceAttribute).FullName}]\n" +
                $"    public class {_targetType.Name}{GeneratedInterfaceSuffix}\n" +
                $"    {{\n";
            var footer =
                $"    }}\n" +
                $"}}\n";
            var code =
                header +
                string.Join("\n", _methods.Select(GenerateCSharpMethodWrapper)) +
                footer;
            var filePath =
                new Regex($"^{_rootNamespace}")
                    .Replace(_targetType.Namespace ?? "", "")
                    .Replace(".", "/") + $"/{_targetType.Name}.generated.cs";
            GeneratorUtils.WriteFile(code, filePath);
        }
        
        public static IEnumerable<Type> GetDeclaringTypes(Assembly assembly = null)
        {
            var assemblies = assembly != null ? new[] {assembly} : AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.GetMethods()
                    .Any(m => m.GetCustomAttribute<ScriptableAttribute>() != null));
        }

        private void GenerateScriptLayer()
        {
            var filePath =
                new Regex($"^{_rootNamespace}")
                    .Replace(_targetType.Namespace ?? "", "")
                    .Replace(".", "/") + $"/{_targetType.Name}.generated.js";
            var @namespace = _targetType.Namespace != null ? (_targetType.Namespace + ".") : "";
            var header =
                GeneratorUtils.GetHeaderComment() + "\n" +
                (_scriptHeader ?? "") + "\n" +
                GetScriptNamespaceDeclarations() + "\n\n" +
                $"{@namespace}{_targetType.Name} = class\n" +
                $"{{\n";
            var constructor =
                $"    constructor(thisPtr /*: IntPtr*/)\n" +
                $"    {{\n" +
                $"        this.thisPtr = thisPtr;\n" +
                $"    }}\n\n";
            var footer =
                $"}}\n";
            var code = 
                header +
                (_methods.Any(m => !m.IsStatic) ? constructor : "") +
                string.Join("\n", _methods.Select(GenerateScriptMethodWrapper)) + "\n" +
                footer;
            GeneratorUtils.WriteFile(code, $"../../scripts/{filePath}", protectEditor: false);
        }

        private string GenerateCSharpMethodWrapper(MethodInfo method)
        {
            var @params = GetParamsForCSharp(method);
            var paramDeclarations = @params.Select(p => $"{p.Type.FullName} {p.Name}");
            var paramList = string.Join(", ", paramDeclarations);
            var returnType = method.ReturnType == typeof(void) ? "void" : method.ReturnType.FullName; // CS0673 fix
            var delegateDeclaration = 
                $"        public delegate {returnType} {method.Name}{nameof(Delegate)}({paramList});\n";
            var header =
                $"        [{typeof(AOT.MonoPInvokeCallbackAttribute).FullName}(typeof({method.Name}{nameof(Delegate)}))]\n" +
                $"        public static {returnType} {method.Name}({paramList})\n" +
                $"        {{\n";
            const string thisRefVar = "thisRef";
            var thisRefDeclaration = method.IsStatic ? "" :
                $"            var thisHandle = {typeof(GCHandle).FullName}.{nameof(GCHandle.FromIntPtr)}({ThisPtrVar});\n" +
                $"            var {thisRefVar} = ({_targetType.FullName}) thisHandle.{nameof(GCHandle.Target)};\n";
            var stringParams = method.GetParameters().Where(p => p.ParameterType == typeof(string));
            var stringParamDeclarations = string.Join("", stringParams.Select(p =>
                $"            var {p.Name} = {typeof(Marshal).FullName}.{nameof(Marshal.PtrToStringAuto)}({p.Name}Ptr);\n"));
            var callSubject = method.IsStatic ? _targetType.FullName : thisRefVar;
            var argsList = string.Join(", ", method.GetParameters().Select(p => p.Name));
            var returnStatement = method.ReturnType != typeof(void) ? "return " : "";
            var call =
                $"            {returnStatement}{callSubject}.{method.Name}({argsList});\n";
            var footer = 
                $"        }}\n";
            return
                delegateDeclaration +
                header +
                thisRefDeclaration + 
                stringParamDeclarations + 
                call + 
                footer;
        }

        private static string GenerateScriptMethodWrapper(MethodInfo method)
        {
            var @params = GetParamsForScripts(method);
            var paramDeclarations = @params.Select(p => $"{p.Name} /*: {p.Type.FullName}*/");
            var staticKeyword = method.IsStatic ? "static " : "";
            var name = method.Name; //TODO? .Replace("get_", "get ").Replace("set_", "set ");
            var header =
                $"    {staticKeyword}{name}({string.Join(", ", paramDeclarations)}) /*: {method.ReturnType.FullName}*/\n" +
                $"    {{\n";
            var stringParamsConversion = @params.Where(p => p.Type == typeof(string)).Select(p =>
                $"        {p.Name} = {typeof(Runtime).FullName}.GetPtrFromString({p.Name});\n");
            var returnTypeSymbol = GetDyncallSigTypeSymbol(method.ReturnType);
            var paramTypeSymbols = @params.Select(p => GetDyncallSigTypeSymbol(p.Type));
            var sig = returnTypeSymbol + string.Join("", paramTypeSymbols);
            var paramNames = @params.Select(p => p.Name);
            var call =
                $"        var sig = \"{sig}\";\n" +
                $"        var ptr = this.constructor.{method.Name}Ptr;\n" +
                $"        var args = [ {string.Join(", ", paramNames)} ];\n" +
                $"        var result = {typeof(Runtime).FullName}.DynCall(sig, ptr, args);\n";
            var resultConversion = (method.ReturnType == typeof(string)) ? 
                $"        result = {typeof(Runtime).FullName}.GetStringFromPtr(result);\n" : "";
            var returnStatement = (method.ReturnType != typeof(void)) ?
                $"        return result;\n" : "";
            var footer =
                $"    }}";

            return
                header +
                string.Join("\n", stringParamsConversion) +
                call +
                resultConversion +
                returnStatement +
                footer;
        }
        
        private static VarInfo[] GetParamsForScripts(MethodBase method)
        {
            return method.GetParameters()
                .Select(p => new VarInfo {Name = p.Name, Type = p.ParameterType})
                .ToArray();
        }

        // returns list of params with references changed to pointers and optional "this" pointer
        private static VarInfo[] GetParamsForCSharp(MethodBase method)
        {
            var thisParam =
                method.IsStatic
                    ? Enumerable.Empty<VarInfo>()
                    : new[] {new VarInfo {Name = ThisPtrVar, Type = typeof(IntPtr)}};
            return thisParam.Concat(
                method.GetParameters()
                    .Select(p =>
                        p.ParameterType.IsPrimitive
                            ? new VarInfo {Name = p.Name, Type = p.ParameterType}
                            : new VarInfo {Name = p.Name + PtrSuffix, Type = typeof(IntPtr)}))
                .ToArray();
        }

        private static string GetDyncallSigTypeSymbol(Type type)
        {
            if (!type.IsPrimitive && type != typeof(void))
                type = typeof(object); // all reference types threated the same way
            if (DyncallSigTypeSymbols.ContainsKey(type))
                return DyncallSigTypeSymbols[type];
            else
                throw new ArgumentException($"Unsupported type: {type.FullName}", nameof(type));
        }

        private string GetScriptNamespaceDeclarations()
        {
            var @namespace = _targetType.Namespace;
            if (@namespace == null)
                return "";
            var namespaceSegments = @namespace.Split('.');
            var namespaceDeclarations =
                Enumerable.Range(1, namespaceSegments.Length)
                    .Select(i => string.Join(".", namespaceSegments.Take(i)))
                    .Select(n => $"{n} = {n} || {{}};");
            return string.Join("\n", namespaceDeclarations);
        }
    }
}