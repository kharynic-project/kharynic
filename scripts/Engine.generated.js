window.Engine = window.Engine || {};

window.Engine.Hello = function(message /*: string*/)
{
    message = this.GetPtrFromString(message);
    var sig = "vi";
    var ptr = this.externals.Hello;
    var args = [ message ];
    this.dynCall(sig, ptr, args);
}

console.log("Engine.generated.js loaded");
