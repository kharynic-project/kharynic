using System;
using System.Collections;
using UnityEngine;

namespace org.kharynic
{
    public class UnityBridge : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Main()
        {
            var gameObject = new GameObject(nameof(UnityBridge));
            gameObject.AddComponent<UnityBridge>();
        }

        public void Start()
        {
            Engine.Instance.Main(Environment.GetCommandLineArgs());
            StartCoroutine(LogFramerates(interval: TimeSpan.FromSeconds(3)));
        }

        public void OnDestroy()
        {
            Engine.Instance.Dispose();
        }

        private bool _highGraphicSettings = false;
        private int _graphicSettingsSwitchCount = 0;
        private bool HighGraphicSettings
        {
            get { return _highGraphicSettings; }
            set
            {
                if (value == _highGraphicSettings)
                    return;
                _graphicSettingsSwitchCount++;
                Debug.Log($"Switching graphic settings to {(value ? "high" : "low")}");
                _highGraphicSettings = value;
                QualitySettings.shadows = value ? ShadowQuality.All : ShadowQuality.HardOnly;
                QualitySettings.shadowResolution = value ? ShadowResolution.High : ShadowResolution.Low;
                QualitySettings.shadowCascades = value ? 4 : 1;
                QualitySettings.shadowDistance = value ? 30 : 10;
                QualitySettings.antiAliasing = value ? 2 : 0;
            }
        }
        
        IEnumerator LogFramerates(TimeSpan interval)
        {
            var lastFrameCount = 0;
            var lastTotalTime = 0f;
            while (true)
            {
                var frameCount = Time.frameCount;
                var totalTime = Time.realtimeSinceStartup;
                var framerate = (frameCount - lastFrameCount) / (totalTime - lastTotalTime);
                Debug.Log($"Framerate: {framerate:F1}fps");
                lastFrameCount = frameCount;
                lastTotalTime = totalTime;
                if (framerate > 50 && _graphicSettingsSwitchCount < 3)
                    HighGraphicSettings = true;
                else if (framerate < 30)
                    HighGraphicSettings = false;
                yield return new WaitForSeconds((float)interval.TotalSeconds);
            }
        }
    }
}
