using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kharynic.Engine.Unity
{
    public static class GraphicsSettings
    {
        private static bool _highGraphicSettings = false;
        private static int _graphicSettingsSwitchCount = 0;
        private static float _framerate = 40f;
        
        private static void SetGraphicSettings(bool high)
        {
            _graphicSettingsSwitchCount++;
            _highGraphicSettings = high;
            QualitySettings.shadows = high ? ShadowQuality.All : ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = high ? ShadowResolution.High : ShadowResolution.Low;
            QualitySettings.shadowCascades = high ? 4 : 1;
            QualitySettings.shadowDistance = high ? 100 : 50;
            QualitySettings.antiAliasing = high ? 2 : 0;
            foreach (var light in UnityEngine.Object.FindObjectsOfType<Light>())
            {
                light.shadows = high ? LightShadows.Soft : LightShadows.Hard;
            }
        }

        public static void Init()
        {
            SetGraphicSettings(high: false);
            DisableAmbientLighting();
        }
        
        

        private static void DisableAmbientLighting()
        {
//            RenderSettings.fog = true;
//            RenderSettings.fogColor = Color.black;
//            RenderSettings.fogMode = FogMode.ExponentialSquared;
//            RenderSettings.fogDensity = 0.02f;
            RenderSettings.ambientIntensity = 0;
            RenderSettings.reflectionIntensity = 0;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
        }
        
        private static void MonitorFramerate(TimeSpan interval)
        {
            var lastFrameCount = 0;
            var lastTotalTime = 0f;
            Engine.Instance.CoroutineManager.StartCoroutine(() =>
            {
                var frameCount = Time.frameCount;
                var totalTime = Time.realtimeSinceStartup;
                _framerate = (frameCount - lastFrameCount) / (totalTime - lastTotalTime);
                lastFrameCount = frameCount;
                lastTotalTime = totalTime;
                if (_framerate > 50 && !_highGraphicSettings && _graphicSettingsSwitchCount < 3)
                {
                    Debug.Log($"{_framerate}fps, switching graphic settings to high");
                    SetGraphicSettings(true);
                }
                else if (_framerate < 30 && _highGraphicSettings)
                {
                    Debug.Log($"{_framerate}fps, switching graphic settings to low");
                    SetGraphicSettings(false);
                }
            }, nameof(MonitorFramerate), interval);
        }
    }
}