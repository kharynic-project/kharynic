using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace org.kharynic
{
	public class Engine : IDisposable
	{
		public static Engine Instance { get; } = new Engine();
		public TimeSpan RunningTime => DateTime.Now - _startupTime;
		private CoroutineManager CoroutineManager { get; }
		private DateTime _startupTime;
		public bool DebugMode { get; set; } = true;

		private Engine()
		{
			CoroutineManager = new CoroutineManager();
			_startupTime = DateTime.Now;
		}

		public void Main(string[] args)
		{
			LogBuildInfo();
			if (args.Any(a => !string.IsNullOrWhiteSpace(a)))
				Debug.Log($"args: {string.Join(" ", args)}");
			RegisterExternals();
			CoroutineManager.Start();
			Scripts.OnLoad();
			Debug.Log($"loading game from {Scripts.GetRootUrl()}");
		}

		public void Dispose()
		{
			Debug.Log($"{nameof(Engine)}.{nameof(Dispose)}");
			CoroutineManager.Dispose();
			Debug.Log("engine shutdown");
		}
		
		public CoroutineManager.Coroutine StartCoroutine(
			Action loop,
			string name,
			TimeSpan? interval = null,
			bool autoRestart = false)
		{
			var coroutine = new CoroutineManager.Coroutine(loop, name, interval, autoRestart);
			coroutine.Start(CoroutineManager);
			return coroutine;
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
		
		private void RegisterExternals()
		{
			var wrappers = typeof(EngineExternals.Wrappers).GetMethods(BindingFlags.Public | BindingFlags.Static);
			foreach (var wrapper in wrappers)
				RegisterExternal(wrapper);
		}

		private void RegisterExternal(MethodInfo method)
		{
			var delegateType = method.DeclaringType?.GetNestedType($"{method.Name}{nameof(Delegate)}");
			Debug.Assert(delegateType != null);
			var @delegate = Delegate.CreateDelegate(delegateType, method);
			var delegateFunctionPtr = Marshal.GetFunctionPointerForDelegate(@delegate);
			Scripts.RegisterExternal(method.Name, delegateFunctionPtr);
		}
	}
}
