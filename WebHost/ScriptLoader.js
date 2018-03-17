window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

Kharynic.WebHost.ScriptLoader = class 
{
    constructor(host /*: HTMLElement*/)
    {
        this._host = host;
        this._loadedScripts = [];
    }

    /**
     * Browser entry point called in /index.html
     * Loads recursivly all files listed in MainSourceList.
     * Files that define class / object with qualified name corresponding to their 
     * path and static method / function Init()
     * (e.g. /WebHost/WebHost.js : window.Kharynic.WebHost.WebHost.Init()) will have
     * that method called after all scripts are loaded.
     */
    async Start()
    {
        await this._LoadAllScripts(this.constructor.MainSourceList);
        this._RunInitializers();
    }

    async _LoadAllScripts(listPath) 
    {
        var loadScript = this._LoadScript;
        var response = await fetch(listPath);
        var list = await response.text();
        console.log("loaded: " + listPath);
        for (let line of list.split('\n')) 
        {
            if(line.startsWith("// ") || line.length == 0)
                continue;
            else if (line.endsWith(this.constructor.SourceListExtension))
                await this._LoadAllScripts(line);
            else
                await this._LoadScript(line)
        };
    }

    _LoadScript(path) 
    {
        var that = this;
        return new Promise(continuation => {
            var script = that._host.ownerDocument.createElement("script");
            script.onload = function()
            {
                console.log("loaded: " + path);
                that._loadedScripts.push(path);
                continuation();
            };
            script.setAttribute("src", path);
            that._host.appendChild(script);
        });
    }

    _RunInitializers()
    {
        for(let path of this._loadedScripts)
        {
            var qualifiedNameSegments = path.split(/(?:\.js|\/)/).filter(String);
            var object = this.constructor.RootNamespace;
            for(let qualifiedNameSegment of qualifiedNameSegments)
            {
                object = object[qualifiedNameSegment];
                if (object == undefined)
                    break;
            }
            if (object == undefined)
            {
                continue;
            }
            var init = object[this.constructor.InitMethodName];
            if (typeof(init) == "function" && init.length == 0)
            {
                console.log("initializing: " + path);
                init.call(object);
            }
        }
    }
}
Kharynic.WebHost.ScriptLoader.SourceListExtension = ".src";
Kharynic.WebHost.ScriptLoader.MainSourceList = "/WebHost/WebHost.src";
Kharynic.WebHost.ScriptLoader.RootNamespace = Kharynic;
Kharynic.WebHost.ScriptLoader.InitMethodName = "Init";
