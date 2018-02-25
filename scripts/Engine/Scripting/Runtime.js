org = org || {};
org.kharynic = org.kharynic || {};
org.kharynic.Scripting = org.kharynic.Scripting || {};

org.kharynic.Scripting.Runtime = class
{
    static Init(emscriptenModule)
    {
        this.constructor._emscriptenModule = emscriptenModule;
        this.constructor.DynCall = emscriptenModule.Runtime.dynCall;
        this.constructor.GetStringFromPtr = emscriptenModule.UTF8ToString;
    }

    static GetPtrFromString(str) 
    {
        var bufferSize = this.constructor.emscriptenModule.lengthBytesUTF8(str) + 1;
        var buffer = this.constructor.emscriptenModule._malloc(bufferSize);
        this.constructor.emscriptenModule.stringToUTF8(str, buffer, bufferSize);
        return buffer;
    }
}

console.log("Runtime.js loaded");
