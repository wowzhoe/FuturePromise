using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTutor.External.Future.Utils
{
    public static class TaskExtensions
    {
        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception;
            }
        }

        public static IEnumerable<Task<T>> GetInCompletingOrder<T>(this IEnumerable<Task<T>> source)
        {
            Task<T>[] tasks = source.ToArray();
            TaskCompletionSource<T>[] tcss = new TaskCompletionSource<T>[tasks.Length];

            int currentSlot = -1;

            for (int i = 0; i < tasks.Length; i++)
            {
                tcss[i] = new TaskCompletionSource<T>();

                tasks[i].ContinueWith(prev =>
                {

                    int indexToSet = Interlocked.Increment(ref currentSlot);

                    tcss[indexToSet].SetResult(prev.Result);

                });
            }

            return tcss.Select(t => t.Task);
        }
    }
}