using System;
using System.Diagnostics.Contracts;

namespace Utilities.Functional.Task
{
    public class TaskBranched : ITask
    {
        private readonly Func<bool> taskShouldBeDone;
        private readonly ITask trueTask;
        private readonly ITask falseTask;

        public TaskBranched(ITask trueTask, ITask falseTask, Func<bool> predicate)
        {
            Contract.Requires(falseTask != null);
            Contract.Requires(predicate != null);
            Contract.Requires(trueTask != null);

            this.taskShouldBeDone = predicate;
            this.trueTask = trueTask;
            this.falseTask = falseTask;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(taskShouldBeDone != null);
            Contract.Invariant(trueTask != null);
            Contract.Invariant(falseTask != null);
        }

        public void Do()
        {
            if (this.taskShouldBeDone())
                this.trueTask.Do();
            else
                this.falseTask.Do();
        }
    }
}