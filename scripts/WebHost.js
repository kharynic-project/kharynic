window.WebHost = {
    Host: undefined,
    GameContainer: undefined,
    PlayerFrame: undefined,
    PlayerCanvas: undefined,
    Player: undefined,
    Splash: undefined,
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
    Init: function(host) {
        this.Host = host;
        this.ShowSplash();
        this.CreatePlayer();
        this.PlayerFrame.style.opacity = 0;
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

        this.PlayerFrame.style.opacity = 1;
        this.Splash.remove();
        this.PlayerCanvas.addEventListener("click", this.RequestFullscreen);
    }
};
