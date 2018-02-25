using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace org.kharynic.Scripting
{
    // Use this class to gather runtime method data from generated code and pass it to scripts.
    // They already have generated wrappers, but function pointers are needed to run them.
    public static class Runtime
    {
        private const BindingFlags MethodBindingFlags = BindingFlags.Public |
                                                        BindingFlags.Static |
                                                        BindingFlags.DeclaredOnly;

        public static void RegisterAll(Assembly assembly = null)
        {
            var methods = 
                GetInterfaces(assembly)
                .SelectMany(GetInterfaceData);
            foreach (var method in methods)
                RegisterExternalMethod(method.Key, method.Value);
        }

        public static IntPtr GetPointer(object o)
        {
            var handle = GCHandle.Alloc(o, GCHandleType.Normal);
            return GCHandle.ToIntPtr(handle);
        }

        private static IEnumerable<Type> GetInterfaces(Assembly assembly = null)
        {
            var assemblies = assembly != null ? new[] {assembly} : AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.GetCustomAttribute<GeneratedInterfaceAttribute>() != null);
        }

        private static Dictionary<string, IntPtr> GetInterfaceData(Type type)
        {
            return type
                .GetMethods(MethodBindingFlags)
                .ToDictionary(GetQualifiedName, GetPointer);
        }

        private static IntPtr GetPointer(MethodInfo m)
        {
            var delegateType = m.DeclaringType?.GetNestedType($"{m.Name}{nameof(Delegate)}");
            System.Diagnostics.Debug.Assert(delegateType != null);
            var @delegate = Delegate.CreateDelegate(delegateType, m);
            return Marshal.GetFunctionPointerForDelegate(@delegate);
        }

        private static string GetQualifiedName(MethodInfo m)
        {
            System.Diagnostics.Debug.Assert(m.DeclaringType != null);
            return $"{m.DeclaringType.FullName}.{m.Name}";
        }

#if !UNITY_EDITOR
        [DllImportAttribute("__Internal")] 
        private static extern void RegisterExternalMethod(string qualifiedName, IntPtr pointer);
#else
        private static void RegisterExternalMethod(string qualifiedName, IntPtr pointer) {}
#endif
    }
}