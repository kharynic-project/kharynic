using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.kharynic
{
    public class CoroutineManager : IDisposable
    {
        private readonly List<Coroutine> _coroutines = new List<Coroutine>();
        private CoroutineManagerUnityHost _unityHost;

        private class CoroutineManagerUnityHost : UnityEngine.MonoBehaviour { }

        public class Coroutine : IDisposable
        {
            public string Name { get; }
            public TimeSpan? Interval { get; }
            public bool AutoRestart { get; }
            public Action Loop { get; }
            public bool Suspended { get; set; }
            private CoroutineManager _manager;
            private UnityEngine.Coroutine _coroutine;

            public Coroutine(
                Action loop, 
                string name, 
                TimeSpan? interval = null,
                bool autoRestart = false)
            {
                Name = name;
                Loop = loop;
                Interval = interval;
                AutoRestart = autoRestart;
            }

            public void Start(CoroutineManager coroutineManager)
            {
                if (_manager != null)
                    throw new InvalidOperationException("already started");
                if (coroutineManager?._unityHost == null)
                    throw new ObjectDisposedException(nameof(CoroutineManager));
                _manager = coroutineManager;
                var enumerator = CreateEnumerator();
                _coroutine = _manager._unityHost.StartCoroutine(enumerator);
                Debug.Log($"{nameof(Coroutine)}.{nameof(Start)}({Name})");
                _manager._coroutines.Add(this);
            }

            private IEnumerator CreateEnumerator()
            {
                while (true)
                {
                    if (Suspended)
                        yield return null;
                    try
                    {
                        Loop();
                    }
                    catch (TaskCanceledException)
                    {
                        Debug.Log($"Coroutine finished: {Name}");
                        _manager._coroutines.Remove(this);
                        yield break;
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Coroutine crashed{(AutoRestart ? ", restarting" : "")}: {Name} {e}");
                        if (!AutoRestart)
                        {
                            _manager._coroutines.Remove(this);
                            yield break;
                        }
                    }
                    if (Interval != null)
                        yield return new UnityEngine.WaitForSeconds((float) Interval.Value.TotalSeconds);
                    else
                        yield return null;
                }
            }
            
            public void Stop()
            {
                Debug.Log($"{nameof(Coroutine)}.{nameof(Stop)}({Name})");
                if (_manager._unityHost == null)
                    return; // Unity is already shutting down anyway
                if (!IsRunning())
                    Debug.Log($"Coroutine {Name} cannot be stopped as its not running");
                _manager._coroutines.Remove(this);
                _manager._unityHost.StopCoroutine(_coroutine);
            }

            public bool IsRunning()
            {
                return _manager._coroutines.Contains(this);
            }

            public void Dispose()
            {
                if (IsRunning())
                    Stop();
            }
        }

        public void Start()
        {
            _unityHost = new UnityEngine.GameObject(nameof(CoroutineManager))
                .AddComponent<CoroutineManagerUnityHost>();
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(Start)}");
        }

        public void Dispose()
        {
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(Dispose)}");
            foreach (var coroutine in _coroutines.ToArray())
                coroutine.Stop();
            if (_unityHost != null)
            {
                // haven't been destroyed before this object
                UnityEngine.Object.Destroy(_unityHost?.gameObject);
            }
        }
    }
}