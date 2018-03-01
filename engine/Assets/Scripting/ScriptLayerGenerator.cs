using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace org.kharynic.Scripting
{
    // Generates EcmaScript glue code for calling .net methods from scripts.
    // Generated code needs to be initialized at runtime by Runtime.RegisterAll.
    // TODO: support for reference parameters and return types
    internal class ScriptLayerGenerator : GeneratorBase
    {
        protected override bool ProtectEditor => false;
        private readonly string _scriptHeader;

        public ScriptLayerGenerator(Type targetType, string rootNamespace, string scriptHeader)
            : base(targetType, rootNamespace)
        {
            _scriptHeader = scriptHeader;
            
        }

        public override string Path => $"/scripts/{base.Path}.js";

        protected override string GenerateCode()
        {
            var @namespace = TargetType.Namespace != null ? (TargetType.Namespace + ".") : "";
            var header =
                GeneratorUtils.GetHeaderComment() + "\n" +
                (_scriptHeader ?? "") + "\n" +
                GetScriptNamespaceDeclarations() + "\n\n" +
                $"{@namespace}{TargetType.Name} = class\n" +
                $"{{\n";
            var constructor =
                $"    constructor({ThisPtrVar} /*: {typeof(IntPtr).FullName}*/)\n" +
                $"    {{\n" +
                $"        this.{ThisPtrVar} = {ThisPtrVar};\n" + 
                $"    }}\n\n";
            var footer =
                $"}}\n";
            return
                header +
                constructor +
                string.Join("\n\n", Methods.Select(GenerateMethodWrapper)) + "\n" +
                footer;
        }

        private static string GenerateMethodWrapper(MethodInfo method)
        {
            var @params = GetParams(method);
            var paramDeclarations = @params.Where(p => p.Name != ThisPtrVar).Select(p => $"{p.Name} /*: {p.Type.FullName}*/");
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
                $"        var ptr = this{(method.IsStatic?"":".constructor")}.{method.Name}{PtrSuffix};\n" +
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
        
        private static VarInfo[] GetParams(MethodBase method)
        {
            var thisParam =
                method.IsStatic
                    ? Enumerable.Empty<VarInfo>()
                    : new[] {new VarInfo {Name = ThisPtrVar, Type = method.DeclaringType}};
            return thisParam.Concat(
                    method.GetParameters().Select(p => new VarInfo {Name = p.Name, Type = p.ParameterType}))
                .ToArray();
        }
        
        // source: http://www.dyncall.org/docs/manual/manualse4.html
        // only some combinations are present in generated code, but probably all used ones
        private static readonly Dictionary<Type, string> DyncallSigTypeSymbols = new Dictionary<Type, string>
        {
            {typeof(bool), "B"},
            {typeof(char), "c"},
            {typeof(double), "d"},
            {typeof(float), "f"},
            {typeof(int), "i"},
            {typeof(IntPtr), "i"}, // replaced by int (in 32bit emscripten)
            {typeof(long), "j"},
            {typeof(object), "i"}, // replaced by IntPtr
            {typeof(void), "v"}
        };
        
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
            const string rootObject = "window";
            var @namespace = TargetType.Namespace;
            if (@namespace == null)
                return "";
            var namespaceSegments = @namespace.Split('.');
            var namespaceDeclarations =
                Enumerable.Range(1, namespaceSegments.Length)
                    .Select(i => (i == 1 ? $"{rootObject}." : "") + string.Join(".", namespaceSegments.Take(i)))
                    .Select(n => $"{n} = {n} || {{}};");
            return string.Join("\n", namespaceDeclarations);
        }
    }
}