# Script loading
All scripts are listed in hierarchy of .src files and loaded by ScriptLoader
just after game's webpage is loaded.
Script loading cannot have any side effects, only define classes under global
object Kharynic. Thanks to this they can be loaded in any order and in parallel.
If class needs initialization, it defines static method Init(), which will be 
called by ScriptLoader after all scripts are loaded. 

# Initialization sequence
- JS: [index.html] > ScriptLoader.Start > _RunInitializers
  - JS: WebHost.Init > _Load
    - JS: Player..ctor > [Unity loader]
      - C#: [Unity init] > Startup.Main
      - C#: [Unity startup] > Startup.Start
        - C#: Engine.Main
          - JS: WebHost.OnEngineStart
            - JS: Runtime.Init
          - C#: Runtime.RegisterAll
          - JS: WebHost.OnEngineReady
            - JS: Startup.StartGame
