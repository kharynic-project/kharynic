using System;

namespace Kharynic.Engine.Scripting.BindingsGenerator.ScriptExports
{
    public static class Generator
    {
        public static void Run(Type targetType)
        {
            var generators = new GeneratorBase[]
            {
                new EmscriptenLayerGenerator(targetType),
                new ExternImportStubsGenerator(targetType),
                new EditorPreviewStubsGenerator(targetType) 
            };
            foreach (var generator in generators)
                generator.Run();
        }
    }
}