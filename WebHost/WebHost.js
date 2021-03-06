window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

/**
 * Responsible for browser integration. Loads game engine.
 * Exposes script functions to the engine with `@export` attribute.
 */
Kharynic.WebHost.WebHost = class 
{
    constructor(host /*: HTMLElement*/)
    {
        this._host = host;
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
    /**@export*/ OnEngineStart() /*: System.Void*/
    {
        console.log("WebHost.OnEngineStart")
        var emscriptenModule = this._player.EmscriptenModule;
        Kharynic.Engine.Scripting.Runtime.Init(emscriptenModule);
    }

    /**
     * Initializes game content
     */
    /**@export*/ OnEngineReady() /*: System.Void*/
    {
        console.log("WebHost.OnEngineReady");
        this._player.OnEngineReady();
        this._splash.Remove();
        Kharynic.Game.Startup.StartGame();
    }

    /**@export*/ Log(message /*: System.String*/) /*: System.Void*/
    {
        this._console.log(message); 
    }

    /**@export*/ GetRootUrl() /*: System.String*/
    {
        var document = this._host.ownerDocument;
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
        var webHostLoadingTime = (new Date() - window.performance.timing.requestStart) / 1000;
        console.log("WebHost: loaded after " + webHostLoadingTime + "s");
        this._player = new Kharynic.WebHost.Player(this._host);
    }

    async _ShowWatermark() 
    {
        var watermark = document.createElement("div");
        watermark.id = "watermark";
        watermark.textContent = "github.com/kharynic-project\nDeveloper preview - not playable yet";
        this._host.appendChild(watermark);
        var engineVersion = await (await fetch(this.constructor.EngineVersionFile)).text();
        var unityVersion = await (await fetch(this.constructor.UnityVersionFile)).text();
        unityVersion = unityVersion.split(": ")[1].replace(/\s+/, "");
        watermark.textContent = 
            "Kharynic v" + engineVersion + 
            " / Unity v" + unityVersion + "\n" + 
            watermark.textContent;
    }
}
Kharynic.WebHost.WebHost.EngineVersionFile = "/Engine/Version.txt";
Kharynic.WebHost.WebHost.UnityVersionFile = "/Engine/ProjectSettings/ProjectVersion.txt";
Kharynic.WebHost.WebHost.Instance = undefined; // required by non-static @exports
Kharynic.WebHost.WebHost.DefaultHost = document.body;
