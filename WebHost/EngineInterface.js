// This file defines scripts interface exposed to the engine.
// It is used by build script to generate C# interface and .jslib Emscripten wrapper.
// For this reason it needs type annotations in block comments.
window.Kharynic = window.Kharynic || {};

Kharynic.WebHost.EngineInterface = 
{
    Externals: {},

    OnLoad: function () /*: void*/
    {
        Kharynic.WebHost.OnLoad();
        Kharynic.Engine.Scripting.Runtime.Init(Kharynic.WebHost.Player.Module);
        Kharynic.WebHost.HideSplash();
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
        Kharynic.WebHost.Scripts.Externals[name] = functionPtr;
    },

    GetRootUrl: function() /*: string*/
    {
        return document.location.href.replace(/\/(index.html)?$/i, "");
    }
};

console.log("EngineInterface.js loaded");
