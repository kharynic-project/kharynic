mergeInto(LibraryManager.library, {
  OnLoad: function () {
    window.WebHost.Scripts.OnLoad();
  },
  Execute: function (code) {
    window.WebHost.Scripts.Execute(Pointer_stringify(code));
  },
  Log: function (message) {
    window.WebHost.Scripts.Log(Pointer_stringify(message)); 
  },
  RegisterExternal: function(name, functionPtr) {
    window.WebHost.Scripts.RegisterExternal(Pointer_stringify(name), functionPtr);
  }
});
