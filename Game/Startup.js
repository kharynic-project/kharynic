window.Kharynic = window.Kharynic || {};
Kharynic.Game = Kharynic.Game || {};

Kharynic.Game.Startup = class 
{
    static StartGame()
    {
        console.log("Startup.StartGame")
        Kharynic.Engine.WorldObject.Create("resources/meshes/locations/fort-bergmar.obj");
    }
}
