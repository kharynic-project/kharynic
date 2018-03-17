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
                return WebHost.WebHost.GetRootUrl();
            var fileProtocol = BuildInfo.LocalProjectPath.Contains(":") ? "file:///" : "file://";
            return $"{fileProtocol}{BuildInfo.LocalProjectPath}";
        });

        public static async Task<string> LoadText(string pathOrUrl)
        {
            var url = pathOrUrl.Contains("://") ? pathOrUrl : $"{LazyGameUrl.Value}/{pathOrUrl}";
            using (var www = new UnityEngine.WWW(url))
            {
                await www.WaitAsync();
                if (!string.IsNullOrEmpty(www.error) || string.IsNullOrEmpty(www.text))
                    throw new WebException(www.error);
                return www.text;
            }
        }
    }
}
