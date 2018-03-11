using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Kharynic.Engine.Unity
{
    public static class AsyncExtensions
    {
        public interface IAwaiter : INotifyCompletion
        {
            bool IsCompleted { get; }
            void GetResult();
        }
        
        private class EnumeratorAwaiter : IAwaiter
        {
            private readonly IEnumerator _enumerator;

            public EnumeratorAwaiter(IEnumerator enumerator)
            {
                _enumerator = enumerator;
            }
        
            public void OnCompleted(Action continuation)
            {
                Engine.Instance.CoroutineManager.StartCoroutine(() =>
                {
                    if (_enumerator.MoveNext()) 
                        return true;
                    continuation();
                    return false;
                });
            }

            public bool IsCompleted => !_enumerator.MoveNext();

            public void GetResult() { }
        }
        
        public static IAwaiter GetAwaiter(this IEnumerator enumerator) => new EnumeratorAwaiter(enumerator);
    }
}
