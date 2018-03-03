using System;
using Kharynic.Engine.Scripting;

namespace Kharynic.Engine
{
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
