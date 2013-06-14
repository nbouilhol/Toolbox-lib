using System;

namespace Utilities.Functional.Task
{
    public class Task : ITask
    {
        private readonly Action action;

        public Task(Action action)
        {
            this.action = action;
        }

        public void Do()
        {
            action();
        }
    }
}