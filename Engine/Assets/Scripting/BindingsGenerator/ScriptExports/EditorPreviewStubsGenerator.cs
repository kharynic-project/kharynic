using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Kharynic.Engine.Scripting.BindingsGenerator.ScriptExports
{
    internal class EditorPreviewStubsGenerator : GeneratorBase
    {
        public EditorPreviewStubsGenerator(Type targetType) : base(targetType)
        {
        }

        protected override bool ProtectEditor => false;
        public override string Path => GeneratorUtils.GetSourceFilePath(TargetType, "cs").Replace(".cs", ".preview.cs");
        
        protected override string GenerateCode()
        {
            var header =
                $"{GeneratorUtils.GetHeaderComment()}\n" +
                $"// this file is used by editor preview which cannot regenerate it, so it has to be commited\n\n" +
                $"#if UNITY_EDITOR\n\n" + 
                $"namespace {TargetType.Namespace}\n" +
                $"{{\n" +
                $"    public static class {TargetType.Name}\n" +
                $"    {{\n" +
                $"        public const bool Enabled = false;\n\n";
            var methodStubs = string.Join("\n\n", Functions.Select(GenerateMethodStub));
            var footer =
                $"\n    }}\n" +
                $"}}\n" + 
                "\n#endif\n";

            return
                header +
                methodStubs +
                footer;
        }

        private string GetTypeReference(Type type)
        {
            return type == typeof(void) ? "void" : type.FullName;
        }

        private string GenerateMethodStub(Function function)
        {
            var parameters = string.Join(", ", function.Params.Select(p => $"{p.Value} {p.Key}"));
            var header =
                $"        public static {GetTypeReference(function.Type)} {function.Name}({parameters})\n" +
                $"        {{\n";
            var log =
                $"            {typeof(Debug).FullName}.{nameof(Debug.Log)}(\"{TargetType.Name}.{function.Name}: unsupported platform\");\n";
            var returnStatement = function.Type != typeof(void) ? 
                $"            return default({function.Type});\n" : "";
            var footer =
                $"        }}";
            return
                header +
                log +
                returnStatement +
                footer;
        }
    }
}