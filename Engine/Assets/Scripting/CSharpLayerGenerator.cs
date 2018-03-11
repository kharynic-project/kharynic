using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Kharynic.Engine.Scripting
{
    // Generates C# glue code for calling .net methods from scripts.
    // TODO: support for reference parameters and return types
    internal class CSharpLayerGenerator : GeneratorBase
    {
        public const string GeneratedInterfaceSuffix = "_generated";
        const string ThisRefVar = "thisRef";

        public CSharpLayerGenerator(Type targetType) : base(targetType)
        {
        }

        public override string Path => GeneratorUtils.GetSourceFilePath(TargetType, "cs", isUnityAsset: true);

        protected override string GenerateCode()
        {
            var header =
                $"{GeneratorUtils.GetHeaderComment()}\n" +
                $"namespace {TargetType.Namespace}\n" +
                $"{{\n" +
                $"    [{typeof(GeneratedInterfaceAttribute).FullName}]\n" +
                $"    public static class {TargetType.Name}{GeneratedInterfaceSuffix}\n" +
                $"    {{\n";
            var footer =
                $"    }}\n" +
                $"}}\n";
            return
                header +
                string.Join("\n", Methods.Select(GenerateMethodWrapper)) +
                footer;
        }

        private string GetReturnType(MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
                return "void"; // CS0673 fix
            else if (method.ReturnType.IsPrimitive || method.ReturnType == typeof(string))
                return method.ReturnType.FullName;
            else
                return typeof(IntPtr).FullName; // MarshalDirectiveException fix
        }
        
        private string GenerateMethodWrapper(MethodInfo method)
        {
            var @params = GetParams(method);
            var paramDeclarations = @params.Select(p => $"{p.Type.FullName} {p.Name}");
            var paramList = string.Join(", ", paramDeclarations);
            var returnType = GetReturnType(method);
            var delegateDeclaration = 
                $"        public delegate {returnType} {method.Name}{nameof(Delegate)}({paramList});\n";
            var header =
                $"        [{typeof(AOT.MonoPInvokeCallbackAttribute).FullName}(typeof({method.Name}{nameof(Delegate)}))]\n" +
                $"        public static {returnType} {method.Name}({paramList})\n" +
                $"        {{\n";
            const string thisHandleVar = "thisHandle";
            var thisRefDeclaration = method.IsStatic ? "" :
                $"            var {thisHandleVar} = {typeof(GCHandle).FullName}.{nameof(GCHandle.FromIntPtr)}({ThisPtrVar});\n" +
                $"            var {ThisRefVar} = ({TargetType.FullName}) {thisHandleVar}.{nameof(GCHandle.Target)};\n";
            var stringParams = method.GetParameters().Where(p => p.ParameterType == typeof(string));
            var stringParamDeclarations = string.Join("", stringParams.Select(p =>
                $"            var {p.Name} = {typeof(Marshal).FullName}.{nameof(Marshal.PtrToStringAuto)}({p.Name}{PtrSuffix});\n"));
            var call = GetCallExpression(method);
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

        private string GetCallExpression(MethodInfo method)
        {
            const string getPrefix = "get_";
            const string setPrefix = "set_";
            var callSubject = method.IsStatic ? TargetType.FullName : ThisRefVar;
            string call;
            if (method.Name.StartsWith(getPrefix))
                call =
                    $"            return {callSubject}.{method.Name.Substring(getPrefix.Length)};\n";
            else if (method.Name.StartsWith(setPrefix))
            {
                var valueParam = method.GetParameters().Select(p => p.Name).Single();
                call =
                    $"            {callSubject}.{method.Name.Substring(setPrefix.Length)} = {valueParam};\n";
            }
            else
            {
                var argsList = string.Join(", ", method.GetParameters().Select(p => p.Name));
                call = $"{callSubject}.{method.Name}({argsList})";
                if (method.ReturnType == typeof(void))
                    call =
                        $"            {call};\n";
                else if (method.ReturnType.IsPrimitive || method.ReturnType == typeof(string))
                    call =
                        $"            return {call};\n";
                else
                {
                    var handleType = $"{typeof(GCHandleType).FullName}.{nameof(GCHandleType.Normal)}";
                    call =
                        $"            var resultRef = {call};\n" +
                        $"            var resultHandle = {typeof(GCHandle).FullName}.{nameof(GCHandle.Alloc)}(resultRef, {handleType});\n" +
                        $"            var result = {typeof(GCHandle).FullName}.{nameof(GCHandle.ToIntPtr)}(resultHandle);\n" +
                        $"            return result;\n";
                }
            }
            return call; 
        }

        // returns list of params with references changed to pointers and optional "this" pointer
        private static IEnumerable<VarInfo> GetParams(MethodBase method)
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
    }
}