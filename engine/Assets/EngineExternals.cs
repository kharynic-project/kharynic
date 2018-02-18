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
    public static partial class EngineExternals
    {
        public static string GetVersion()
        {
            return BuildInfo.Version;
        }
        
        public static void Hello(string message)
        {
            Debug.Log($"EngineExternals.Hello({message})");
        }
        
        public static int Add(int a, int b)
        {
            Debug.Log($"EngineExternals.Add({a}, {b})");
            return a + b;
        }
    }
}
