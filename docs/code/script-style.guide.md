# Script style guide
Disclaimer: this is not how you normally write JavaScript. 

This project uses two-way interop between JavaScript and C#.
This style was defined this way to make mapping between scripts and engine as straightforward as possible. That's why it attempts to imitate C# approach to namespaces, classes, fields, methods, constants and static functions.

## Namespaces
Put classes in namespaces (empty objects, where root namespace is under global object). Additively re-declare namespaces before using them in a file, so that loading order doesn't matter. `namespace Kharynic.WebHost {...}` from C# corresponds to:
```
window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};
```

## Classes
Put all definitions in classes, named like containing files and with namespace corresponding to their location. Root namespace corresponds to project root directory.

| `Kharynic.WebHost.Player = class {}`     |
| ---------------------------------------- |
| `~/Projects/Kharynic/WebHost/Player.js`  |
| `https://Kharynic.org/WebHost/Player.js` |

## Methods
Define methods in class body. Use `this.` to access instance members or `this.constructor.` to access static class members.
```
Kharynic.WebHost.Player = class 
{
    Method()
    {
        console.log(this._field);
        console.log(this.constructor.Constant);
    }
}
```

## Constructors
Define constructor in class body as method called `constructor`.
```
Kharynic.WebHost.Player = class 
{
    constructor()
    {
        this.Method();
        this.constructor.StaticMethod();
    }
}
```

## Fields
Define fields in class constructor. They are only class elements allowed to be ever changed. Accessing them from outside methods is not allowed.

Always initialize all class fields on top of the constructor and separate them with empty line from the rest of constructor body. If value for a field is not available at this moment, initialize it with `null` instead.
```
Kharynic.WebHost.Player = class 
{
    constructor()
    {
        this._field = "value";
        this._initializedLater = null;

        this.Method();
    }
}
```

## Constants
Define constants as properties of class definition after its body.
```
Kharynic.WebHost.Player = class 
{
}
Kharynic.WebHost.Player.Constant = "value";
```

## Static functions
Define static functions in class body with prefix `static `. Use `this.` to access other static class members.
```
Kharynic.WebHost.Player = class 
{
    static StaticMethod()
    {
        console.log(this.Constant);
    }
}
```

## Global objects and browser APIs
### Don't directly access global object nor it's children
The only exception are namespace declarations, with root namespace under global object `window`, and `console` object which is used only for debugging.Try to write context-agnostic code which uses only dependencies explicitly passed to it.
### Use browser APIs only in WebHost
Access them only trough `_host` object referencing HTML element hosting game display. For example, if `Window` is needed there, use `_host.ownerDocument.defaultView` instead. In `Kharynic.Game` do not access anything except `WebHost`, `Engine` and language APIs that should be available in even most basic headless environments.

## Game content
Game content scripts are never accessed by engine. Because of this, rules there are looser.

Each declaration still should be under namespace `Kharynic.Game`, but there is no need to redeclare it in each file. 
Declarations don't have to be wrapped in classes - free constant objects and static functions are allowed there. 
