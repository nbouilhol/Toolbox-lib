using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace Utilities.Functional.Task
{
    public static class TaskExtensions
    {
        public static ITask AsTask(this Action action)
        {
            return new Task(action);
        }

        public static ITask If(this ITask task, Func<bool> predicate)
        {
            Contract.Requires(task != null);
            Contract.Requires(predicate != null);

            return new TaskPredicated(task, predicate);
        }

        public static ITask Else(this ITask task, Func<bool> predicate, ITask elseTask)
        {
            Contract.Requires(task != null);
            Contract.Requires(predicate != null);

            return new TaskBranched(task, elseTask, predicate);
        }

        public static ITask Else(this ITask task, Func<bool> predicate, Action elseAction)
        {
            Contract.Requires(task != null);
            Contract.Requires(predicate != null);

            return new TaskBranched(task, elseAction.AsTask(), predicate);
        }

        public static ITask Subscribe(this ITask task, params ITask[] tasks)
        {
            return new TaskComposite(tasks.Cons(task));
        }

        public static ITask Subscribe(this ITask task, params Action[] actions)
        {
            Contract.Requires(actions != null);

            return new TaskComposite(actions.Select(action => action.AsTask()).Cons(task));
        }

        public static ITask Async(this ITask task)
        {
            Contract.Requires(task != null);

            return new TaskAsync(task);
        }

        public static ITaskCancellable Async(this ITask task, CancellationTokenSource cancellationTokenSource)
        {
            Contract.Requires(task != null);
            Contract.Requires(cancellationTokenSource != null);

            return new TaskAsync(task, cancellationTokenSource);
        }

        public static ITask WillCancel(this ITask task, ICancellable taskToCancel)
        {
            return new TaskCancel(task, taskToCancel);
        }
    }
}
