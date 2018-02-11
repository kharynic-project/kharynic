// generated on 2018.02.11 19:07 by org.kharynic.Editor.EngineExternals+Generator.GenerateScriptWrappers
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.39+d22
// *********************

window.Engine = window.Engine || {};

window.Engine.Hello = function(message /*: String*/) /*: Void*/
{
    message = this.GetPtrFromString(message);
    var sig = "vi";
    var ptr = this.externals.Hello;
    var args = [ message ];
    this.dynCall(sig, ptr, args);
}

window.Engine.Add = function(a /*: Int32*/, b /*: Int32*/) /*: Int32*/
{
    var sig = "iii";
    var ptr = this.externals.Add;
    var args = [ a,b ];
    return this.dynCall(sig, ptr, args);
}

console.log("Engine.generated.js loaded");
