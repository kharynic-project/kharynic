using Kharynic.Engine.Scripting.BindingsGenerator.ScriptExports;

namespace Kharynic.Engine.Editor
{
    public class ScriptImportsGeneratorAdapter : UnityEditor.Build.IPreprocessBuild
    {
        public int callbackOrder => 2;

        [UnityEditor.MenuItem("Kharynic/Generate script imports")]
        public static void Run()
        {
            Generator.Run(typeof(WebHost.WebHost));
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"{nameof(ScriptImportsGeneratorAdapter)} finished");
        }

        public void OnPreprocessBuild(UnityEditor.BuildTarget target, string path)
        {
            Run();
        }
    }
}
