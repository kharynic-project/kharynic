window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

Kharynic.WebHost.WebHost = class 
{
    constructor(host /*: HTMLElement*/)
    {
        this._host = host;
        this._splash = null;
        this._player = null;

        this._ShowWatermark();
        this._Load();
    }

    // browser entry point called from /index.html
    static async Init(host /*: HTMLElement*/)
    {
        console.log("WebHost.Init")
        this.Instance = new this(host);
    }

    OnEngineStart()
    {
        console.log("WebHost.OnEngineStart")
        this._player.OnEngineStart();
        this._splash.Remove();
        Kharynic.Engine.Scripting.Runtime.Init(this._player.EmscriptenModule);
    }

    static Maximize(element /*: HTMLElement*/) 
    {
        element.style.position = "absolute";
        element.style.top = "0";
        element.style.left = "0";
        element.style.width = "100%";
        element.style.height = "100%";
        element.style.overflow = "hidden";
        element.style.border = "none";
    }

    async _Load()
    {
        await this._LoadScripts();
        console.log("all scripts loaded");
        this._splash = new Kharynic.WebHost.Splash(this._host);
        this._player = new Kharynic.WebHost.Player(this._host);
    }

    _LoadScript(path) 
    {
        var that = this;
        return new Promise(continuation => {
            var script = that._host.ownerDocument.createElement("script");
            script.onload = function()
            {
                console.log(path + " loaded");
                continuation();
            };
            script.setAttribute("src", path);
            that._host.ownerDocument.head.appendChild(script);
        });
    }

    async _LoadAllScripts(listPath, basePath) 
    {
        var loadScript = this._LoadScript;
        var response = await fetch(listPath);
        var list = await response.text();
        for (let line of list.split('\n')) 
        {
            if(!line.startsWith("// ") && line.length > 0)
                await this._LoadScript((basePath != undefined ? basePath : "") + line)
        };
    }

    async _LoadScripts()
    {
        await this._LoadAllScripts("/WebHost/filelist.txt");
        await this._LoadAllScripts("/WebHost/filelist.generated.txt");
        await this._LoadAllScripts("/Game/filelist.txt", "/Game/");
    }

    _ShowWatermark() 
    {
        var watermark = document.createElement("div");
        watermark.id = "watermark";
        watermark.textContent = "github.com/kharynic-project\nDeveloper preview - not playable yet";
        this._host.appendChild(watermark);
        fetch(this.constructor.EngineVersionFile).then(function(response) 
        {
            response.text().then(function(version) 
            {
                watermark.textContent = "Kharynic Engine v" + version + "\n" + watermark.textContent;
            })
        });
    }
}
Kharynic.WebHost.WebHost.EngineVersionFile = "/Engine/Version.txt";
Kharynic.WebHost.WebHost.Instance = undefined;
