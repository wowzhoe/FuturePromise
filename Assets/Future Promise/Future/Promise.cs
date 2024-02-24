using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartTutor.External.Future.Utils;

namespace SmartTutor.External.Promise
{
    public class Promise<T> : IPromise<T>, IRejectable, IResolvable<T>
    {
        private List<Action<T>> resolveCallbacks = new List<Action<T>>();
        private List<RejectHandler> rejectHandlers = new List<RejectHandler>();

        private Action<Action<T>, Action<Exception>> resolver;
        private Action<T> onResolved;
        private Action<Exception> onRejected;

        private Exception rejectionException;
        private T resolveValue;

        public struct RejectHandler
        {
            public Action<Exception> callback;
            public IRejectable rejectable;
        }

        public Promise() {}

        /// <summary>
        /// Begins running a given function on a background thread to resolve the future's value, as long
        /// as it is still in the Pending state.
        /// </summary>
        /// <param name="resolver">The function that will retrieve the desired value.</param>
        public Promise(Action<Action<T>, Action<Exception>> resolver)
        {
            try
            {
                resolver(Resolve, Reject);
            }
            catch (Exception ex)
            {
                Reject(ex);
            }
        }

        /// <summary>
        /// Adds a new callback to invoke if the future value is retrieved successfully.
        /// </summary>
        /// <param name="onResolved">The callback to invoke.</param>
        /// <returns>The future so additional calls can be chained together.</returns>
        public void OnCompleted(Action<T> onResolved)
        {
            Then(onResolved).Catch(Reject);
        }

        /// <summary>
        /// Adds a new callback to invoke if the future value is retrieved.
        /// </summary>
        /// <param name="onResolved">The callback to invoke if the future value is retrieved successfully.</param>
        /// <param name="onRejected">The callback to invoke if the future has an error.</param>
        /// <returns>The future so additional calls can be chained together.</returns>
        public void OnCompleted(Action<T> onResolved, Action<Exception> onRejected)
        {
            Then(onResolved, onRejected).Catch(Reject);
        }

        /// <summary>
        /// Adds a new callback to invoke if the future has an error.
        /// </summary>
        /// <param name="onRejected">The callback to invoke.</param>
        /// <returns>The future so additional calls can be chained together.</returns>
        public IPromise<T> Catch(Action<Exception> onRejected)
        {
            Promise<T> resultPromise = new Promise<T>();
            Action<Exception> rejectHandler = delegate (Exception ex)
            {
                onRejected(ex);
                resultPromise.Reject(ex);
            };
            ActionHandlers(resultPromise, null, rejectHandler);
            return resultPromise;
        }

        /// <summary>
        /// Adds a new callback to invoke if the future value is retrieved successfully.
        /// </summary>
        /// <param name="onResolved">The callback to invoke.</param>
        /// <returns>The future so additional calls can be chained together.</returns>
        public IPromise<T> Then(Action<T> onResolved)
        {
            Promise<T> resultPromise = new Promise<T>();
            Action<T> resolveHandler = delegate (T v)
            {
                onResolved?.Invoke(v);
            };

            ActionHandlers(resultPromise, resolveHandler, null);
            return resultPromise;
        }

        /// <summary>
        /// Adds a new callback to invoke if the future value is retrieved.
        /// </summary>
        /// <param name="onResolved">The callback to invoke if the future value is retrieved successfully.</param>
        /// <param name="onRejected">The callback to invoke if the future has an error.</param>
        /// <returns>The future so additional calls can be chained together.</returns>
        public IPromise<T> Then(Action<T> onResolved, Action<Exception> OnRejected)
        {
            Promise<T> resultPromise = new Promise<T>();
            Action<T> resolveHandler = delegate (T v)
            {
                onResolved?.Invoke(v);
            };
            Action<Exception> rejectHandler = delegate (Exception ex)
            {
                onRejected?.Invoke(ex);
                resultPromise.Reject(ex);
            };

            ActionHandlers(resultPromise, resolveHandler, rejectHandler);
            return resultPromise;
        }

        /// <summary>
        /// Directly call when retrieve callback for handling error result
        /// </summary>
        /// <param name="ex">The future.</param>
        public void Reject(Exception ex)
        {
            InvokeRejectHandlers(ex);
        }

        /// <summary>
        /// Directly call when retrieve callback for handling successfully result
        /// </summary>
        /// <param name="ex">The future.</param>
        public void Resolve(T value)
        {
            InvokeResolveHandlers(value);
        }

        private void ActionHandlers(IRejectable resultPromise, Action<T> resolveHandler, Action<Exception> rejectHandler)
        {
            AddResolveHandler(resolveHandler, resultPromise);
            AddRejectHandler(rejectHandler, resultPromise);
        }

        private void AddResolveHandler(Action<T> onResolved, IRejectable rejectable)
        {
            resolveCallbacks.Add(onResolved);
        }

        private void AddRejectHandler(Action<Exception> onRejected, IRejectable rejectable)
        {
            rejectHandlers.Add(new RejectHandler
            {
                callback = onRejected,
                rejectable = rejectable
            });
        }

        private void InvokeHandler<T>(Action<T> callback, IRejectable rejectable, T value)
        {
            try
            {
                callback(value);
            }
            catch (Exception ex)
            {
                rejectable.Reject(ex);
            }
        }

        private void InvokeResolveHandlers(T value)
        {
            resolveCallbacks?.Where(x => x != null).ToList().ForEach(x =>
                {
                    InvokeHandler(x, null, value);
                });
            ClearHandlers();
        }

        private void InvokeRejectHandlers(Exception ex)
        {
            rejectHandlers?.Each(delegate (RejectHandler handler)
                {
                    InvokeHandler(handler.callback, handler.rejectable, ex);
                });
            ClearHandlers();
        }

        private void ClearHandlers()
        {
            rejectHandlers = null;
            resolveCallbacks = null;
        }
    }
}