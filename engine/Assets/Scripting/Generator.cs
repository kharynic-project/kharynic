using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace org.kharynic.Scripting
{
    public class Generator
    {
        private readonly CSharpLayerGenerator _cSharpLayerGenerator;
        private readonly ScriptLayerGenerator _scriptLayerGenerator;

        public Generator(
            Type targetType, 
            string rootNamespace,
            string scriptHeader = null)
        {
            _cSharpLayerGenerator = new CSharpLayerGenerator(targetType, rootNamespace);
            _scriptLayerGenerator = new ScriptLayerGenerator(targetType, rootNamespace, scriptHeader);
        }

        public void Run()
        {
            _cSharpLayerGenerator.Run();
            _scriptLayerGenerator.Run();
        }

        public static void GenerateAllInterfaces(Assembly assembly, string scriptHeader = null)
        {
            var scriptingInterfaces = GetDeclaringTypes(assembly);
            foreach (var type in scriptingInterfaces)
            {
                var generator = new Generator(type, BuildInfo.RootNamespace, scriptHeader);
                generator.Run();
                Debug.Log($"{generator.GetType().Name} : {type.Name} finished");
            }
        }
        
        // finds types this needs to be ran on
        private static IEnumerable<Type> GetDeclaringTypes(Assembly assembly = null)
        {
            var assemblies = assembly != null ? new[] {assembly} : AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.GetMethods()
                    .Any(m => m.GetCustomAttribute<ScriptableAttribute>() != null));
        }
    }
}
