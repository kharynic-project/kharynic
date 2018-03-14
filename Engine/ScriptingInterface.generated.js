// Code generated on 2018.03.14 by /Engine/Assets/Scripting/ScriptLayerGenerator.cs - DO NOT EDIT.
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.101+821.preview
// *********************

window.Kharynic = window.Kharynic || {};
Kharynic.Engine = Kharynic.Engine || {};

Kharynic.Engine.ScriptingInterface = class
{
    constructor(thisPtr /*: System.IntPtr*/)
    {
        this.thisPtr = thisPtr;
    }

    static Hello(message /*: System.String*/) /*: System.Void*/
    {
        message = Kharynic.Engine.Scripting.Runtime.GetPtrFromString(message);
        var sig = "vi";
        var ptr = this.HelloPtr;
        var args = [ message ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
    }

    static Add(a /*: System.Int32*/, b /*: System.Int32*/) /*: System.Int32*/
    {
        var sig = "iii";
        var ptr = this.AddPtr;
        var args = [ a, b ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
        return result;
    }

    static GetEngine() /*: System.IntPtr*/
    {
        var sig = "i";
        var ptr = this.GetEnginePtr;
        var args = [  ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
        return result;
    }
}
