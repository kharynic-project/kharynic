using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kharynic.Engine.Unity
{
    public static class AsyncExtensions
    {
        public static IAwaitable WaitAsync(this IEnumerator enumerator) 
            => new Awaitable(new EnumeratorAwaiter(enumerator));
        
        
        
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

            public void Run(IEnumerator enumerator, Action continuation)
            {
                StartCoroutine(CreateEnumeratorWrapper(enumerator, continuation));
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
    }
}
