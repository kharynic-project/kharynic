// generated on 2018.02.25 22:32 by /engine/Assets/Scripting/Generator.cs
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.79+943
// *********************


org = org || {};
org.kharynic = org.kharynic || {};

org.kharynic.ScriptingInterface = class
{
    static Hello(message /*: System.String*/) /*: System.Void*/
    {
        message = org.kharynic.Scripting.Runtime.GetPtrFromString(message);
        var sig = "vi";
        var ptr = this.constructor.HelloPtr;
        var args = [ message ];
        var result = org.kharynic.Scripting.Runtime.DynCall(sig, ptr, args);
    }
    static Add(a /*: System.Int32*/, b /*: System.Int32*/) /*: System.Int32*/
    {
        var sig = "iii";
        var ptr = this.constructor.AddPtr;
        var args = [ a, b ];
        var result = org.kharynic.Scripting.Runtime.DynCall(sig, ptr, args);
        return result;
    }
    static GetEngine() /*: System.IntPtr*/
    {
        var sig = "i";
        var ptr = this.constructor.GetEnginePtr;
        var args = [  ];
        var result = org.kharynic.Scripting.Runtime.DynCall(sig, ptr, args);
        return result;
    }
}