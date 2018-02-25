using org.kharynic.Scripting;

namespace org.kharynic.Editor
{
    public class EcmaScriptInterfaceGeneratorAdapter : UnityEditor.Build.IPreprocessBuild
    {
        public int callbackOrder => 2;

        [UnityEditor.MenuItem("Kharynic/Generate externals")]
        public static void Run()
        {
            var scriptHeader =
                $"// this file is used by scripts which cannot regenerate it, so it has to be commited\n\n" +
                $"// *********************\n" +
                $"//       Externals\n" +
                $"//  Phoenix V{kharynic.BuildInfo.Version}\n" +
                $"// *********************\n\n";
            
            var scriptingInterfaces =  Generator.GetDeclaringTypes(typeof(ScriptingInterface).Assembly);
            foreach (var type in scriptingInterfaces)
            {
                var generator = new Generator(type, kharynic.BuildInfo.RootNamespace, scriptHeader);
                generator.Run();
                Debug.Log($"{generator.GetType().Name} : {type.Name} finished");
            }
            UnityEditor.AssetDatabase.Refresh();
        }

        public void OnPreprocessBuild(UnityEditor.BuildTarget target, string path)
        {
            Run();
        }
    }
}
