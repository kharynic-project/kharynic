using System;
using System.Linq;

namespace Kharynic.Engine.Scripting.BindingsGenerator.ScriptExports
{
    internal class EmscriptenLayerGenerator : GeneratorBase
    {
        public EmscriptenLayerGenerator(Type targetType) : base(targetType)
        {
        }

        public override string Path => GeneratorUtils.GetSourceFilePath(TargetType, "jslib");
        
        protected override string GenerateCode()
        {
            var header =
                $"{GeneratorUtils.GetHeaderComment()}\n" +
                "mergeInto(LibraryManager.library, \n" +
                "{\n";
            var methodWrappers = string.Join(",\n\n", Functions.Select(GenerateMethodWrapper));
            var footer = "\n});\n";
            
            return
                header +
                methodWrappers +
                footer;
        }

        private string GenerateMethodWrapper(Function function)
        {
            var args = string.Join(", ",
                function.Params.Keys.Select(paramName => 
                    function.Params[paramName] == typeof(string) ? 
                        $"Pointer_stringify({paramName})" : 
                        paramName));
            var call = $"{TargetType.FullName}.{(function.IsStatic ? "" : "Instance.")}{function.Name}({args})";
            string body;
            if (function.Type == typeof(void))
                body = 
                    $"        {call};";
            else if (function.Type == typeof(string))
                body =
                    $"        var result = {call};\n" +
                    $"        var bufferSize = lengthBytesUTF8(result) + 1;\n" +
                    $"        var buffer = _malloc(bufferSize);\n" +
                    $"        stringToUTF8(result, buffer, bufferSize);\n" +
                    $"        return buffer;";
            else
                body =
                    $"        return {call};";

            return
                $"    {function.Name}: function({string.Join(", ", function.Params.Keys)})\n" +
                $"    {{\n" + 
                body + "\n" +
                $"    }}";
        }
    }
}
