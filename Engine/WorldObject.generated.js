// Code generated on 2018.03.23 by /Engine/Assets/Scripting/BindingsGenerator/EngineExports/ScriptLayerGenerator.cs - DO NOT EDIT.
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.108+db3.preview
// *********************

window.Kharynic = window.Kharynic || {};
Kharynic.Engine = Kharynic.Engine || {};

Kharynic.Engine.WorldObject = class
{
    constructor(thisPtr /*: System.IntPtr*/)
    {
        this.thisPtr = thisPtr;
    }

    static Create(meshPath /*: System.String*/) /*: Kharynic.Engine.WorldObject*/
    {
        meshPath = Kharynic.Engine.Scripting.Runtime.GetPtrFromString(meshPath);
        var sig = "ii";
        var ptr = this.CreatePtr;
        var args = [ meshPath ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
        return result;
    }

    SetPosition(x /*: System.Single*/, y /*: System.Single*/, z /*: System.Single*/) /*: System.Void*/
    {
        var sig = "vifff";
        var ptr = this.constructor.SetPositionPtr;
        var args = [ this.thisPtr, x, y, z ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
    }

    Remove() /*: System.Void*/
    {
        var sig = "vi";
        var ptr = this.constructor.RemovePtr;
        var args = [ this.thisPtr ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
    }

    SetRotation(x /*: System.Single*/, y /*: System.Single*/, z /*: System.Single*/) /*: System.Void*/
    {
        var sig = "vifff";
        var ptr = this.constructor.SetRotationPtr;
        var args = [ this.thisPtr, x, y, z ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
    }

    set Name(value /*: System.String*/) /*: System.Void*/
    {
        value = Kharynic.Engine.Scripting.Runtime.GetPtrFromString(value);
        var sig = "vii";
        var ptr = this.constructor.set_NamePtr;
        var args = [ this.thisPtr, value ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
    }

    get Name() /*: System.String*/
    {
        var sig = "ii";
        var ptr = this.constructor.get_NamePtr;
        var args = [ this.thisPtr ];
        var result = Kharynic.Engine.Scripting.Runtime.DynCall(sig, ptr, args);
        result = Kharynic.Engine.Scripting.Runtime.GetStringFromPtr(result);
        return result;
    }
}
