mergeInto(LibraryManager.library, 
{
    RegisterExternalMethod: function(qualifiedName, pointer)
    {
        var referenceSegments = qualifiedName.split(".");
        var parentType = window;
        for(var i=0; i<referenceSegments.length-1; i++)
            parentType = parentType[referenceSegments[i]];
        var fieldName = referenceSegments[referenceSegments.length-1] + "Ptr";
        parentType[fieldName] = pointer;
    }
});
