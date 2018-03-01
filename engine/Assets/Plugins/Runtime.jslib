mergeInto(LibraryManager.library, 
{
    RegisterExternalMethod: function(qualifiedName, pointer)
    {
        Kharynic.Engine.Scripting.Runtime.RegisterExternalMethod(qualifiedName, pointer);
    }
});
