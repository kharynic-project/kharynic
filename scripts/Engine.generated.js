// Code generated on 2018.03.01 by /engine/Assets/Scripting/ScriptLayerGenerator.cs - DO NOT EDIT.
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.92+0ae
// *********************


window.org = window.org || {};
org.kharynic = org.kharynic || {};

org.kharynic.Engine = class
{
    constructor(thisPtr /*: System.IntPtr*/)
    {
        this.thisPtr = thisPtr;
    }

    GetVersion() /*: System.String*/
    {
        var sig = "ii";
        var ptr = this.constructor.GetVersionPtr;
        var args = [ thisPtr ];
        var result = org.kharynic.Scripting.Runtime.DynCall(sig, ptr, args);
        result = org.kharynic.Scripting.Runtime.GetStringFromPtr(result);
        return result;
    }
}
