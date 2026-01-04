using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace CocaCopa.Unity.Extensions {
    public static class TaskExtensions {
        /// <summary>
        /// Allows yielding a Task inside a Unity coroutine.
        /// Example: yield return myTask.AsCoroutine();
        /// </summary>
        public static IEnumerator AsCoroutine(this Task task) {
            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                Debug.LogException(task.Exception);
        }

        /// <summary>
        /// Same as above, but for Task<T>.
        /// </summary>
        public static IEnumerator AsCoroutine<T>(this Task<T> task, System.Action<T> onCompleted = null) {
            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                Debug.LogException(task.Exception);
            else
                onCompleted?.Invoke(task.Result);
        }
    }
}
