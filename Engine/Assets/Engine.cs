using System;
using System.Linq;
using Kharynic.Engine.Scripting;
using Kharynic.WebHost;

namespace Kharynic.Engine
{
	public class Engine : IDisposable
	{
		public static Engine Instance { get; } = new Engine();
		public CoroutineManager CoroutineManager { get; }
		public bool DebugMode { get; set; } = true;

		private Engine()
		{
			CoroutineManager = new CoroutineManager();
		}

		// called after Unity game loop is started
		public void Main(string[] args)
		{
			LogBuildInfo();
			CoroutineManager.Start();
			WebHost.WebHost.OnEngineStart();
			Runtime.RegisterAll(GetType().Assembly);
			WebHost.WebHost.OnEngineReady();
		}

		public void Dispose()
		{
			Debug.Log("engine shutdown");
			CoroutineManager.Dispose();
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
	}
}
