using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace org.kharynic
{
    public class UnityBridge : MonoBehaviour
    {
        private UnityEngine.Camera _camera;
        private Engine _engine = Engine.Instance;
        
        [RuntimeInitializeOnLoadMethod]
        public static void Main()
        {
            var gameObject = new GameObject(nameof(UnityBridge));
            gameObject.AddComponent<UnityBridge>();
        }

        public void Start()
        {
            _engine.Main(Environment.GetCommandLineArgs().Skip(1).ToArray());
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
        }


        
//        IEnumerator LogFramerates(TimeSpan interval)
//        {
//            var lastFrameCount = 0;
//            var lastTotalTime = 0f;
//            var highGraphicSettings = false;
//            var graphicSettingsSwitchCount = 0;
//            var framerate = 40f;
//            Action<bool> setGraphicSettings = high =>
//            {
//                graphicSettingsSwitchCount++;
//                highGraphicSettings = high;
//                Debug.Log($"{framerate}fps, switching graphic settings to {(high ? "high" : "low")}");
//                QualitySettings.shadows = high ? ShadowQuality.All : ShadowQuality.HardOnly;
//                QualitySettings.shadowResolution = high ? ShadowResolution.High : ShadowResolution.Low;
//                QualitySettings.shadowCascades = high ? 4 : 1;
//                QualitySettings.shadowDistance = high ? 30 : 10;
//                QualitySettings.antiAliasing = high ? 2 : 0;
//            };
//            while (true)
//            {
//                yield return new WaitForSeconds((float)interval.TotalSeconds);
//                var frameCount = Time.frameCount;
//                var totalTime = Time.realtimeSinceStartup;
//                framerate = (frameCount - lastFrameCount) / (totalTime - lastTotalTime);
//                lastFrameCount = frameCount;
//                lastTotalTime = totalTime;
//                if (framerate > 50 && !highGraphicSettings && graphicSettingsSwitchCount < 3)
//                    setGraphicSettings(true);
//                else if (framerate < 30)
//                    setGraphicSettings(false);
//            }
//        }
    }
}
