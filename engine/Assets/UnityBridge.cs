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
        
        IEnumerator LogFramerates(TimeSpan interval)
        {
            int lastFrameCount = 0;
            float lastTotalTime = 0;
            while (true)
            {
                var frameCount = Time.frameCount;
                var totalTime = Time.realtimeSinceStartup;
                var framerate = (frameCount - lastFrameCount) / (totalTime - lastTotalTime);
                Debug.Log($"Framerate: {framerate:F1}fps");
                lastFrameCount = frameCount;
                lastTotalTime = totalTime;
                yield return new WaitForSeconds((float)interval.TotalSeconds);
            }
        }
    }
}
