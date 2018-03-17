window.Kharynic = window.Kharynic || {};
Kharynic.WebHost = Kharynic.WebHost || {};

Kharynic.WebHost.Splash = class
{
    constructor(host /*: HTMLElement*/)
    {
        this._element = host.ownerDocument.createElement("div");

        this._element.id = "Splash";
        var unityLogo = host.ownerDocument.createElement("img");
        unityLogo.id = this.constructor.UnityLogoId;
        unityLogo.src = this.constructor.UnityLogoPath;
        this._element.appendChild(unityLogo);
        Kharynic.WebHost.WebHost.Maximize(this._element);
        host.appendChild(this._element);
    }

    Remove()
    {
        this._element.remove();
    }
}
Kharynic.WebHost.Splash.UnityLogoId = "unityLogo";
Kharynic.WebHost.Splash.UnityLogoPath = "/resources/images/unity.png";
