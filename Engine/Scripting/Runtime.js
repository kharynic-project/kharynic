window.Kharynic = window.Kharynic || {};
Kharynic.Engine = Kharynic.Engine || {};
Kharynic.Engine.Scripting = Kharynic.Engine.Scripting || {};

Kharynic.Engine.Scripting.Runtime = class
{
    // called from Kharynic.WebHost.EngineInterface.OnLoad
    // enables calling transpiled CLR functions from scripts
    static Init(emscriptenModule)
    {
        this._emscriptenModule = emscriptenModule;
        this.GetStringFromPtr = emscriptenModule.UTF8ToString; // Pointer_stringify?
        this.DynCall = emscriptenModule.Runtime.dynCall;
        console.log("Kharynic.Engine.Scripting.Runtime: initialized")
    }

    static GetStringFromPtr(ptr /*: int*/)
    {
        throw new Error('uninitialized');
    }

    static DynCall(sig /*: string*/, ptr /*: int*/, args /*: object[]*/)
    {
        throw new Error('uninitialized');
    }

    static GetPtrFromString(str /*: string*/) 
    {
        var bufferSize = this._emscriptenModule.lengthBytesUTF8(str) + 1;
        var buffer = this._emscriptenModule._malloc(bufferSize);
        this._emscriptenModule.stringToUTF8(str, buffer, bufferSize);
        return buffer;
    }

    // called from Kharynic.Engine.Scripting.Runtime.RegisterAll
    // exposes engine methods to scripts
    static RegisterExternalMethod(qualifiedName /*: string*/, pointer /*: int*/)
    {
        var referenceSegments = this.GetStringFromPtr(qualifiedName).split(".");
        var parentType = window;
        for(var i=0; i<referenceSegments.length-1; i++)
            parentType = parentType[referenceSegments[i]];
        var fieldName = referenceSegments[referenceSegments.length-1] + "Ptr";
        parentType[fieldName] = pointer;
    }
}
