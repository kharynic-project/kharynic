// Code generated on 2018.03.18 by /Engine/Assets/Scripting/ScriptLayerGenerator.cs - DO NOT EDIT.
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.104+216.preview
// *********************

window.Kharynic = window.Kharynic || {};
Kharynic.Engine = Kharynic.Engine || {};

Kharynic.Engine.Engine = class
{
    constructor(thisPtr /*: System.IntPtr*/)
    {
        this.thisPtr = thisPtr;
    }

    GetVersion() /*: System.String*/
    {
        var sig = "ii";
        var ptr = this.constructor.GetVersionPtr;
        var args = [ this.thisPtr ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
        result = Kharynic.Engine.Scripting.Runtime.GetStringFromPtr(result);
        return result;
    }
}
