window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

/**
 * Responsible for browser integration. Loads game engine.
 * Exposes script functions to the engine with @Export attribute.
 */
Kharynic.WebHost.WebHost = class 
{
    constructor(host /*: HTMLElement*/)
    {
        this._host = host;
        this._scriptLoader = new Kharynic.WebHost.ScriptLoader(host);
        this._console = host.ownerDocument.defaultView.console;
        this._splash = null;
        this._player = null;

        this._ShowWatermark();
        this._Load();
    }

    static Init()
    {
        this.Instance = new this(this.DefaultHost);
    }

    /**
     * Passes EmscriptenModule from loaded Unity gameInstance to Scripting.Runtime
     * to enable registration of exported engine methods and calls to them.
     */
    /**@export*/ static OnEngineStart() /*: void*/
    {
        console.log("WebHost.OnEngineStart")
        var that = this.Instance;
        that._player.OnEngineStart();
        that._splash.Remove();
        var emscriptenModule = that._player.EmscriptenModule;
        Kharynic.Engine.Scripting.Runtime.Init(emscriptenModule);
    }

    /**@export*/ static Log(message /*: string*/) /*: void*/
    {
        this.Instance._console.log(message); 
    }

    /**@export*/ static GetRootUrl() /*: string*/
    {
        var document = this.Instance._host.ownerDocument;
        return document.location.href.replace(/\/(index.html)?$/i, "");
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
        this._splash = new Kharynic.WebHost.Splash(this._host);
        this._player = new Kharynic.WebHost.Player(this._host);
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
Kharynic.WebHost.WebHost.Instance = undefined; // required by EngineInterface
Kharynic.WebHost.WebHost.DefaultHost = document.body;
