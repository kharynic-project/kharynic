// generated on 2018.02.18 21:27 by org.kharynic.Editor.EngineExternals+Generator.GenerateScriptWrappers
// this file is used by scripts which cannot regenerate it, so it has to be commited

// *********************
//       Externals
//  Phoenix V0.0.76+529
// *********************

window.Engine = window.Engine || {};

window.Engine.GetVersion = function() /*: String*/
{
    var sig = "i";
    var ptr = this.externals.GetVersion;
    var args = [  ];
    var result = this.dynCall(sig, ptr, args);
    return this.GetStringFromPtr(result);
}

window.Engine.Hello = function(message /*: String*/) /*: Void*/
{
    message = this.GetPtrFromString(message);
    var sig = "vi";
    var ptr = this.externals.Hello;
    var args = [ message ];
    var result = this.dynCall(sig, ptr, args);
}

window.Engine.Add = function(a /*: Int32*/, b /*: Int32*/) /*: Int32*/
{
    var sig = "iii";
    var ptr = this.externals.Add;
    var args = [ a,b ];
    var result = this.dynCall(sig, ptr, args);
    return result;
}

console.log("Engine.generated.js loaded");
