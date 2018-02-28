using System;
using org.kharynic.Scripting;

namespace org.kharynic
{
    /*
     * org.kharynic.Editor.EngineExternals.Generator generates nested Wrappers
     * class containing equivalents of these methods marked with AOT.MonoPInvokeCallback.
     * They also convert Emscripten string pointers to CLR strings and are
     * accompanied by delegate declarations required for MonoPInvokeCallback.
     * Only basic parameter types are allowed, but window.JSON.stringify and
     * UnityEngine.JsonUtility.FromJson<T> can be sparingly used to pass complex
     * parameters.
     * Calling these functions requires separate JS-side wrappers using
     * WebHost.Player.Module.Runtime.dynCall.
     */
    public class ScriptingInterface
    {
        private readonly Engine _engine;

        public ScriptingInterface(Engine engine)
        {
            _engine = engine;
        }

        [Scriptable]
        public static void Hello(string message)
        {
            Debug.Log($"{nameof(ScriptingInterface)}.{nameof(Hello)}({message})");
        }
        
        [Scriptable]
        public static int Add(int a, int b)
        {
            Debug.Log($"{nameof(ScriptingInterface)}.{nameof(Add)}({a}, {b})");
            return a + b;
        }
        
        [Scriptable]
        public static IntPtr GetEngine()
        {
            return Runtime.GetPointer(Engine.Instance);
        }
    }
}
