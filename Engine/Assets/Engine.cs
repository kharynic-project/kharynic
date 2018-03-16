using System;
using System.Linq;
using Kharynic.Engine.Scripting;
using Kharynic.WebHost;

namespace Kharynic.Engine
{
	public class Engine : IDisposable
	{
		public static Engine Instance { get; } = new Engine();
		public TimeSpan RunningTime => DateTime.Now - _startupTime;
		public CoroutineManager CoroutineManager { get; }
		private readonly DateTime _startupTime;
		public bool DebugMode { get; set; } = true;
		private ScriptingInterface _scriptingInterface;

		private Engine()
		{
			CoroutineManager = new CoroutineManager();
			_startupTime = DateTime.Now;
			_scriptingInterface = new ScriptingInterface(this);
		}

		// called after Unity game loop is started
		public async void Main(string[] args)
		{
			LogBuildInfo();
			if (args.Any(a => !string.IsNullOrWhiteSpace(a)))
				Debug.Log($"args: {string.Join(" ", args)}");
			CoroutineManager.Start();
			Debug.Log($"loading game from {EngineInterface.GetRootUrl()}");
			// todo: specify loading order. OnLoad needs to be called before RegisterAll, 
			// as it's needed to signal that emscripten module is ready to use.
			EngineInterface.OnLoad();
			Runtime.RegisterAll(GetType().Assembly);
		}

		public void Dispose()
		{
			Debug.Log($"{nameof(Engine)}.{nameof(Dispose)}");
			CoroutineManager.Dispose();
			Debug.Log("engine shutdown");
		}

		private static void LogBuildInfo()
		{
			Debug.Log(
				$"*** Kharynic Engine v{BuildInfo.Version} - developed 2018 Kharynic Project\n" +
				$"*** Kharynic Project: Adam Golebiowski / kharynic@avallach.ovh and others\n" +
				$"*** compiled on {BuildInfo.BuildDate}, {BuildInfo.Type}-build, config {BuildInfo.Config}\n" +
				$"*** built with {BuildInfo.Toolset} for {BuildInfo.Platform}+{BuildInfo.Runtime}+{BuildInfo.TranspilationTarget}\n" +
				$"*** from: {BuildInfo.LocalProjectPath} by {BuildInfo.User}");
		}
        
		[Scriptable]
		public string GetVersion()
		{
			return BuildInfo.Version;
		}
	}
}
