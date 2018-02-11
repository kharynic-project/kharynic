using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace org.kharynic
{
	public class Engine
	{
		public static Engine Instance { get; } = new Engine();

		public void Main(string[] args)
		{
			LogBuildInfo();
			if (args.Length > 0)
				Debug.Log($"args: {string.Join(" ", args)}");
			Debug.LogEngineObjects();
			RegisterExternals();
			Scripts.OnLoad();
			CreateCamera();
		}

		private void CreateCamera()
		{
			foreach (var otherCamera in UnityEngine.Camera.allCameras)
				UnityEngine.Object.Destroy(otherCamera.gameObject);
			var gameObject = new UnityEngine.GameObject
			{
				name = "MainCamera",
				tag = "MainCamera"
			};
			var camera = gameObject.AddComponent<UnityEngine.Camera>();
			camera.clearFlags = UnityEngine.CameraClearFlags.Color;
			camera.backgroundColor = UnityEngine.Color.black;
			camera.useOcclusionCulling = true;
			camera.allowHDR = false;
			camera.allowMSAA = false;
			camera.allowDynamicResolution = true;
		}

		public void Dispose()
		{
			Debug.Log("engine shutdown");
		}

		private static void LogBuildInfo()
		{
			Debug.Log(
				$"*** Kharynic Engine v{BuildInfo.Version} - developed 2018 Kharynic Project\n" +
				$"  *** Kharynic Project: Adam Golebiowski / kharynic@avallach.ovh and others\n" +
				$"    *** compiled on {BuildInfo.BuildDate}, {BuildInfo.Type}-build, config {BuildInfo.Config}\n" +
				$"    *** built with {BuildInfo.Toolset} for {BuildInfo.Platform}+{BuildInfo.Runtime}+{BuildInfo.TranspilationTarget}\n" +
				$"    *** from: {BuildInfo.LocalProjectPath} by {BuildInfo.User}");
			#if UNITY_EDITOR
				Debug.Log($"{DateTime.Now:yyyy.MM.dd.HH:mm} Writing log to {Debug.LogFile}");
			#endif
		}
		
		private void RegisterExternals()
		{
			var wrappers = typeof(EngineExternals.Wrappers).GetMethods(BindingFlags.Public | BindingFlags.Static);
			foreach (var wrapper in wrappers)
				RegisterExternal(wrapper);
		}

		private void RegisterExternal(MethodInfo method)
		{
			var paramTypes = method.GetParameters().Select(p => p.ParameterType);
			Type delegateType;
			if (method.ReturnType == (typeof(void)))
				delegateType = Expression.GetActionType(paramTypes.ToArray());
			else
				delegateType = Expression.GetFuncType(paramTypes.Concat(new[] {method.ReturnType}).ToArray());
			var @delegate = Delegate.CreateDelegate(delegateType, method);
			var delegateFunctionPtr = Marshal.GetFunctionPointerForDelegate(@delegate);
			Scripts.RegisterExternal(method.Name, delegateFunctionPtr);
		}
	}
}
