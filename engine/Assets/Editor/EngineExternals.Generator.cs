using UnityEditor;
using UnityEditor.Build;

namespace org.kharynic.Editor
{
    public partial class EngineExternals
    {
        public class Generator : IPreprocessBuild
        {
            public int callbackOrder => 2;
            
            public void OnPreprocessBuild(BuildTarget target, string path)
            {
                Debug.Log($"{nameof(EngineExternals)}.{nameof(Generator)}.{nameof(OnPreprocessBuild)}");
            }

            private void GeneratePInvokeMethodWrappers()
            {
                
            }

            private void GenerateScriptWrappers()
            {
                // function getPtrFromString(str) {
                //     var bufferSize = lengthBytesUTF8(str) + 1;
                //     var buffer = _malloc(bufferSize);
                //     stringToUTF8(str, buffer, bufferSize);
                //     return buffer;
                // }
                // 
                // var param = getPtrFromString("world");
                // Runtime.dynCall('vi', delegateFunctionPtr, [param]);
            }
        }
    }
}