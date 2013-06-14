using System.Diagnostics.Contracts;

namespace Utilities.Functional.Task
{
    public class TaskCancel : ITask
    {
        private readonly ICancellable objectToCancel;
        private readonly ITask taskToRun;

        public TaskCancel(ITask taskToRun, ICancellable objectToCancel)
        {
            Contract.Requires(taskToRun != null);
            Contract.Requires(objectToCancel != null);

            this.objectToCancel = objectToCancel;
            this.taskToRun = taskToRun;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(taskToRun != null);
            Contract.Invariant(objectToCancel != null);
        }

        public void Do()
        {
            objectToCancel.Cancel();
            taskToRun.Do();
        }
    }
}