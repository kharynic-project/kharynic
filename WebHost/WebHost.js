window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

Kharynic.WebHost.WebHost = class 
{
    constructor(host /*: HTMLElement*/)
    {
        this._host = host;
        this._scriptLoader = new Kharynic.WebHost.ScriptLoader(host);
        this._splash = null;
        this._player = null;

        this._ShowWatermark();
        this._Load();
    }

    static Init()
    {
        console.log("WebHost.Init")
        this.Instance = new this(this.DefaultHost);
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
