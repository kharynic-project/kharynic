using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace org.kharynic.Scripting
{
    public static class Generator
    {
        public static void GenerateAllInterfaces(Assembly assembly = null, string scriptHeader = null)
        {
            var scriptingInterfaces = GetDeclaringTypes(assembly);
            var scriptPaths = new List<string>();
            foreach (var type in scriptingInterfaces)
            {
                var cSharpLayerGenerator = new CSharpLayerGenerator(type, BuildInfo.RootNamespace);
                var scriptLayerGenerator = new ScriptLayerGenerator(type, BuildInfo.RootNamespace, scriptHeader);
                cSharpLayerGenerator.Run();
                scriptLayerGenerator.Run();
                scriptPaths.Add(scriptLayerGenerator.Path);
                Debug.Log($"{type.FullName} generated");
            }
            GenerateFileList(scriptPaths);
        }

        private static void GenerateFileList(IEnumerable<string> paths)
        {
            var list = string.Join("\n", paths) + "\n";
            const string path = "../../scripts/filelist.generated.txt";
            GeneratorUtils.WriteFile(list, path, protectEditor: false);
        }


        
        // finds types this needs to be ran on
        private static IEnumerable<Type> GetDeclaringTypes(Assembly assembly)
        {
            var assemblies = assembly != null ? new[] {assembly} : AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.GetMethods()
                    .Any(m => m.GetCustomAttribute<ScriptableAttribute>() != null));
        }
    }
}
