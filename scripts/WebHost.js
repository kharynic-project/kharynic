window.WebHost = {
    Host: undefined,
    GameContainer: undefined,
    PlayerFrame: undefined,
    PlayerCanvas: undefined,
    Player: undefined,
    Splash: undefined,
    Scripts: undefined,
    ShowSplash: function() {
        this.Splash = document.createElement("div");
        this.Splash.id = "Splash";
        this.Maximize(this.Splash);
        var unityLogo = document.createElement("img");
        unityLogo.id = "unityLogo";
        unityLogo.src = "resources/images/unity.png";
        this.Splash.appendChild(unityLogo);
        this.Host.appendChild(this.Splash);
    },
    HideSplash: function() {
        this.PlayerFrame.style.opacity = 1;
        this.Splash.remove();
    },
    Maximize: function(element) {
        element.style.position = "absolute";
        element.style.top = "0";
        element.style.left = "0";
        element.style.width = "100%";
        element.style.height = "100%";
        element.style.overflow = "hidden";
        element.style.border = "none";
    },
    CreatePlayer: function() {
        this.PlayerFrame = document.createElement("iframe");
        this.PlayerFrame.id = "PlayerFrame";
        this.PlayerFrame.src = "bin/index.html";
        this.PlayerFrame.onload = function(event) {
            WebHost.PlayerFrame.contentWindow.WebHost = WebHost;
            WebHost.GameContainer = WebHost.PlayerFrame.contentDocument.getElementById("gameContainer");
        }
        this.PlayerFrame.style.opacity = 0;
        this.Host.appendChild(this.PlayerFrame);
    },
    RequestFullscreen: function () {
        var host = window.WebHost.Host;
        host.requestFullscreen = 
            host.requestFullscreen ||
            host.mozRequestFullScreen ||
            host.webkitRequestFullScreen ||
            host.msRequestFullscreen ||
            function(){};
        var canvas = window.WebHost.PlayerCanvas;
        canvas.requestPointerLock = 
            canvas.requestPointerLock ||
            canvas.mozRequestPointerLock ||
            canvas.webkitRequestPointerLock ||
            function(){};
        host.requestFullscreen();
        canvas.requestPointerLock();
    },
    LoadScript: function(path) {
        var script = document.createElement("script");
        script.setAttribute("src", "scripts/" + path);
        document.head.appendChild(script);
    },
    LoadScripts: function() {
        this.LoadScript("Scripts.js");
        this.LoadScript("Engine.js");
        this.LoadScript("Engine.generated.js");
    },
    ShowWatermark: function() {
        var watermark = document.createElement("div");
        watermark.id = "watermark";
        watermark.textContent = "github.com/kharynic-project\nDeveloper preview - not playable yet";
        this.Host.appendChild(watermark);
        var engineVersionUrl = "../engine/Assets/Editor/BuildInfo.Version.txt";
        fetch(engineVersionUrl).then(function(response) {
            response.text().then(function(version) {
                watermark.textContent = "Kharynic Engine v" + version + "\n" + watermark.textContent;
            })
        });
    },
    Init: function(host) {
        this.Host = host;
        this.ShowSplash();
        this.ShowWatermark();
        this.LoadScripts();
        this.CreatePlayer();
        try { window.screen.orientation.lock('landscape').catch(function() { }); } catch(e) {}
        this.Host.addEventListener("click", this.RequestFullscreen);
    },
    OnLoad: function () {
        this.Player = this.PlayerFrame.contentWindow.gameInstance;
        this.Maximize(this.PlayerFrame);
        this.Maximize(this.GameContainer);
        this.PlayerCanvas = this.GameContainer.getElementsByTagName("canvas")[0];
        this.Maximize(this.PlayerCanvas);
        this.Host.onresize = function () { WebHost.Maximize(WebHost.PlayerCanvas); };
        this.HideSplash();
        this.PlayerCanvas.addEventListener("click", this.RequestFullscreen);
    }
};
