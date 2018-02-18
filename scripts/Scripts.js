// This file defines scripts interface exposed to the engine.
// It is used by build script to generate C# interface and .jslib Emscripten wrapper.
// For this reason it needs type annotations in block comments.
window.WebHost.Scripts = 
{
    Externals: {},

    OnLoad: function () /*: void*/
    {
        window.WebHost.OnLoad();
        window.Engine.Init(window.WebHost.Player.Module, this.Externals);
        window.WebHost.HideSplash();
    },

    Execute: function (code /*: string*/) /*: void*/
    {
        window.eval(code);
    },
    
    Log: function (message /*: string*/) /*: void*/
    {
        window.console.log(message); 
    },

    RegisterExternal: function(name /*: string*/, functionPtr /*: IntPtr*/) /*: void*/
    {
        window.WebHost.Scripts.Externals[name] = functionPtr;
    },
};
console.log("Scripts.js loaded");
