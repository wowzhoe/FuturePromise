using System;
using System.Threading.Tasks;

namespace SmartTutor.External.Promise
{
    public interface IPromise<T>
    {
        void OnCompleted(Action<T> onResolved);
        void OnCompleted(Action<T> onResolved, Action<Exception> onRejected);

        IPromise<T> Catch(Action<Exception> onRejected);
        IPromise<T> Then(Action<T> onResolved);
        IPromise<T> Then(Action<T> onResolved, Action<Exception> onRejected);
    }
}
