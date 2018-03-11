using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kharynic.Engine.Unity
{
    public static class AsyncExtensions
    {
        public const float MinimumTargetFps = 30f;
        public static IAwaitable WaitAsync(this IEnumerator enumerator) 
            => new Awaitable(new EnumeratorAwaiter(enumerator));
        
        public static IAwaitable WaitMoment() 
            => new Awaitable(new MomentAwaiter());
        
        // members recognized by compiler
        public interface IAwaitable
        {
            IAwaiter GetAwaiter();
        }
        
        // members recognized by compiler
        public interface IAwaiter : INotifyCompletion
        {
            bool IsCompleted { get; }
            void GetResult();
        }

        private struct Awaitable : IAwaitable
        {
            private readonly IAwaiter _awaiter;

            public Awaitable(IAwaiter awaiter)
            {
                _awaiter = awaiter;
            }
            
            public IAwaiter GetAwaiter()
            {
                return _awaiter;
            }
        }

        private class AsyncEnumeratorRunner : MonoBehaviour
        {
            public static readonly Lazy<AsyncEnumeratorRunner> Instance = 
                new Lazy<AsyncEnumeratorRunner>(() => 
                    new GameObject(nameof(AsyncEnumeratorRunner)).AddComponent<AsyncEnumeratorRunner>());
            
            private static IEnumerator CreateEnumeratorWrapper(IEnumerator enumerator, Action continuation)
            {
                yield return enumerator;
                continuation();
            }
            
            private static IEnumerator CreateActionWrapper(Action continuation)
            {
                yield return null;
                const float maxDeltaTime = 1f / MinimumTargetFps;
                while (Time.smoothDeltaTime > maxDeltaTime && UnityEngine.Random.value > 0.1)
                {
                    var waitingTime = Time.smoothDeltaTime * 2;
                    Debug.Log($"framerate below min ({1/Time.smoothDeltaTime}fps), logic paused for {waitingTime}s");
                    yield return new WaitForSecondsRealtime(waitingTime);
                }
                continuation();
            }

            public void Run(IEnumerator enumerator, Action continuation)
            {
                StartCoroutine(CreateEnumeratorWrapper(enumerator, continuation));
            }

            public void Run(Action continuation)
            {
                StartCoroutine(CreateActionWrapper(continuation));
            }
        }
        
        private class EnumeratorAwaiter : IAwaiter
        {
            private readonly IEnumerator _enumerator;

            public EnumeratorAwaiter(IEnumerator enumerator)
            {
                _enumerator = enumerator;
            }
        
            // called if IsCompleted returned false, only to pass continuation
            public void OnCompleted(Action continuation)
            {
                AsyncEnumeratorRunner.Instance.Value.Run(_enumerator, continuation);
            }

            // called once to check whether it's completed at start and continuation can be ran immediately
            // cannot actually check enumerator as this would discard first delay returned from it
            public bool IsCompleted => false;

            // actually makes no sense
            public void GetResult() { }
        }

        private class MomentAwaiter : IAwaiter
        {
            public void OnCompleted(Action continuation)
            {
                AsyncEnumeratorRunner.Instance.Value.Run(continuation);
            }

            public bool IsCompleted => false;
            public void GetResult() { }
        }
    }
}
