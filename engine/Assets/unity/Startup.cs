using System;
using System.Linq;
using UnityEngine;

namespace org.kharynic.unity
{
    public class Startup : MonoBehaviour
    {
        private UnityEngine.Camera _camera;
        private Engine _engine = Engine.Instance; //TODO: refactor? 
        
        [RuntimeInitializeOnLoadMethod]
        public static void Main()
        {
            var gameObject = new GameObject(nameof(Startup));
            gameObject.AddComponent<Startup>();
        }

        public void Start()
        {
            _engine.Main(Environment.GetCommandLineArgs().Skip(1).ToArray());
            GraphicsSettings.Init();
            CreateCamera();
        }

        public void OnDestroy()
        {
            _engine.Dispose();
            Application.Quit();
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
            _camera = gameObject.AddComponent<UnityEngine.Camera>();
            _camera.clearFlags = UnityEngine.CameraClearFlags.Color;
            _camera.backgroundColor = UnityEngine.Color.black;
            _camera.useOcclusionCulling = true;
            _camera.allowHDR = false;
            _camera.allowMSAA = false;
            _camera.allowDynamicResolution = true;

            if (_engine.DebugMode)
            {
                gameObject.AddComponent<FreeMotionController>();
                var lightHost = new GameObject("camera light");
                lightHost.transform.parent = gameObject.transform;
                lightHost.transform.localPosition = new Vector3 {y = -1f, z = -1f};
                var light = lightHost.AddComponent<Light>();
                light.type = LightType.Spot;
                light.range = 130f;
                light.spotAngle = 110f;
                light.renderMode = LightRenderMode.ForcePixel;
                light.shadows = LightShadows.Hard;
            }
        }
    }
}
