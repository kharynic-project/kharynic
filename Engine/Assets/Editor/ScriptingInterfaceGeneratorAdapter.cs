using Kharynic.Engine.Scripting.BindingsGenerator.EngineExports;

namespace Kharynic.Engine.Editor
{
    public class ScriptingInterfaceGeneratorAdapter : UnityEditor.Build.IPreprocessBuild
    {
        public int callbackOrder => 2;

        [UnityEditor.MenuItem("Kharynic/Generate externals")]
        public static void Run()
        {
            var scriptHeader =
                $"// this file is used by scripts which cannot regenerate it, so it has to be commited\n\n" +
                $"// *********************\n" +
                $"//       Externals\n" +
                $"//  Phoenix V{Kharynic.Engine.BuildInfo.Version}\n" +
                $"// *********************";
            ScriptingInterfaceGenerator.GenerateAllInterfaces(typeof(ScriptingInterface).Assembly, scriptHeader);
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"{nameof(ScriptingInterfaceGeneratorAdapter)} finished");
        }

        public void OnPreprocessBuild(UnityEditor.BuildTarget target, string path)
        {
            Run();
        }
    }
}
