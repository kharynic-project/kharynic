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
            Generator.GenerateAllInterfaces(typeof(ScriptingInterface).Assembly, scriptHeader);
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"{nameof(EcmaScriptInterfaceGeneratorAdapter)} finished");
        }

        public void OnPreprocessBuild(UnityEditor.BuildTarget target, string path)
        {
            Run();
        }
    }
}
