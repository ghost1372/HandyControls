#if !NET40
using System;
using System.Threading.Tasks;

namespace HandyControl.Tools.Extension
{
    public static class TaskExtension
    {
        public static Task ContinueWith(this Action action, Action continuationAction, bool runCompletedActionInUIThread = true)
        {
            var taskScheduler = runCompletedActionInUIThread
                    ? TaskScheduler.FromCurrentSynchronizationContext()
                    : TaskScheduler.Current;

            return Task.Run(action).ContinueWith(unusedTask => continuationAction(), taskScheduler);
        }

        public static Task<TResult> ContinueWith<TResult>(this Action action, Func<TResult> continuationFunction, bool runCompletedActionInUIThread = true)
        {
            var taskScheduler = runCompletedActionInUIThread
                    ? TaskScheduler.FromCurrentSynchronizationContext()
                    : TaskScheduler.Current;

            return Task.Run(action).ContinueWith(unusedTask => continuationFunction(), taskScheduler);
        }

        public static Task<TResult> ContinueWith<TResult>(this Func<TResult> function, Action<TResult> continuationAction, bool runCompletedActionInUIThread = true)
        {
            var taskScheduler = runCompletedActionInUIThread
                    ? TaskScheduler.FromCurrentSynchronizationContext()
                    : TaskScheduler.Current;

            var task = Task.Run(function);
            task.ContinueWith(unusedTask => continuationAction(task.Result), taskScheduler);
            return task;
        }

        public static Task<TResult> ContinueWith<TWorkResult, TResult>(this Func<TWorkResult> function, Func<TWorkResult, TResult> continuationAction, bool runCompletedActionInUIThread = true)
        {
            var taskScheduler = runCompletedActionInUIThread
                    ? TaskScheduler.FromCurrentSynchronizationContext()
                    : TaskScheduler.Current;

            return Task.Run(function).ContinueWith(workTask => continuationAction(workTask.Result), taskScheduler);
        }

        public static Task ContinueWith(this Task task, Action continuationAction, bool runCompletedActionInUIThread = true)
        {
            var taskScheduler = runCompletedActionInUIThread
                    ? TaskScheduler.FromCurrentSynchronizationContext()
                    : TaskScheduler.Current;
            if (task.Status == TaskStatus.Created)
                task.Start();

            return task.ContinueWith(unusedTask => continuationAction(), taskScheduler);
        }
    }
}
#endif
