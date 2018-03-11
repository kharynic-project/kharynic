using System;

namespace Kharynic.Engine.Scripting
{
    // Used to mark method as accessible from scripts
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ScriptableAttribute : Attribute
    {
        
    }
    
    // Used in generated code to mark generated types.
    // Not recommended for manual use.
    [AttributeUsage(AttributeTargets.Class)]
    public class GeneratedInterfaceAttribute : Attribute
    {
        
    }
}
