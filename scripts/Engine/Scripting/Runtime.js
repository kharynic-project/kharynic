window.org = window.org || {};
org.kharynic = org.kharynic || {};
org.kharynic.Scripting = org.kharynic.Scripting || {};

// This class has C# part (engine/Assets/Scripting/Runtime.cs).
org.kharynic.Scripting.Runtime = class
{
    static Init(emscriptenModule)
    {
        this.constructor._emscriptenModule = emscriptenModule;
        this.constructor.GetStringFromPtr = emscriptenModule.UTF8ToString;
    }

    static GetStringFromPtr()
    {
        throw new Error('uninitialized');
    }

    static DynCall(sig /*: string*/, ptr /*: int*/, args /*: any[]*/)
    {
        return _emscriptenModule['dynCall_' + sig].apply(null, [ptr].concat(args));
    }

    static GetPtrFromString(str) 
    {
        var bufferSize = this.constructor.emscriptenModule.lengthBytesUTF8(str) + 1;
        var buffer = this.constructor.emscriptenModule._malloc(bufferSize);
        this.constructor.emscriptenModule.stringToUTF8(str, buffer, bufferSize);
        return buffer;
    }

    static RegisterExternalMethod(qualifiedName, pointer)
    {
        var referenceSegments = Pointer_stringify(qualifiedName).split(".");
        var parentType = window;
        for(var i=0; i<referenceSegments.length-1; i++)
            parentType = parentType[referenceSegments[i]];
        var fieldName = referenceSegments[referenceSegments.length-1] + "Ptr";
        parentType[fieldName] = pointer;
    }
}

console.log("Runtime.js loaded");
