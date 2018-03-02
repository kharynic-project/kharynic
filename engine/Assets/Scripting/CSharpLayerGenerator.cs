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

        public CSharpLayerGenerator(Type targetType, string rootNamespace) 
            : base(
                targetType, 
                rootNamespace)
        {
        }

        public override string Path => $"{base.Path.Replace("/Engine/", "/Engine/Assets/")}.cs";

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

        private string GenerateMethodWrapper(MethodInfo method)
        {
            var @params = GetParams(method);
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
            const string thisHandleVar = "thisHandle";
            var thisRefDeclaration = method.IsStatic ? "" :
                $"            var {thisHandleVar} = {typeof(GCHandle).FullName}.{nameof(GCHandle.FromIntPtr)}({ThisPtrVar});\n" +
                $"            var {thisRefVar} = ({TargetType.FullName}) {thisHandleVar}.{nameof(GCHandle.Target)};\n";
            var stringParams = method.GetParameters().Where(p => p.ParameterType == typeof(string));
            var stringParamDeclarations = string.Join("", stringParams.Select(p =>
                $"            var {p.Name} = {typeof(Marshal).FullName}.{nameof(Marshal.PtrToStringAuto)}({p.Name}{PtrSuffix});\n"));
            var callSubject = method.IsStatic ? TargetType.FullName : thisRefVar;
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