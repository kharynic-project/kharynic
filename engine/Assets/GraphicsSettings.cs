using System;
using UnityEngine;

namespace org.kharynic
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
            Debug.Log($"{_framerate}fps, switching graphic settings to {(high ? "high" : "low")}");
            QualitySettings.shadows = high ? ShadowQuality.All : ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = high ? ShadowResolution.High : ShadowResolution.Low;
            QualitySettings.shadowCascades = high ? 4 : 1;
            QualitySettings.shadowDistance = high ? 30 : 10;
            QualitySettings.antiAliasing = high ? 2 : 0;
        }
        
        public static void MonitorFramerate(TimeSpan interval)
        {
            var lastFrameCount = 0;
            var lastTotalTime = 0f;
            Engine.Instance.StartCoroutine(() =>
            {
                var frameCount = Time.frameCount;
                var totalTime = Time.realtimeSinceStartup;
                _framerate = (frameCount - lastFrameCount) / (totalTime - lastTotalTime);
                lastFrameCount = frameCount;
                lastTotalTime = totalTime;
                if (_framerate > 50 && !_highGraphicSettings && _graphicSettingsSwitchCount < 3)
                    SetGraphicSettings(true);
                else if (_framerate < 30 && _highGraphicSettings)
                    SetGraphicSettings(false);
            }, nameof(MonitorFramerate), interval);
        }
    }
}