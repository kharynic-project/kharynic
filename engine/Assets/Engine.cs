namespace org.kharynic
{
	public class Engine : UnityEngine.MonoBehaviour
	{
		private void LogBuildInfo()
		{
			Debug.Log(
				$"*** Kharynic Engine v{BuildInfo.Version} - developed 2018 Kharynic Project\n" +
			    $"  *** Kharynic Project: Adam Golebiowski / kharynic@avallach.ovh and others\n" +
			    $"    *** compiled on {BuildInfo.BuildDate}, {BuildInfo.Type}-build, config {BuildInfo.Config}\n" +
			    $"    *** built with {BuildInfo.Toolset} for {BuildInfo.Platform}+{BuildInfo.Runtime}+{BuildInfo.TranspilationTarget}\n" +
			    $"    *** from: {BuildInfo.LocalProjectPath} by {BuildInfo.User}");
		}
		public void Start()
		{
			LogBuildInfo();
			Scripts.OnLoad();
		}
	}
}
