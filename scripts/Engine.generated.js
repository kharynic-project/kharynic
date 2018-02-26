// generated on 2018.02.26 08:35 by /engine/Assets/Scripting/Generator.cs
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.81+e83_preview
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
        var sig = "i";
        var ptr = this.constructor.GetVersionPtr;
        var args = [  ];
        var result = org.kharynic.Scripting.Runtime.DynCall(sig, ptr, args);
        result = org.kharynic.Scripting.Runtime.GetStringFromPtr(result);
        return result;
    }
}
