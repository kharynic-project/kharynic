using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kharynic.Engine.Scripting
{
    public static class ScriptingInterfaceGenerator
    {
        // omit assembly to search trough all loaded ones (may be slow)
        public static void GenerateAllInterfaces(Assembly assembly = null, string scriptHeader = null)
        {
            var scriptableTypes = GetScriptableTypes(assembly);
            var scriptPaths = new List<string>();
            var rootNamespace = BuildInfo.RootNamespace;
            foreach (var type in scriptableTypes)
            {
                var cSharpLayerGenerator = new CSharpLayerGenerator(type);
                var scriptLayerGenerator = new ScriptLayerGenerator(type, scriptHeader);
                cSharpLayerGenerator.Run();
                scriptLayerGenerator.Run();
                scriptPaths.Add(scriptLayerGenerator.Path.Replace("//", "/"));
                Debug.Log($"{type.FullName} generated");
            }
            GenerateScriptFileList(scriptPaths);
        }

        private static void GenerateScriptFileList(IEnumerable<string> paths)
        {
            var list = 
                GeneratorUtils.GetHeaderComment() + "\n" +
                string.Join("\n", paths) + "\n";
            const string path = "WebHost/WebHost.generated.src";
            GeneratorUtils.WriteFile(list, path);
        }

        // finds types this needs to be ran on
        private static IEnumerable<Type> GetScriptableTypes(Assembly assembly)
        {
            var assemblies = assembly != null ? new[] {assembly} : AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.GetMethods()
                    .Any(m => m.GetCustomAttribute<ScriptableAttribute>() != null));
        }
    }
}
