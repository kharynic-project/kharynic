// This file defines scripts interface exposed to the engine.
// It is used by build script to generate C# interface and .jslib Emscripten wrapper.
// For this reason it needs type annotations in block comments.
window.Kharynic = window.Kharynic || {};

// TODO: merge with WebHost?
Kharynic.WebHost.EngineInterface = 
{
    // called from Kharynic.Engine.Engine.Main
    // prepares game display and prepares scripting
    OnLoad: function () /*: void*/
    {
        Kharynic.WebHost.WebHost.Instance.OnEngineStart();
    },
    
    Log: function (message /*: string*/) /*: void*/
    {
        window.console.log(message); 
    },

    GetRootUrl: function() /*: string*/
    {
        return document.location.href.replace(/\/(index.html)?$/i, "");
    }
};
