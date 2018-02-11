using UnityEditor;
using UnityEditor.Build;

namespace org.kharynic.Editor
{
    public static partial class Scripts
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 3;
            
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                Debug.Log($"{nameof(Scripts)}.{nameof(Generator)}.{nameof(OnPreprocessBuild)}");
            }

            public void GenerateExternImportStubs()
            {
                
            }

            public void GenerateEditorPreviewStubs()
            {
                
            }

            public void GenerateEmscriptenLib()
            {
                // mergeInto(LibraryManager.library, {
                //     OnLoad: function () {
                //         window.Scripts.OnLoad();
                //     },
                //     Execute: function (code) {
                //         window.Scripts.Execute(Pointer_stringify(code));
                //     },
                //     Log: function (message) {
                //         window.Scripts.Log(Pointer_stringify(message)); 
                //     },
                //     RegisterExternal: function(name, functionPtr) {
                //         window.Scripts.RegisterExternal(Pointer_stringify(name), functionPtr);
                //     }
                // });
            }
        }
    }
}