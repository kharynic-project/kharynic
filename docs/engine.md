# Initialization sequence
- JS: WebHost.Init (called inline in index.html)
  - JS: WebHost.CreatePlayer
    - C#: [Unity init] > Startup.Main > [Unity startup] > Startup.Start
      - C#: Engine.Main
        - JS: EngineInterface.OnLoad
          - JS: WebHost.OnLoad
          - JS: Runtime.Init
        - C#: Runtime.RegisterAll
