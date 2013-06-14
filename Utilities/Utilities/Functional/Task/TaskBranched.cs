using System;
using System.Diagnostics.Contracts;

namespace Utilities.Functional.Task
{
    public class TaskBranched : ITask
    {
        private readonly ITask falseTask;
        private readonly Func<bool> taskShouldBeDone;
        private readonly ITask trueTask;

        public TaskBranched(ITask trueTask, ITask falseTask, Func<bool> predicate)
        {
            Contract.Requires(falseTask != null);
            Contract.Requires(predicate != null);
            Contract.Requires(trueTask != null);

            taskShouldBeDone = predicate;
            this.trueTask = trueTask;
            this.falseTask = falseTask;
        }

        public void Do()
        {
            if (taskShouldBeDone())
                trueTask.Do();
            else
                falseTask.Do();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(taskShouldBeDone != null);
            Contract.Invariant(trueTask != null);
            Contract.Invariant(falseTask != null);
        }
    }
}