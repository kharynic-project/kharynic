window.Engine = window.Engine || {};

// window.Engine.emscriptenModule = undefined;
// window.Engine.externals = undefined;
// window.Engine.dynCall = undefined;
// window.Engine.GetStringFromPtr = undefined;

window.Engine.Init = function(emscriptenModule, externals) {
    // this.emscriptenModule = emscriptenModule;
    // this.externals = externals;
    // this.dynCall = emscriptenModule.Runtime.dynCall;
    // this.GetStringFromPtr = emscriptenModule.UTF8ToString;
    org.kharynic.Scripting.Runtime.Init(emscriptenModule);
};

// window.Engine.GetPtrFromString = function (str) 
// {
//     var bufferSize = this.emscriptenModule.lengthBytesUTF8(str) + 1;
//     var buffer = this.emscriptenModule._malloc(bufferSize);
//     this.emscriptenModule.stringToUTF8(str, buffer, bufferSize);
//     return buffer;
// };

console.log("Engine.js loaded");
