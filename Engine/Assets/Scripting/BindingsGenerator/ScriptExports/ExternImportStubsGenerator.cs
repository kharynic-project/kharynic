using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Kharynic.Engine.Scripting.BindingsGenerator.ScriptExports
{
    internal class ExternImportStubsGenerator : GeneratorBase
    {
        public ExternImportStubsGenerator(Type targetType) : base(targetType)
        {
        }

        public override string Path => GeneratorUtils.GetSourceFilePath(TargetType, "cs");
        
        protected override string GenerateCode()
        {
            var header =
                $"{GeneratorUtils.GetHeaderComment()}\n\n" +
                $"namespace {TargetType.Namespace}\n" +
                $"{{\n" +
                $"    public static class {TargetType.Name}\n" +
                $"    {{\n" +
                $"        public const bool Enabled = true;\n\n";
            var methodStubs = string.Join("\n\n", Functions.Select(GenerateMethodStub));
            var footer =
                $"\n    }}\n" +
                $"}}\n";

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
            var attribute = $"[{typeof(DllImportAttribute).FullName}(\"__Internal\")]";
            return
                $"        {attribute} public static extern {GetTypeReference(function.Type)} {function.Name}({parameters});";
        }
    }
}