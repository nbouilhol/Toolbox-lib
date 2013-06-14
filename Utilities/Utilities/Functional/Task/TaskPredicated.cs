using System;
using System.Diagnostics.Contracts;

namespace Utilities.Functional.Task
{
    public class TaskPredicated : ITask
    {
        private readonly ITask decoratedTask;
        private readonly Func<bool> taskShouldBeDone;

        public TaskPredicated(ITask task, Func<bool> predicate)
        {
            Contract.Requires(predicate != null);
            Contract.Requires(task != null);

            taskShouldBeDone = predicate;
            decoratedTask = task;
        }

        public void Do()
        {
            if (taskShouldBeDone())
                decoratedTask.Do();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(taskShouldBeDone != null);
            Contract.Invariant(decoratedTask != null);
        }
    }
}