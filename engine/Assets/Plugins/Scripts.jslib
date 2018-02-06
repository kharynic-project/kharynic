mergeInto(LibraryManager.library, {
  OnLoad: function () {
    window.WebHost.OnLoad();
  },
  Execute: function (code) {
    window.eval(Pointer_stringify(code));
  },
  Log: function (message) {
    window.console.log(Pointer_stringify(message)); 
  },
});
