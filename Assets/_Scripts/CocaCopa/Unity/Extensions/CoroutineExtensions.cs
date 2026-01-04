using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CocaCopa.Unity.Extensions {
    public static class CoroutineExtensions {

        public static Task AsTask(this IEnumerator coroutine, MonoBehaviour runner, CancellationToken cancellationToken = default) {
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));

            var tcs = new TaskCompletionSource<object>();

            runner.StartCoroutine(Run());

            return tcs.Task;

            IEnumerator Run() {
                while (true) {
                    if (cancellationToken.IsCancellationRequested) {
                        tcs.TrySetCanceled(cancellationToken);
                        yield break;
                    }

                    object current;
                    try {
                        if (!coroutine.MoveNext()) {
                            tcs.TrySetResult(null);
                            yield break;
                        }
                        current = coroutine.Current;
                    }
                    catch (Exception ex) {
                        tcs.TrySetException(ex);
                        yield break;
                    }

                    yield return current;
                }
            }
        }
    }
}
