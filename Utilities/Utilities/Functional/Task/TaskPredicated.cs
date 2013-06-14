using System;
using System.Diagnostics.Contracts;

namespace Utilities.Functional.Task
{
    public class TaskPredicated : ITask
    {
        private readonly Func<bool> taskShouldBeDone;
        private readonly ITask decoratedTask;

        public TaskPredicated(ITask task, Func<bool> predicate)
        {
            Contract.Requires(predicate != null);
            Contract.Requires(task != null);

            taskShouldBeDone = predicate;
            decoratedTask = task;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(taskShouldBeDone != null);
            Contract.Invariant(decoratedTask != null);
        }

        public void Do()
        {
            if (taskShouldBeDone())
                decoratedTask.Do();
        }
    }
}