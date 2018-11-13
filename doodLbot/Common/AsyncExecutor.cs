using System;
using System.Threading;
using System.Threading.Tasks;

namespace doodLbot.Common
{
    /// <summary>
    /// Executes methods in asynchronous manner. Used in event handlers where async methods are not
    /// allowed.
    /// </summary>
    public class AsyncExecutor
    {
        private readonly SemaphoreSlim sem;


        public AsyncExecutor()
        {
            this.sem = new SemaphoreSlim(1, 1);
        }


        /// <summary>
        /// Awaits for a Task asynchronously.
        /// </summary>
        /// <param name="task">Task to execute.</param>
        public void Execute(Task task)
        {
            this.sem.Wait();

            Exception tex = null;

            var are = new AutoResetEvent(false);
            _ = Task.Run(async () => {
                try {
                    await task;
                } catch (Exception ex) {
                    tex = ex;
                } finally {
                    are.Set();
                }
            });
            are.WaitOne();

            this.sem.Release();

            if (!(tex is null))
                throw tex;
        }

        /// <summary>
        /// Awaits for a Task asynchronously and returns result of type <typeparamref name="T"/>. 
        /// </summary>
        /// <typeparam name="T">Task return value type.</typeparam>
        /// <param name="task">Task to execute.</param>
        /// <returns></returns>
        public T Execute<T>(Task<T> task)
        {
            this.sem.Wait();

            Exception tex = null;
            T result = default;

            var are = new AutoResetEvent(false);
            _ = Task.Run(async () => {
                try {
                    result = await task;
                } catch (Exception ex) {
                    tex = ex;
                } finally {
                    are.Set();
                }
            });
            are.WaitOne();

            this.sem.Release();

            if (!(tex is null))
                throw tex;

            return result;
        }
    }
}
