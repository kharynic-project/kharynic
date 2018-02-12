using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace org.kharynic
{
    public class CoroutineManager : IDisposable
    {
        private readonly Dictionary<string, UnityEngine.Coroutine> _coroutines = new Dictionary<string, UnityEngine.Coroutine>();
        private CoroutineManagerUnityHost _unityHost;
        private int _nextId = 0;

        private class CoroutineManagerUnityHost : UnityEngine.MonoBehaviour
        {
            public CoroutineManager _manager;
            private void OnDestroy()
            {
                Debug.Log($"{nameof(CoroutineManager)}.{nameof(_unityHost)} destroying");
//                _manager._unityHost = null;
                _manager.Dispose();
            }
        }

        public void Start()
        {
            _unityHost = new UnityEngine.GameObject(nameof(CoroutineManager))
                .AddComponent<CoroutineManagerUnityHost>();
            _unityHost._manager = this;
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(Start)}");
        }

        private IEnumerator CreateEnumerator(
            Action loop, 
            TimeSpan interval, 
            string id,
            bool autoRestart)
        {
            while (true)
            {
                try
                {
                    loop();
                }
                catch (TaskCanceledException)
                {
                    Debug.Log($"Coroutine finished: {id}");
                    _coroutines.Remove(id);
                    yield break;
                }
                catch (Exception e)
                {
                    Debug.Log($"Coroutine crashed{(autoRestart ? ", restarting" : "")}: {id} {e}");
                    if (!autoRestart)
                    {
                        _coroutines.Remove(id);
                        yield break;
                    }
                }
                if (interval != default(TimeSpan))
                    yield return new UnityEngine.WaitForSeconds((float) interval.TotalSeconds);
                else
                    yield return null;
            }
        }
        
        public string StartCoroutine(
            Action loop, 
            TimeSpan interval = default(TimeSpan),
            bool autoRestart = false,
            [CallerFilePath] string callerFilePath = "/anonymous.")
        {
            if (_unityHost == null)
                throw new ObjectDisposedException(nameof(CoroutineManager));
            var nameStart = callerFilePath.LastIndexOfAny(new[] {'/', '\\'}) + 1;
            var nameLength = callerFilePath.LastIndexOf('.') - nameStart;
            var id = $"{_nextId}.{callerFilePath.Substring(nameStart, nameLength)}";
            _nextId += 1;
            var enumerator = CreateEnumerator(loop, interval, id, autoRestart);
            var coroutine = _unityHost.StartCoroutine(enumerator);
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(StartCoroutine)}({id})");
            _coroutines[id] = coroutine;
            return id;
        }

        public bool IsRunning(string id)
        {
            return _coroutines.ContainsKey(id);
        }

        public void StopCoroutine(string id)
        {
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(StopCoroutine)}({id})");
            if (_unityHost == null)
                throw new ObjectDisposedException(nameof(CoroutineManager));
            if (!_coroutines.ContainsKey(id))
                Debug.Log($"Coroutine {id} cannot be stopped as its not running");
            var coroutine = _coroutines[id];
            _coroutines.Remove(id);
            _unityHost.StopCoroutine(coroutine);
            Debug.Log($"Coroutine stopped: {id}");
        }

        public void Dispose()
        {
            Debug.Log($"{nameof(CoroutineManager)}.{nameof(Dispose)}");
            if (_unityHost != null)
            {
                // haven't been destroyed before this object
                foreach (var coroutine in _coroutines.Keys.ToArray())
                    StopCoroutine(coroutine);
                UnityEngine.Object.Destroy(_unityHost?.gameObject);
            }
        }
    }
}