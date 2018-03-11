using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kharynic.Engine
{
    public class CoroutineManager : IDisposable
    {
        public static TimeSpan? MinimumInterval { get; set; } = null;
        private readonly List<Coroutine> _coroutines = new List<Coroutine>();
        private CoroutineManagerUnityHost _unityHost;

        private class CoroutineManagerUnityHost : UnityEngine.MonoBehaviour { }

        public class Coroutine : IDisposable
        {
            public string Name { get; }
            public TimeSpan? Interval { get; }
            public bool AutoRestart { get; }
            public Func<bool> Loop { get; }
            public bool Suspended { get; set; }
            private CoroutineManager _manager;
            private UnityEngine.Coroutine _coroutine;

            public Coroutine(
                Func<bool> loop, 
                string name = null, 
                TimeSpan? interval = null,
                bool autoRestart = false)
            {
                Name = name;
                Loop = loop;
                Interval = interval;
                AutoRestart = autoRestart;
            }

            public Coroutine(
                string name, 
                CoroutineManager manager,
                UnityEngine.Coroutine coroutine)
            {
                Name = name;
                _manager = manager;
                _coroutine = coroutine;
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
                if (Name != null)
                    Debug.Log($"{nameof(Coroutine)}.{nameof(Start)}({Name})");
                _manager._coroutines.Add(this);
            }

            private TimeSpan? Max(TimeSpan? a, TimeSpan? b)
            {
                return (a == b || a > b) ? a : b;

            }

            private IEnumerator CreateEnumerator()
            {
                while (true)
                {
                    if (Suspended)
                        yield return null;
                    var @continue = false;
                    try
                    {
                        @continue = Loop();
                    }
                    catch (TaskCanceledException)
                    {
                        @continue = false;
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
                    if (!@continue)
                    {
                        if (Name != null)
                            Debug.Log($"Coroutine finished: {Name}");
                        _manager._coroutines.Remove(this);
                        yield break;
                    }
                    var interval = Max(Interval, MinimumInterval);
                    if (interval != null)
                        yield return new UnityEngine.WaitForSeconds((float) interval.Value.TotalSeconds);
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

        [Obsolete(message:"use await or universal coroutines instead")]
        public void StartUnityCoroutine(IEnumerator routine, string name)
        {
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(StartUnityCoroutine)}({name})");
            _unityHost.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(Func<bool> loop, string name = null, TimeSpan? interval = null, bool autoRestart = false)
        {
            var coroutine = new Coroutine(loop, name, interval, autoRestart);
            coroutine.Start(this);
            return coroutine;
        }

        public Coroutine StartCoroutine(Action infiniteLoop, string name = null, TimeSpan? interval = null, bool autoRestart = false)
        {
            var loopWrapper = new Func<bool>(() => { infiniteLoop(); return true; });
            return StartCoroutine(loopWrapper, name, interval, autoRestart);
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