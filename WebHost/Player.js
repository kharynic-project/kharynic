window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

Kharynic.WebHost.Player = class 
{
    constructor(host /*: HTMLElement*/)
    {
        this._host = host;
        this._iframe = host.ownerDocument.createElement("iframe");
        this._gameContainer = null; // div parent of WebGL canvas
        this._canvas = null; // WebGL canvas
        this._playerLoadingStartTime = new Date();

        this._LockOrientation();
        this._iframe.id = this.constructor.HtmlId;
        var that = this;
        this._iframe.onload = function() { that._InitSandbox(); };
        this._iframe.src = this.constructor.Source;
        console.log("Unity: loading...");
        this._host.appendChild(this._iframe);
    }

    OnEngineStart()
    {
        var playerLoadingTime = (new Date() - this._playerLoadingStartTime) / 1000;
        console.log("Unity: loaded after " + playerLoadingTime + "s");
        this._canvas = this._gameContainer.getElementsByTagName("canvas")[0];
        this._Maximize();
        var that = this;
        this._host.onresize = function () { that._Maximize(); };
        this._canvas.addEventListener("click", function () { that._RequestFullscreen(); });
    }

    get EmscriptenModule()
    {
        return this._iframe.contentWindow.gameInstance.Module;
    }

    _LockOrientation()
    {
        var screen = this._host.ownerDocument.defaultView.screen;
        try { screen.orientation.lock('landscape').catch(function() { }); } catch(e) {}
    }

    _RequestFullscreen()
    {
        var requestFullscreen = 
            this._host.requestFullscreen ||
            this._host.mozRequestFullScreen ||
            this._host.webkitRequestFullScreen ||
            this._host.msRequestFullscreen ||
            function(){};
        var requestPointerLock = 
            this._canvas.requestPointerLock ||
            this._canvas.mozRequestPointerLock ||
            this._canvas.webkitRequestPointerLock ||
            function(){};
        requestFullscreen.call(this._host);
        requestPointerLock.call(this._canvas);
    }

    _Maximize()
    {
        Kharynic.WebHost.WebHost.Maximize(this._iframe);
        Kharynic.WebHost.WebHost.Maximize(this._gameContainer);
        Kharynic.WebHost.WebHost.Maximize(this._canvas);
    }

    _InitSandbox()
    {
        var playerWindow = this._iframe.contentWindow;
        this._SecureRequests(playerWindow);
        playerWindow.Kharynic = Kharynic;
        this._gameContainer = this._iframe.contentDocument.getElementById("gameContainer");
    }

    _SecureRequests(sandbox /*: Window*/) 
    {
        // mods/branches ran inside iframe can only use local requests
        var nativeOpen = sandbox.XMLHttpRequest.prototype.open;
        var webHostLocation = this._host.ownerDocument.location.origin;
        sandbox.XMLHttpRequest.prototype.open = function(method, url) 
        {
            if (url.indexOf("//") > 0 && !url.startsWith(webHostLocation + "/")) 
            {
                this.send = function(){ console.log("external request blocked: " + url); };
                arguments[1] = webHostLocation + "/blocked?" + encodeURIComponent(url);
            }
            nativeOpen.apply(this, arguments);
        };
    }
}
Kharynic.WebHost.Player.HtmlId = "PlayerFrame";
Kharynic.WebHost.Player.Source = "/bin/index.html";
