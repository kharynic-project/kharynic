using System;
using System.Net;
using System.Threading.Tasks;
using Kharynic.Engine.Unity;

namespace Kharynic.Engine
{
    public class Resources
    {
        private static readonly Lazy<string> LazyGameUrl = new Lazy<string>(() => {
            if (BuildInfo.IsWebApp)
                return WebHost.EngineInterface.GetRootUrl();
            var fileProtocol = BuildInfo.LocalProjectPath.Contains(":") ? "file:///" : "file://";
            return $"{fileProtocol}{BuildInfo.LocalProjectPath}";
        });

        private static string GameUrl => LazyGameUrl.Value;

        private static string ToUrl(string projectRelativePath) => $"{GameUrl}/{projectRelativePath}";

        public static async Task<string> LoadText(string path)
        {
            using (var www = new UnityEngine.WWW(ToUrl(path)))
            {
                await www;
                if (!string.IsNullOrEmpty(www.error) || string.IsNullOrEmpty(www.text))
                    throw new WebException(www.error);
                return www.text;
            }
        }
    }
}
